using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListTransFileViewModel
    {
        public int CLFileID { get; set; }
        public Nullable<int> CLTMID { get; set; }
        public string FileCaption { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

    }
}