using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EntityCFP {
    public class CallForPaperSeries {
        public int CallForPaperSeries_SK;
        public string CallForPaperSeriesName;
        public string CallForPaperSeriesShortName;
        public int numberOfEntries;
        public List<CallForPaper> cfpList;

        public static CallForPaperSeries getCallForPaperSeries(int id) {
            CallForPaperSeries cfp = new CallForPaperSeries();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 1 [Series_SK],[SeriesShortName],[SeriesName],[numberOfEntries] " +
                                                    " FROM [cfp].[wikicfp_Series] series" +
                                                    " LEFT JOIN (Select Count(*) AS numberOfEntries,CFP_Series FROM cfp.wikicfp_CFP GROUP BY CFP_Series) AS subQ ON subQ.CFP_Series=Series_SK" +
                                                    " WHERE [Series_SK]=@SK", conn);
                command.Parameters.AddWithValue("@SK", id);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        cfp = new CallForPaperSeries {
                            CallForPaperSeries_SK = Convert.ToInt32(reader["Series_SK"]),
                            CallForPaperSeriesName = reader["SeriesShortName"].ToString(),
                            CallForPaperSeriesShortName = reader["SeriesName"].ToString(),
                            numberOfEntries = Convert.ToInt32(reader["numberOfEntries"])
                        };
                    }
                }
                conn.Close();
                cfp.cfpList = CallForPaper.getCallForPaperListBySeries(id);
            }
            catch (Exception e) {
                //handle
                Debug.WriteLine(e.Message);
                conn.Close();
            }
            return cfp;
        }

        public static List<CallForPaperSeries> getCallForPaperSeriesList() {
            List<CallForPaperSeries> list = new List<CallForPaperSeries>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 10 [Series_SK],[SeriesShortName],[SeriesName],[numberOfEntries] " +
                                                    " FROM [cfp].[wikicfp_Series]" +
                                                    " LEFT JOIN (Select Count(*) AS numberOfEntries,CFP_Series FROM cfp.wikicfp_CFP GROUP BY CFP_Series) AS subQ ON subQ.CFP_Series=Series_SK"
                                                    , conn);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new CallForPaperSeries {
                            CallForPaperSeries_SK = Convert.ToInt32(reader["Series_SK"]),
                            CallForPaperSeriesName = reader["SeriesShortName"].ToString(),
                            CallForPaperSeriesShortName = reader["SeriesName"].ToString(),
                            numberOfEntries = Convert.ToInt32(reader["numberOfEntries"])
                        });
                    }
                }

                conn.Close();
            }
            catch (Exception e) {
                //handle
                Debug.WriteLine(e.Message);
                conn.Close();
            }
            return list;
        }
    }

    
}
