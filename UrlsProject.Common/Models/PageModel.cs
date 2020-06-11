using System.Collections.Generic;

namespace UrlsProject.Common.Models
{
    public class PageModel
    {
        public string Url { get; set; }
        public IEnumerable<HistoryModel> HostHistory { get; set; }
    }
}
