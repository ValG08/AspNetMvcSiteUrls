using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlsProject.Common.Models
{
    public class PageResultInfoViewModel
    {
        public bool Success { get; set; }
        public string HostUrl { get; set; }
        public string Error { get; set; }
        public double AverageResponse { get; set; }

        public IEnumerable<PageInfoModel> Pages { get; set; }

        public static PageResultInfoViewModel ErrorWithResult(string hostUrl)
        {
            return new PageResultInfoViewModel
            {
                Success = false,
                Error = string.Format("Not Found {0}. ", hostUrl),
                HostUrl = hostUrl
            };
        }
    }
}
