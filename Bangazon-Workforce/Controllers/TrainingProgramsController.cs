using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon_Workforce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Bangazon_Workforce.Controllers
{
    public class TrainingProgramsController : Controller
    {
        // GET: TrainingPrograms

        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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






        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, StartDate, EndDate, MaxAttendees  FROM TrainingProgram where StartDate > GETDATE()";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };



                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();

                    return View(trainingPrograms);
                }
            }

        }

        public ActionResult PastTrainingPrograms()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, StartDate, EndDate, MaxAttendees  FROM TrainingProgram where StartDate < GETDATE()";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };



                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();

                    return View(trainingPrograms);
                }
            }

        }

        // GET: TrainingPrograms/Details/5
        public ActionResult Details(int id)
        {
            var trainingProgram = GetTrainingProgramById(id);
            return View(trainingProgram);
        }

        public ActionResult PastDetails(int id)
        {
            var trainingProgram = GetTrainingProgramById(id);
            return View(trainingProgram);
        }






        // GET: TrainingPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainingPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @StartDate, @EndDate,  @MaxAttendees)";


                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));



                        int newId = (int)cmd.ExecuteScalar();

                        trainingProgram.Id = newId;
                        return RedirectToAction(nameof(Index));
                    }
                }


            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Edit/5
        public ActionResult Edit(int id)
        {

            var trainingProgram = GetTrainingProgramById(id);
            return View(trainingProgram);
        }

        // POST: TrainingPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = @"Update TrainingProgram
                                        set Name = @Name,
                                        StartDate = @StartDate,
                                        EndDate = @EndDate,
                                        MaxAttendees = @MaxAttendees
                                        where Id = @id";


                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();


                    }
                    return RedirectToAction(nameof(Index));
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: TrainingPrograms/Delete/5
        public ActionResult Delete(int id)
        {
            var trainingProgram = GetTrainingProgramById(id);
            return View(trainingProgram);
        }

        // POST: TrainingPrograms/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));
                    }
                }

            }
            catch
            {
                return View();
            }
        }



        public TrainingProgram GetTrainingProgramById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id, t.Name, t.StartDate, t.EndDate, t.MaxAttendees, e.Id as EmployeeId, e.FirstName, e.LastName
                                        FROM TrainingProgram t LEFT JOIN EmployeeTraining et on et.TrainingProgramId = t.Id
                                        Left Join Employee e on e.Id = et.EmployeeId
                                        WHERE t.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    TrainingProgram trainingProgram = null;

                    while (reader.Read())
                    {
                        if (trainingProgram == null)
                        {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                            Employees = new List<Employee>()                           
                        };
                            trainingProgram.Employees.Add(new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            }
                            );

                        }

                    }
                    reader.Close();

                    return trainingProgram;
                }
            }
        }


    }


    



}






