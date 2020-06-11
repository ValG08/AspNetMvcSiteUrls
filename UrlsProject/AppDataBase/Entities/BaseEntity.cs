using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UrlsProject.AppDataBase.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}