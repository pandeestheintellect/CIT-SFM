using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FirstProject.Models;
//using FirstProject.Models.Respository;
using FirstProject.Models.ViewModels;

namespace FirstProject.Controllers
{
    public class HomeController : Controller
    {
        sampleEntities db = new sampleEntities();
        public ActionResult Index()
        {
            IList<EmployeeViewModel> employee = new List<EmployeeViewModel>();
            var employeelist = from emp in db.Employees select emp;
            var query = employeelist.ToList();
            foreach (var em in query)
            {
                employee.Add(new EmployeeViewModel()
                {
                    empid = em.empid,
                    EmpName = em.EmpName,
                    phoneno = em.phoneno,
                    email = em.email
                });
            }
            return View(employee);
        }

        public ActionResult About()
        {
            Employee employee = db.Employees.SingleOrDefault(a => a.empid == 1);
            EmployeeViewModel empvm = new EmployeeViewModel();
            empvm.empid = employee.empid;
            empvm.EmpName = employee.EmpName;
            empvm.phoneno = employee.phoneno;
            empvm.email = employee.email;
            return View(empvm);

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}