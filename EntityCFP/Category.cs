using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCFP {
    public class Category {
        public int Category_SK;
        public string CategoryName;
        //public List<(int, string, int)> relatedCategories;

        public Category(int sk,string name) {
            Category_SK = sk;
            CategoryName = name;        
        }

        public static List<Category> getFakeCategoriesList() {
            List<Category> list = new List<Category>();
            list.Add(new Category(1, "Prvni"));
            list.Add(new Category(2, "Druhy"));
            list.Add(new Category(3, "Treti"));
            list.Add(new Category(4, "Ctvrty"));
            return list;
        }
    }

    
}
