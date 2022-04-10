using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EntityCFP {
    public class Category {
        public int Category_SK { get; set; }
        public string CategoryName { get; set; }
        //public List<(int, string, int)> relatedCategories;

       

        public static List<Category> getFakeCategoriesList() {
            List<Category> list = new List<Category>();
            list.Add(new Category { Category_SK=1, CategoryName="Prvni" });
            list.Add(new Category { Category_SK = 2, CategoryName = "Druhy" });
            list.Add(new Category { Category_SK = 3, CategoryName = "Treti" });
            list.Add(new Category { Category_SK = 4, CategoryName = "Ctvrty" });
            return list;
        }

        public static List<Category> getCategoryList() {
            List<Category> list = new List<Category>();
            SqlConnection conn = new SqlConnection("Server=localhost;Database=AzureDb;Trusted_Connection=True;");
            try {
                conn.Open();
                SqlCommand command = new SqlCommand("Select TOP 10 Category_SK_A,CategoryName_A from [cfp].[wikicfp_Category_DistanceMatrix] GROUP BY Category_SK_A, CategoryName_A ORDER BY Count(Category_SK_A)",conn);
                
                using (SqlDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        list.Add(new Category{
                            Category_SK = Convert.ToInt32(reader["Category_SK_A"]),
                            CategoryName = reader["CategoryName_A"].ToString()
                        });
                    }
                }

                    conn.Close();
            }
            catch(Exception e) {
                //handle
                conn.Close(); 
            }

            return list;
        }
    }

    
}
