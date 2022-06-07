using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BuildInspect.Models.Domain;
using BuildInspect.Models.ViewModel;
using AutoMapper;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using System.Globalization;
using BuildInspect.Models.Security;
using System.IO;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class EmployeeController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();

        private readonly IEmployeeServices employeeService;

        public EmployeeController(IEmployeeServices _employeeService)
        {
            employeeService = _employeeService;

        }

        public ActionResult Index()
        {            
            return View(employeeService.GetAllEmployees().Where(a => a.IsActive == 1).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
