using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_Workforce.Models;
using Bangazon_Workforce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon_Workforce.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public ActionResult Assign(int id)
        {
            var trainingProgramOptions = GetAllTrainingPrograms(id);
            var CurrentEntrolledTriainingPrograms = GetCurrentEmployeesTrainingPrograms(id);
            var employee = GetEmployeeById(id);
            var viewModel = new AssignToProgramViewModel()
            {
                EnrolledTrainingPrograms = CurrentEntrolledTriainingPrograms,
                TrainingPrograms = trainingProgramOptions,
                EmployeeId = employee.Id,
                EmployeeFirstName = employee.FirstName,
                EmployeeLastName = employee.LastName

            };


            return View(viewModel);
        }

        public ActionResult Assign(List<AssignToProgramViewModel> employeeTrainingList)
        {

            foreach(var employeeTraining in employeeTrainingList) { 

            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@EmployeeId, @TrainingProgramId)";

                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", employeeTraining.EmployeeId));
                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", employeeTraining.TrainingProgramId));
                        



                        var id = (int)cmd.ExecuteScalar();
                        employeeTraining.EmployeeId = id;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                return View(employeeTraining);
            }
            }





            return RedirectToAction(nameof(Index));
        }



        // GET: Employees
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.Name FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId";

                    var reader = cmd.ExecuteReader();
                    var employees = new List<Employee>();

                    while (reader.Read())
                    {
                        var employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department ()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        // GET: Employees/Details/5
        public ActionResult Details(int id)
        {
            var employee = GetEmployeeById(id);
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var departmentOptions = GetDepartmentOptions();
            var computerOptions = GetComputerOptions();
            var viewModel = new EmployeeCreateViewModel()
            {
                DepartmentOptions = departmentOptions,
                ComputerOptions = computerOptions
            };
            return View(viewModel);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeCreateViewModel employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, Email, IsSupervisor, DepartmentId, ComputerId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @email, @isSupervisor, @departmentId, @computerId)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));



                        var id = (int)cmd.ExecuteScalar();
                        employee.EmployeeId = id;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                return View(employee);
            }
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var employee = GetEmployeeById(id);
            var DepartmentOptions = GetDepartmentOptions();
            var ComputerOptions = GetComputerOptions();
            var viewModel = new EmployeeEditViewModel()
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DepartmentOptions = DepartmentOptions,
                ComputerOptions = ComputerOptions
            };
            return View(viewModel);
           
        }

        // POST: Employees/Edit/Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee 
                                            SET LastName = @lastName, 
                                                ComputerId = @computerId,
                                                DepartmentId = @departmentId
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@computerId", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            return NotFound();
                        }
                    }
                }


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private List<SelectListItem> GetDepartmentOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Name")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

        private List<SelectListItem> GetComputerOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, CONCAT(c.Make, ' ', c.Model) AS ComputerInfo 
                                        FROM Computer c
                                        LEFT JOIN Employee e on e.ComputerId = c.Id
                                        WHERE c.DecomissionDate IS NULL 
                                        AND c.Id NOT IN (SELECT ComputerId FROM Employee)";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("ComputerInfo")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.Name AS DepartmentName, CONCAT( c.Make, ' ', c.Model) AS ComputerInfo, COALESCE(tp.Name, 'N/A') AS TrainingProgram 
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId 
                                        LEFT JOIN Computer c ON c.Id = e.ComputerId
                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    while (reader.Read())
                    {
                        if (employee == null)
                        {


                            employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Department = new Department()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                                },
                                Computer = new Computer()
                                {
                                    Make = reader.GetString(reader.GetOrdinal("ComputerInfo"))
                                },
                                TrainingPrograms = new List<TrainingProgram>()
                            };
                        }
                        employee.TrainingPrograms.Add(new TrainingProgram(){
                            Name = reader.GetString(reader.GetOrdinal("TrainingProgram"))
                        });
                    }
                    reader.Close();
                    return employee;
                }
            }
        }
        private MultiSelectList GetAllTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM TrainingProgram 
                                        group by Name,StartDate, MaxAttendees, Id
                                        having Id NOT IN (SELECT TrainingProgramId FROM EmployeeTraining WHERE EmployeeTraining.EmployeeId = @id)
                                        and StartDate > GETDATE() and MaxAttendees > COUNT(Id) ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    var trainingPrograms = new List<SelectListItem>();



                    while (reader.Read())
                    {
                       
                       
                           var trainingProgram = new SelectListItem()
                            {

                                Text = reader.GetString(reader.GetOrdinal("Name")),
                                Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString(),


                            };

                        
                        trainingPrograms.Add(trainingProgram);
                         var ThetrainingPrograms = new MultiSelectList(trainingPrograms, "Text", "Value");

                       

                    
                        
                    }
                    var MultiSelectTrainingPrograms = new MultiSelectList(trainingPrograms, "Value", "Text");
                    reader.Close();

                    return MultiSelectTrainingPrograms;
                }
            }
        }


        public List<TrainingProgram> GetCurrentEmployeesTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select tp.Id, tp.Name from TrainingProgram tp
                                        left join EmployeeTraining et on tp.Id = et.TrainingProgramId
                                        left join Employee e on e.Id = et.EmployeeId
                                        group by tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees, e.Id
                                        having e.Id = @id and tp.StartDate > GETDATE() and tp.MaxAttendees > COUNT(e.Id)";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    var trainingPrograms = new List<TrainingProgram>();



                    while (reader.Read())
                    {


                        var trainingProgram = new TrainingProgram()
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };


                        trainingPrograms.Add(trainingProgram);


                    }
                   
                    reader.Close();

                    return trainingPrograms;
                }
            }
        }




    }
}