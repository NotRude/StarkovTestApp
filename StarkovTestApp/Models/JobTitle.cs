using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkovTestApp.Models
{
    internal class JobTitle
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
