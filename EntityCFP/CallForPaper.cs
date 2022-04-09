using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
