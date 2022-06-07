using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
namespace BuildInspect.Models.ViewModel
{
  

    public class ModuleViewModel
    {
        public int SCREEN_ID { get; set; }
        public int MENUID { get; set; }
        public string SCREEN_NAME { get; set; }
        public string SCREEN_CLASS_NAME { get; set; }
        public string SCREEN_URL_NAME { get; set; }

        public int ORDER_BY { get; set; }
        public int MODULEID { get; set; }
        public int ScreenID { get; set; }

        public string MenuName { get; set; }

        public List<string> modlist { get; set; }


    }
}