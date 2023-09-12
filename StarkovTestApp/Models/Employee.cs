using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarkovTestApp.Models
{
    internal class Employee
    {
        [Key]
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int JobTittleID { get; set; }
        public string DepartmentName { get; set; }
        public string JobTittleName { get; set; } 
            = "";
    }
}
