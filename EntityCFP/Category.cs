using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace EntityCFP {
    public class Category {
        public int Category_SK { get; set; }
        public string CategoryName { get; set; }
        public int CategoryUsage { get; set; }

        public List<Category> relatedCategories;
        //public List<(int, string, int)> relatedCategories;



        public static List<Category> getFakeCategoriesList() {
            List<Category> list = new List<Category>();
            list.Add(new Category { Category_SK=1, CategoryName="Prvni" });
            list.Add(new Category { Category_SK = 2, CategoryName = "Druhy" });
            list.Add(new Category { Category_SK = 3, CategoryName = "Treti" });
            list.Add(new Category { Category_SK = 4, CategoryName = "Ctvrty" });
            return list;
        }
        public static Category getCategoryByID(int id) {
            Category category = new Category();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT [Category_SK],[CategoryName],[CategoryCount]"+
                                                    " FROM [cfp].[wikicfp_Categories]"+
                                                    " WHERE Category_SK = @SK", conn);
                command.Parameters.AddWithValue("@SK", id);


                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        category.Category_SK = Convert.ToInt32(reader["Category_SK"]);
                        category.CategoryName = reader["CategoryName"].ToString();
                        category.CategoryUsage = Convert.ToInt32(reader["CategoryCount"]);
                    }
                }
                category.relatedCategories = getCategoryListByRelatedID(id);
                conn.Close();
            }
            catch (Exception e) {
                //handle
                Debug.WriteLine(e.Message);
                conn.Close();
            }

            return category;
        }

        public static List<Category> getCategoryList() {
            List<Category> list = new List<Category>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT TOP 10 [Category_SK],[CategoryName],[CategoryCount] " +
                    "FROM[cfp].[wikicfp_Categories] " +
                    "ORDER BY [CategoryCount] DESC", conn);
                
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new Category{
                            Category_SK = Convert.ToInt32(reader["Category_SK"]),
                            CategoryName = reader["CategoryName"].ToString(),
                            CategoryUsage = Convert.ToInt32(reader["CategoryCount"])
                        });
                    }
                }

                    conn.Close();
            }
            catch(Exception e) {
                //handle
                Debug.WriteLine(e.Message);
                conn.Close(); 
            }

            return list;
        }

        public static List<Category> getCategoryListByRelatedID(int id) {
            List<Category> list = new List<Category>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 10 Category_SK_B,CategoryName_B,CNT FROM [cfp].[wikicfp_Category_DistanceMatrix] " +
                    "WHERE Category_SK_A=@SK " +
                    "ORDER BY CNT DESC", conn);
                command.Parameters.AddWithValue("@SK", id);
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new Category {
                            Category_SK = Convert.ToInt32(reader["Category_SK_B"]),
                            CategoryName = reader["CategoryName_B"].ToString(),
                            CategoryUsage = Convert.ToInt32(reader["CNT"])
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
