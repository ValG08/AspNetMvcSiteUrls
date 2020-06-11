using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UrlsProject.AppDataBase.Entities
{
    public class Host : BaseEntity
    {
        public string NameOfHost { get; set; }

        public virtual ICollection<HostPage> HostPages { get; set; } = new List<HostPage>();
    }
}