using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EntityCFP
{
    // Consider spliting into cs files for specific entities with access to db methods
    public class CallForPaper {
        public int CallForPaper_SK;
        public int CallForPaperSeries_SK;
        public string CallForPaperSeriesName;
        public string CallForPaperText;
        public string SubmissionDeadline;
        public string When;
        public string Location;

        public static CallForPaper getCallForPaper(int id) {
            CallForPaper cfp=new CallForPaper();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 1 [CFP_SK],[Series_SK],[SeriesName],[CFP_text],[SubmissionDeadline],[When],[Where] " +
                                                    " FROM [cfp].[wikicfp_CFP] cfp " +
                                                    " LEFT JOIN[cfp].[wikicfp_Series] series ON series.Series_SK = cfp.CFP_Series" +
                                                    " WHERE CFP_SK=@SK", conn);
                command.Parameters.AddWithValue("@SK", id);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        cfp=new CallForPaper {
                            CallForPaper_SK = Convert.ToInt32(reader["CFP_SK"]),
                            CallForPaperSeries_SK = Convert.ToInt32(reader["Series_SK"]),
                            CallForPaperSeriesName = reader["SeriesName"].ToString(),
                            CallForPaperText = reader["CFP_text"].ToString(),
                            SubmissionDeadline = reader["SubmissionDeadline"].ToString(),
                            When = reader["When"].ToString(),
                            Location = reader["Where"].ToString()
                        };
                    }
                }

                conn.Close();
            }
            catch (Exception e) {
                //handle
                Debug.WriteLine(e.Message);
                conn.Close();
            }
            return cfp;
        }

        public static List<CallForPaper> getCallForPaperList() {
            List<CallForPaper> list = new List<CallForPaper>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 10 [CFP_SK],[Series_SK],[SeriesName],[CFP_text],[SubmissionDeadline],[When],[Where] "
                                                  +" FROM [cfp].[wikicfp_CFP] cfp "
                                                  + " LEFT JOIN[cfp].[wikicfp_Series] series ON series.Series_SK = cfp.CFP_Series", conn);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new CallForPaper {
                            CallForPaper_SK = Convert.ToInt32(reader["CFP_SK"]),
                            CallForPaperSeries_SK = Convert.ToInt32(reader["Series_SK"]),
                            CallForPaperSeriesName = reader["SeriesName"].ToString(),
                            CallForPaperText = reader["CFP_text"].ToString(),
                            SubmissionDeadline = reader["SubmissionDeadline"].ToString(),
                            When = reader["When"].ToString(),
                            Location = reader["Where"].ToString()
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

        public static List<CallForPaper> getCallForPaperListBySeries(int id) {
            List<CallForPaper> list = new List<CallForPaper>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 10 [CFP_SK],[CFP_text],[SubmissionDeadline],[When],[Where] "
                                                  + " FROM [cfp].[wikicfp_CFP] cfp WHERE [CFP_Series]=@SK", conn);
                command.Parameters.AddWithValue("@SK", id);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new CallForPaper {
                            CallForPaper_SK = Convert.ToInt32(reader["CFP_SK"]),
                            CallForPaperSeries_SK = id,
                            CallForPaperText = reader["CFP_text"].ToString(),
                            SubmissionDeadline = reader["SubmissionDeadline"].ToString(),
                            When = reader["When"].ToString(),
                            Location = reader["Where"].ToString()
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

        public static List<CallForPaper> getFakeCallForPaperList() {
            List<CallForPaper> list = new List<CallForPaper>();
            list.Add(new CallForPaper {
                CallForPaper_SK = 1,
                CallForPaperSeries_SK = 1,
                CallForPaperSeriesName = "Prvni",
                CallForPaperText = "Wadadadadad",
                When = "21.3.2021",
                Location = "Berlin"
            });

            list.Add(new CallForPaper {
                CallForPaper_SK = 2,
                CallForPaperSeries_SK = 2,
                CallForPaperSeriesName = "Druhy",
                CallForPaperText = "Wadadadadad",
                When = "22.3.2021",
                Location = "Online"
            });

            list.Add(new CallForPaper {
                CallForPaper_SK = 3,
                CallForPaperSeries_SK = 2,
                CallForPaperSeriesName = "Treti",
                CallForPaperText = "Wadadadadad",
                When = "23.3.2021",
                Location = "Warsava"
            });

            list.Add(new CallForPaper {
                CallForPaper_SK = 4,
                CallForPaperSeries_SK = 1,
                CallForPaperSeriesName = "Ctvrty",
                CallForPaperText = "Wadadadadad",
                When = "24.3.2021",
                Location = "London"
            });
            return list;
        }
    }

    
}
