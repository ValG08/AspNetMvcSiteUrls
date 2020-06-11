using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UrlsProject.AppDataBase.Entities;

namespace UrlsProject.AppDataBase.Repository
{
    public class AppRepository: IAppRepository
    {
        private readonly AppDbContext _dbContext;
      
        public AppRepository()
        {
            _dbContext = new AppDbContext();
        }    

        public void Add(Host host)
        {
            if(host == null)
            {
                return;
            }

            try
            {
                _dbContext.Entry(host).State = EntityState.Added;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(HostPage page)
        {
            if(page == null)
            {
                return;
            }

            try
            {
                HostPage findePage = _dbContext.HostPages.Find(page.Id);
                if (findePage != null)
                {
                    _dbContext.Entry(page).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public async Task<int> SaveChanges()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update(Host host)
        {
            if(host == null)
            {
                return;
            }

            try
            {
                Host findedHost = _dbContext.Hosts.Find(host.Id);
                if (findedHost != null)
                {
                    _dbContext.Entry(host).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }

        public async Task<Host> GetHost(string hostUrl)
        {
            try
            {
                if (!_dbContext.Hosts.Any())
                {
                    return null;
                }

                return await _dbContext.Hosts.FirstOrDefaultAsync(x => x.NameOfHost == hostUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
        public async Task<IEnumerable<HostHistory>> GetHostHistory(string hostUrl)
        {
            try
            {
                return await _dbContext.HostHistories
                                    .Include(x => x.Page)
                                    .Include(y => y.Page.Host)
                                    .Where(x => x.Page.Host.NameOfHost == hostUrl)
                                    .AsNoTracking()
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        public async Task<Host> GetHostIncludePages(string hostUrl)
        {
            try
            {
                return await _dbContext.Hosts
                                    .Include(x => x.HostPages)
                                    .SingleOrDefaultAsync(x => x.NameOfHost == hostUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}