using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkovTestApp.Models
{
    internal class Department
    {
        [Key]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int ManagerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ParentName { get; set; }
        public string ManagerName { get; set; }
        [NotMapped]
        public List<Department> SubDepartments { get; set; } 
            = new List<Department>();
    }
}
