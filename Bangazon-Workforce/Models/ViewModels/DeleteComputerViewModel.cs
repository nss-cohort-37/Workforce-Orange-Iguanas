using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models.ViewModels
{
    public class DeleteComputerViewModel
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public Employee Employee { get; set; }
    }
}
