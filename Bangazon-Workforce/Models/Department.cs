using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        [Display(Name="Employees Name")]
        public List<Employee> Employees { get; set; }
        [Display(Name = "Number of Employees")]
        public int EmployeeCount { get; set; }
    }
}
