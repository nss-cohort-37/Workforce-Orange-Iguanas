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
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
     
        [Display(Name = "Supervisor")]
        [Required]
        public bool IsSupervisor { get; set; }
        [Display(Name = "Department")]
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public List<SelectListItem> DepartmentOptions { get; set; }
        [Display(Name = "Computer")]
        [Required]
        public int ComputerId { get; set; }
        [Required]
        public List<SelectListItem> ComputerOptions { get; set; }

    }
}
