using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public string Email { get; set; }
        public bool IsSupervisor { get; set; }
        public int? ComputerId { get; set; }
        public Computer Computer { get; set; }
        public Department Department { get; set; }
        public List<TrainingProgram> TrainingPrograms { get; set; }
    }
}
