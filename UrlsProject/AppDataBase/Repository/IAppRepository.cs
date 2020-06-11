using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlsProject.AppDataBase.Entities;

namespace UrlsProject.AppDataBase.Repository
{
    public interface IAppRepository
    {
        void Update(HostPage page);
        void Add(Host host);     
        void Update(Host host);

        Task<int> SaveChanges();

        Task<Host> GetHost(string hostUrl);
        Task<Host> GetHostIncludePages(string hostUrl);

        Task<IEnumerable<HostHistory>> GetHostHistory(string hostUrl);
    }
}
