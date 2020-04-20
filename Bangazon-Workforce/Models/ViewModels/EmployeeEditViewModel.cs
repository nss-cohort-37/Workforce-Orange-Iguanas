using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Display(Name = "Computer")]
        [Required]
        public int? ComputerId { get; set; }
        public List<SelectListItem> ComputerOptions { get; set; }
        [Display(Name = "Department")]
        [Required]
        public int DepartmentId { get; set; }
        public List<SelectListItem> DepartmentOptions { get; set; }
    }
}
