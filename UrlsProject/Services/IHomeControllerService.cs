using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsProject.Common.Models;

namespace UrlsProject.Services
{
    public interface IHomeControllerService
    {
        Task<PageResultInfoViewModel> Results(string url);

        Task<HistoryViewModel> GetHostHistory(string hostUrl);
    }
}
