using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UrlsProject.AppDataBase.Entities
{
    public class HostHistory : BaseEntity
    {        
        public DateTime Date { get; set; }
        public int ResponseTime { get; set; }

        public int PageId { get; set; }
        [ForeignKey(nameof(PageId))]
        public virtual HostPage Page { get; set; }
    }
}