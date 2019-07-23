using DemoGrids.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DemoGrids.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCustomers(string sidx,string sord, int page, int rows, string searchString)
        {
            try
            {
                // Create Instance of DatabaseContext class for Accessing Database.
                DatabaseContext db = new DatabaseContext();

                //Setting Paging
                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;
                var Results = db.Customers.Select(
                    a => new
                    {
                        a.CustomerID,
                        a.CompanyName,
                        a.ContactName,
                        a.ContactTitle,
                        a.City,
                        a.PostalCode,
                        a.Country,
                        a.Phone,
                    });

                //Get Total Row Count
                int totalRecords = Results.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                //Setting Sorting
                if (sord.ToUpper() == "DESC")
                {
                    Results = Results.OrderByDescending(s => s.CustomerID);
                    Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
                }
                else
                {
                    Results = Results.OrderBy(s => s.CustomerID);
                    Results = Results.Skip(pageIndex * pageSize).Take(pageSize);
                }
                //Setting Search
                if (!string.IsNullOrEmpty(searchString))
                {
                    Results = Results.Where(m => m.CompanyName == searchString || m.ContactName == searchString);
                }
                //Sending Json Object to View.
                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = Results
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new JsonResult();
            }
        }

        [HttpPost]
        public JsonResult CreateCustomer([Bind(Exclude = "CustomerID")] Customers customers)
        {
            StringBuilder msg = new StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        db.Customers.Add(customers);
                        db.SaveChanges();
                        return Json("Saved Successfully", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();

                    return Json(errorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var errormessage = "Error occured: " + ex.Message;
                return Json(errormessage, JsonRequestBehavior.AllowGet);
            }

        }

        public string EditCustomer(Customers customers)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        db.Entry(customers).State = EntityState.Modified;
                        db.SaveChanges();
                        msg = "Saved Successfully";
                    }
                }
                else
                {
                    msg = "Some Validation ";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        public string DeleteCustomer(int Id)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                Customers customer = db.Customers.Find(Id);
                db.Customers.Remove(customer);
                db.SaveChanges();
                return "Deleted successfully";
            }
        }
    }
}