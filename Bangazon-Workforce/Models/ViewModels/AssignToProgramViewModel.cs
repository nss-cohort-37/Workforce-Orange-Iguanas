using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon_Workforce.Models.ViewModels
{
    public class AssignToProgramViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public int TrainingProgramId { get; set; }
        public List<TrainingProgram> EnrolledTrainingPrograms { get; set; }
        public List<SelectListItem> TrainingPrograms {get;set;}
        public List<int> TrainingProgramIds { get; set; }
    }
}
