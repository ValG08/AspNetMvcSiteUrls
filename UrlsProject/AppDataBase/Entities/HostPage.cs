using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UrlsProject.AppDataBase.Entities
{
    public class HostPage : BaseEntity
    {
        public int MinResponse { get; set; }
        public int MaxResponse { get; set; }
        public string Url { get; set; }
                
        public int HostId { get; set; }
        [ForeignKey(nameof(HostId))]
        public virtual Host Host { get; set; }

        public virtual ICollection<HostHistory> HostHistories { get; set; } = new List<HostHistory>();
    }
}