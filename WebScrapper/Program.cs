using CsvHelper;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Linq;

namespace WebScrapper {
    class Program {
        static protected string connstring;
        static protected SqlConnection cnn;
        static void Main(string[] args) {
            connstring = args[0];
            cnn = CreateConnection(connstring);
            //scrapeSeries();
            //scrapeSerie();
            scrapeSerieCFP();
        }

        static void scrapeSeries() {
            HtmlWeb web = new HtmlWeb();
            string url = "http://www.wikicfp.com/cfp/series?t=c&i=";
            for (char letter = 'A'; letter <= 'Z'; letter++) {
                Debug.WriteLine(letter);
                HtmlDocument doc = web.Load(url + letter);
                IEnumerable<HtmlNode> tables = doc.DocumentNode.Descendants("table");
                HtmlNode table;
                var tablesEnum = tables.GetEnumerator();
                for (int i = 0; i < 5; ++i) {
                    tablesEnum.MoveNext();
                }
                table = tablesEnum.Current;
                Debug.WriteLine("Selecting third table on page");
                parseAndStoreSeriesTable(table, 0, cnn);
                System.Threading.Thread.Sleep(5000);
            }
            
            
        }

        static void scrapeSerie() {
            var SelectCommand = new SqlCommand(String.Format("Select top 1 [Series_SK],[SeriesUrl] from cfp.wikicfp_Series where [IsScrapped]=0;"), cnn);
            cnn.Open();
            string SeriesUrl = "";
            int Series_SK = -1;
            using (SqlDataReader reader = SelectCommand.ExecuteReader()) {
                if (reader.Read()) {
                    Series_SK = (int)reader["Series_SK"];
                    SeriesUrl = (string)reader["SeriesUrl"];
                }
            }
            cnn.Close();
            while(Series_SK != -1) {                
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load("http://www.wikicfp.com" + SeriesUrl);
                if(doc != null) {
                    List<(string, int)> urls = new List<(string, int)>();
                    IEnumerable<HtmlNode> tables = doc.DocumentNode.Descendants("table");
                    HtmlNode table;
                    var tablesEnum = tables.GetEnumerator();
                    for (int i = 0; i < 6; ++i) {
                        tablesEnum.MoveNext();
                    }
                    table = tablesEnum.Current;
                    foreach (var a in table.Descendants("a")) {
                        string href = a.Attributes[0].Value;
                        urls.Add((href, Series_SK));
                    }
                    DataTable tbl = new DataTable();
                    tbl.Columns.Add(new DataColumn("CFP_URL", typeof(string)));
                    tbl.Columns.Add(new DataColumn("CFP_Series", typeof(int)));
                    foreach (var url in urls) {
                        DataRow dr = tbl.NewRow();
                        dr["CFP_URL"] = url.Item1;
                        dr["CFP_Series"] = url.Item2;
                        tbl.Rows.Add(dr);
                    }
                    SqlBulkCopy InsertBulk = new SqlBulkCopy(cnn) {
                        DestinationTableName = "cfp.wikicfp_CFP",
                        BulkCopyTimeout = 0
                    };
                    InsertBulk.ColumnMappings.Add("CFP_URL", "CFP_URL");
                    InsertBulk.ColumnMappings.Add("CFP_Series", "CFP_Series");
                    var UpdateCommand = new SqlCommand(String.Format("UPDATE cfp.wikicfp_Series SET [IsScrapped]=1  where [Series_SK]=@Series_SK;"), cnn);
                    UpdateCommand.Parameters.AddWithValue("@Series_SK", Series_SK);

                    cnn.Open();
                    InsertBulk.WriteToServer(tbl);
                    UpdateCommand.ExecuteNonQuery();
                    Series_SK = -1;
                    SeriesUrl = "";
                    using (SqlDataReader reader = SelectCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            Series_SK = (int)reader["Series_SK"];
                            SeriesUrl = (string)reader["SeriesUrl"];
                        }
                    }
                    cnn.Close();
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        static void scrapeSerieCFP() {
            var SelectCommand = new SqlCommand(String.Format("Select top 1 [CFP_SK],[CFP_Url] from [cfp].[wikicfp_CFP] where [wasExtracted]=0;"), cnn);
            cnn.Open();
            string CFP_Url = "";
            int CFP_SK = -1;
            using (SqlDataReader reader = SelectCommand.ExecuteReader()) {
                if (reader.Read()) {
                    CFP_SK = (int)reader["CFP_SK"];
                    CFP_Url = (string)reader["CFP_Url"];
                }
            }
            cnn.Close();
            while (CFP_SK != -1) {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load("http://www.wikicfp.com" + CFP_Url);
                SqlCommand InsertCommand = new SqlCommand("INSERT INTO cfp.wikicfp_CFP_tmp ([CFP_SK],[CFP_text],[SubmissionDeadline],[When],[Where]) VALUES (@CFP_SK,@CFP_text,@SubmissionDeadline,@When, @Where)", cnn);
                SqlCommand UpdateCommand = new SqlCommand(String.Format("UPDATE cfp.wikicfp_CFP SET [wasExtracted]=1  where [CFP_SK]=@CFP_SK;"), cnn);
                UpdateCommand.Parameters.AddWithValue("@CFP_SK", CFP_SK);
                SqlBulkCopy Insertbulk = new SqlBulkCopy(cnn) {
                    DestinationTableName = "cfp.wikicfp_CFP_Category_rel",
                    BulkCopyTimeout = 0
                };
                Insertbulk.ColumnMappings.Add("CFP_SK", "CFP_SK");
                Insertbulk.ColumnMappings.Add("Category_SK", "Category_SK");
                Insertbulk.ColumnMappings.Add("CategoryText", "CategoryText");
                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("CategoryText", typeof(string)));
                tbl.Columns.Add(new DataColumn("Category_SK", typeof(int)));
                tbl.Columns.Add(new DataColumn("CFP_SK", typeof(int)));
                bool itemExists = false;
                if (doc != null) {                                        
                    string text = "";
                    string subdate = "";
                    string when = "";
                    string where = "";
                    List<string> categories = new List<string>();
                    //gglu
                    // - first when where submissiondate
                    // - second categories
                    //cfp 
                    // - cfp text
                    //wikicfp_CFP_Category_rel
                    List<HtmlNode> tables = doc.DocumentNode.Descendants("table").Where(d => d.GetAttributeValue("class", "").Contains("gglu")).ToList();
                    if (tables.Count == 2) {
                        var tr = tables[0].Descendants("tr").ToList();
                        for (int i = 0; i < tr.Count; ++i) {
                            if (tr[i].InnerText.Contains("When")) {
                                when = tr[i].InnerText.Replace("When", "").Trim();
                            }
                            if (tr[i].InnerText.Contains("Where")) {
                                where = tr[i].InnerText.Replace("Where", "").Trim();
                            }
                            if (tr[i].InnerText.Contains("Submission Deadline")) {
                                subdate = tr[i].InnerText.Replace("Submission Deadline", "").Trim();
                            }
                        }
                        var a_categories = tables[1].Descendants("a").ToList();
                        for (int i = 1; i < a_categories.Count; ++i) {
                            categories.Add(a_categories[i].InnerText);
                        }
                        var cfp = doc.DocumentNode.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("cfp")).ToList();
                        text = cfp[0].InnerText.Trim();
                        InsertCommand.Parameters.AddWithValue("@CFP_SK", CFP_SK);
                        InsertCommand.Parameters.AddWithValue("@CFP_text", text);
                        InsertCommand.Parameters.AddWithValue("@SubmissionDeadline", subdate);
                        InsertCommand.Parameters.AddWithValue("@When", when);
                        InsertCommand.Parameters.AddWithValue("@Where", where);
                        if (subdate == "TBD") { InsertCommand.Parameters[2].Value = DBNull.Value; }
                        foreach (var cat in categories) {
                            DataRow dr = tbl.NewRow();
                            dr["CategoryText"] = cat;
                            dr["CFP_SK"] = CFP_SK;
                            dr["Category_SK"] = -1;
                            tbl.Rows.Add(dr);
                        }
                    }
                    

                    //TODO category relation command

                    cnn.Open();
                    if (itemExists) {
                        InsertCommand.ExecuteNonQuery();
                        Insertbulk.WriteToServer(tbl);
                    }
                    UpdateCommand.ExecuteNonQuery();//some items were deleted 
                    CFP_SK = -1;
                    CFP_Url = "";
                    using (SqlDataReader reader = SelectCommand.ExecuteReader()) {
                        if (reader.Read()) {
                            CFP_SK = (int)reader["CFP_SK"];
                            CFP_Url = (string)reader["CFP_Url"];
                        }
                    }
                    cnn.Close();
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private static void parseAndStoreSeriesTable(HtmlNode table, int ignoreRows, SqlConnection cnn) {
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("SeriesShortName", typeof(string)));
            tbl.Columns.Add(new DataColumn("SeriesName", typeof(string)));
            tbl.Columns.Add(new DataColumn("SeriesUrl", typeof(string)));
            SqlBulkCopy InsertBulk = new SqlBulkCopy(cnn) {
                DestinationTableName = "cfp.wikicfp_Series",
                BulkCopyTimeout = 0
            };
            InsertBulk.ColumnMappings.Add("SeriesShortName", "SeriesShortName");
            InsertBulk.ColumnMappings.Add("SeriesName", "SeriesName");
            InsertBulk.ColumnMappings.Add("SeriesUrl", "SeriesUrl");
            //var command1 = new SqlCommand(String.Format("TRUNCATE TABLE cfp.wikicfp_Series;"), cnn);
            //command1.ExecuteNonQuery();
            List<(string, string, string)> col = new List<(string, string, string)>();
            var tabRowEnum = table.Descendants("tr");
            
            Debug.WriteLine("parsing rows");
            int i = 0;
            foreach (var tabrow in tabRowEnum) {
                string row = tabrow.InnerHtml;
                row = row.Replace("<td align=\'left\'>\n<a href=\'", "").Replace("\'>", ";").Replace("</a> - ", ";").Replace(" </td>", "");
                row = row.Replace(">", ";");
                var parts = row.Split(';');
                DataRow dr = tbl.NewRow();
                dr["SeriesUrl"] = parts[0];
                dr["SeriesShortName"] = parts[1];
                dr["SeriesName"] = parts[2];
                tbl.Rows.Add(dr);
                if (i++ > 100) {
                    i = 0;
                    cnn.Open();
                    InsertBulk.WriteToServer(tbl);
                    cnn.Close();
                    tbl.Clear();
                }
            }            
            cnn.Open();
            InsertBulk.WriteToServer(tbl);
            cnn.Close();
        }

        static void parseAndStoreCategoryTable(HtmlNode table,int ignoreRows,SqlConnection cnn) {
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("CategoryCount", typeof(int)));
            tbl.Columns.Add(new DataColumn("CategoryName", typeof(string)));
            tbl.Columns.Add(new DataColumn("CategoryUrl", typeof(string)));
            //var command1 = new SqlCommand(String.Format("TRUNCATE TABLE cfp.wikicfp_Categories;"), cnn);
            //command1.ExecuteNonQuery();
            List<(string, string, int)> col = new List<(string, string, int)>();
            var tabEnum =table.Descendants("tr").GetEnumerator();
            tabEnum.MoveNext();
            Debug.WriteLine("parsing rows");
            for (int i = 0; i < ignoreRows; ++i) {
                tabEnum.MoveNext();
            }
            for(int i=0; i<99; i++) {
                tabEnum.MoveNext();
                var row = tabEnum.Current.InnerHtml; 
                row =row.Replace("<td><a href=\"","").Replace("</a></td><td align=\"center\">",";").Replace("</td>", ";").Replace("\">", ";");
                //Tabulka ma sest sloupcu stridaji se sloupce se jmeny kategorii ktere maji jejich url a poctem anonci
                var parts = row.Split(';');
                DataRow dr = tbl.NewRow();
                dr["CategoryUrl"] = parts[0];
                dr["CategoryName"] = parts[1];
                dr["CategoryCount"] = parts[2];
                tbl.Rows.Add(dr);
                dr = tbl.NewRow();
                dr["CategoryUrl"] = parts[3];
                dr["CategoryName"] = parts[4];
                dr["CategoryCount"] = parts[5];
                tbl.Rows.Add(dr);
                dr = tbl.NewRow();
                dr["CategoryUrl"] = parts[6];
                dr["CategoryName"] = parts[7];
                dr["CategoryCount"] = parts[8];
                tbl.Rows.Add(dr);
            }

            SqlBulkCopy objbulk = new SqlBulkCopy(cnn) {
                DestinationTableName = "cfp.wikicfp_Categories",
                BulkCopyTimeout = 0
            };
            objbulk.ColumnMappings.Add("CategoryCount", "CategoryCount");
            objbulk.ColumnMappings.Add("CategoryName", "CategoryName");
            objbulk.ColumnMappings.Add("CategoryUrl", "CategoryUrl");
            cnn.Open();
            objbulk.WriteToServer(tbl);
            cnn.Close();
        }

        static SqlConnection CreateConnection(string connString) {
            SqlConnection cnn;
            cnn = new SqlConnection(connString);
            try {
                cnn.Open();
                Debug.Write("Connection working. ");
                cnn.Close();
            }
            catch (Exception ex) {
                Debug.Write("Can not open connection ! {0}", ex.Message);
            }
            return cnn;
        }

        /*
         * Prohlizeni podle kategorii bylo omeze na pocet stranek
         * 
                 static void scrapeCategories() {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("http://www.wikicfp.com/cfp/allcat");
            IEnumerable<HtmlNode> tables = doc.DocumentNode.Descendants("table");
            HtmlNode table;
            var tablesEnum = tables.GetEnumerator();
            for (int i = 0; i < 3; ++i) {tablesEnum.MoveNext();}
            table = tablesEnum.Current;
            Debug.WriteLine("Selecting third table on page");
            parseAndStoreCategoryTable(table, 5, cnn);
        }

        static void scrapeCategory() {
            var command1 = new SqlCommand(String.Format("Select top 1 [Category_SK],[CategoryUrl] from cfp.wikicfp_Categories where [IsScrapped]=0;"), cnn);
            cnn.Open();
            string categoryUrl = "";
            int Category_SK = -1;
            using (SqlDataReader reader = command1.ExecuteReader()) {
                if (reader.Read()) {
                    Category_SK = (int)reader["Category_SK"];
                    categoryUrl = (string)reader["CategoryUrl"];
                }
            }
            cnn.Close();
            if (Category_SK != -1) {
                List<string> urls = new List<string>();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load("http://www.wikicfp.com" + categoryUrl);
                while (doc != null) {
                    IEnumerable<HtmlNode> tables = doc.DocumentNode.Descendants("table");
                    HtmlNode table;
                    var tablesEnum = tables.GetEnumerator();
                    for (int i = 0; i < 3; ++i) {tablesEnum.MoveNext();}
                    table = tablesEnum.Current;
                    //Table content
                    foreach (var a in table.Descendants("a")) {
                        string href = a.Attributes[0].Value;
                        urls.Add(href);
                    }
                    tablesEnum.MoveNext();
                    
                    table = tablesEnum.Current;
                    var tdEnum = table.Descendants("td").GetEnumerator();
                    tdEnum.MoveNext();
                    doc = null;
                    foreach (HtmlNode desc in tdEnum.Current.ChildNodes) {
                        if (desc.InnerHtml.Equals("next")) {
                            if (desc.Attributes[0].Name.Equals("href")) {
                                doc = web.Load("http://www.wikicfp.com" + desc.Attributes[0].Value);
                                break;
                            }
                            Debug.WriteLine(".");
                        }
                    };
                    
                    //bottom table with controls
                    //var xpath = "//a[text()='first']";
                    //var matchingElement = doc.evaluate(xpath, doc, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
                }
            
                // Bulk insert do sql
                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("CFP_URL", typeof(string)));
                foreach (var url in urls) {
                    DataRow dr = tbl.NewRow();
                    dr["CFP_URL"] = url;
                    tbl.Rows.Add(dr);
                }
                SqlBulkCopy objbulk = new SqlBulkCopy(cnn) {
                    DestinationTableName = "cfp.wikicfp_CFP_URL",
                    BulkCopyTimeout = 0
                };
                objbulk.ColumnMappings.Add("CFP_URL", "CFP_URL");
                cnn.Open();
                objbulk.WriteToServer(tbl);
                cnn.Close();

            
                var command2 = new SqlCommand(String.Format("UPDATE cfp.wikicfp_Categories SET [IsScrapped]=1  where [Category_SK]=@CategorySK;"), cnn);
                command2.Parameters.AddWithValue("@CategorySK", Category_SK);
                cnn.Open();
                command2.ExecuteNonQuery();
                cnn.Close();
            }
        }
         */
    }
}
