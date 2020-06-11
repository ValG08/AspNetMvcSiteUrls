using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlsProject.Common.Models
{
    public class HistoryViewModel
    {
        public string HostUrl { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }


        public ICollection<PageModel> HostPages { get; set; }

        public static HistoryViewModel HistoryError(string hostUrl, string message)
        {
            return new HistoryViewModel
            {
                Success = false,
                HostUrl = hostUrl,
                Error = message
            };
        }
    }
}
