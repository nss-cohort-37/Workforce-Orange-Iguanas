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
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET: Computers
        public ActionResult Index(string searchString)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Make, Model, PurchaseDate
                                        FROM Computer ";

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        cmd.CommandText += @"WHERE Make LIKE @searchString OR Model LIKE @searchString";
                        cmd.Parameters.Add(new SqlParameter("@searchString", "%" + searchString + "%"));
                    }

                    var reader = cmd.ExecuteReader();
                    var computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        });
                    }
                    reader.Close();
                    return View(computers);
                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            var getComputerById = GetComputerById(id);
            return View(getComputerById);
        }

        // GET: Computers/Create
        public ActionResult Create()
        {
            var employeeOptions = GetEmployeeOptions();
            var viewModel = new ComputerCreateViewModel()
            {
                PurchaseDate = DateTime.Now,
                EmployeeOptions = employeeOptions
            };
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel computer)
        {
            try
            {
                // TODO: Add insert logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT
                                            INTO Computer (PurchaseDate, Make, Model)
                                            OUTPUT INSERTED.Id
                                            VALUES (@purchaseDate, @make, @model)";

                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@model", computer.Model));

                        var Id = (int)cmd.ExecuteScalar();
                        computer.Id = Id;

                        if (computer.EmployeeId != 0)
                        {
                            cmd.CommandText = @"UPDATE Employee
                                               SET ComputerID = @computerId
                                               WHERE Id = @employeeId";

                            cmd.Parameters.Add(new SqlParameter("@computerId", computer.Id));
                            cmd.Parameters.Add(new SqlParameter("@employeeId", computer.EmployeeId));

                            var rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected < 1)
                            {
                                return NotFound();
                            }
                        }

                        return RedirectToAction(nameof(Index));

                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Computers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"Select c.Id, c.Make, c.Model, c.PurchaseDate, COALESCE(e.FirstName, 'n/a') AS FirstName, COALESCE(e.LastName, 'n/a') AS LastName, COALESCE(e.Id, 0) AS EmployeeId
                                            FROM computer c
                                            LEFT JOIN employee e 
                                            on e.ComputerId = c.Id
                                            WHERE c.Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var reader = cmd.ExecuteReader();
                        DeleteComputerViewModel computer = null;

                        if (reader.Read())
                        {
                            computer = new DeleteComputerViewModel()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Model = reader.GetString(reader.GetOrdinal("Model")),
                                Employee = new Employee()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))

                                }
                            };
                        }

                        return View(computer);
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, DeleteComputerViewModel computer)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE
                                            FROM Computer
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            return NotFound();
                        }

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public List<Computer> GetListOfComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Make, Model, PurchaseDate
                                        FROM Computer";

                    var reader = cmd.ExecuteReader();
                    var computers = new List<Computer>();

                    while (reader.Read())
                    {
                        computers.Add(new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate"))
                        });
                    }
                    reader.Close();
                    return computers;
                }
            }
        }

        public Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, PurchaseDate, DecomissionDate, Make, Model
                                        FROM Computer
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Computer computer = null;

                    if (reader.Read())
                    {
                        computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model"))
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                    {
                        computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                    }

                    reader.Close();
                    return computer;
                }
            }
        }

        private List<SelectListItem> GetEmployeeOptions()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, CONCAT(FirstName, ' ', LastName) AS EmployeeName
                                        FROM Employee";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("EmployeeName")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }
    }
}