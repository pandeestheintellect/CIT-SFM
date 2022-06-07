using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ScreenViewModel
    {
        public int screen_id { get; set; }
        public int moduleid { get; set; }
        public string screen_name { get; set; }
        public string screen_class_name { get; set; }
        public string screen_url { get; set; }
        public int order_by { get; set; }

    }
}