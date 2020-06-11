using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlsProject.Common.Models
{
    public class PageInfoModel
    {
        public string Url { get; set; }

        public int MinResponse { get; set; }
        public int MaxResponse { get; set; }
        public int ResponseTime { get; set; }
    }
}
