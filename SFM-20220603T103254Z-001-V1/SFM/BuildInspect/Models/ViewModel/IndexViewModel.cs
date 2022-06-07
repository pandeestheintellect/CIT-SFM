using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class IndexViewModel
    {
        public string DisplayName { get; set; }
        public string Zone { get; set; }
        public string UserType { get; set; }

        public int NoOfSchools { get; set; }

        public List<DashboardSummaryViewModel> dbsvm { get; set; }


    }
}