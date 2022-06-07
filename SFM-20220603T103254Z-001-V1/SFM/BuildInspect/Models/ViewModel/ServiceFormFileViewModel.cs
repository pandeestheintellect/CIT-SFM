using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ServiceFormFileViewModel
    {
        public int ServiceFileID { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public string FileCaption { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

    }
}