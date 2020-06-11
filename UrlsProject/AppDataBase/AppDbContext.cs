using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UrlsProject.AppDataBase.Entities;

namespace UrlsProject.AppDataBase
{
    public class AppDbContext : DbContext
    {       
        public AppDbContext()
            : base("SitesUrlsDb")
        {
            
        }

        public DbSet<Host> Hosts { get; set; }
        public DbSet<HostPage> HostPages { get; set; }
        public DbSet<HostHistory> HostHistories { get; set; }
    }
}