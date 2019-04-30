using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InternalForm.Models;
namespace InternalForm.Controllers
{
    public class HomeController : Controller
    {
        private FeedbackAppEntities _DB = new FeedbackAppEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult internalform()
        {
            BindDropDownList();
            return View();
        }

        private void BindDropDownList()
        {


            var employees = _DB.Database.SqlQuery<internalformModel>(@"select emp_namefirst + ' ' + emp_namelast as empFullName, emp_id  from dbo.employee").ToList();


            ViewBag.Employees = new SelectList(employees, "emp_id", "empFullName");

        }
    }
}