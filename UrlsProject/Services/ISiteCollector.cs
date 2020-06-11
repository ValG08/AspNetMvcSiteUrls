using System.Collections.Generic;
using System.Threading.Tasks;
using UrlsProject.Common.Models;

namespace UrlsProject.Services
{
    public interface ISiteCollector
    {
        Task<IEnumerable<SiteInfoModel>> Collect(string url);
    }
}