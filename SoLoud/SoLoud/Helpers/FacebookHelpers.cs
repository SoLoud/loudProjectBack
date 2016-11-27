using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoLoud.Helpers
{
    public class Facebook
    {
        public class Me
        {
            public string id { get; set; }
            public string birthday { get; set; }
            public string email { get; set; }
            public string gedner { get; set; }
            public string name { get; set; }
            public Picture picture { get; set; }
        }

        public class Picture
        {
            public data data { get; set; }
        }

        public class data
        {
            public bool is_silhouette { get; set; }
            public string url { get; set; }
        }
    }
}