using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
     
        [Display(Name = "Supervisor")]
        [Required]
        public bool IsSupervisor { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public List<SelectListItem> DepartmentOptions { get; set; }

    }
}
