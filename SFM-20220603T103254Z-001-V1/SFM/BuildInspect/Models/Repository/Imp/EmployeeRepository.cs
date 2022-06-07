using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildInspect.Models.Domain;
using AutoMapper;
using System.Data.Entity;
using BuildInspect.Models.Utility;
using System.Data.Entity.Infrastructure;
using System.IO;


namespace BuildInspect.Models.Repository.Interface
{
  
    public class EmployeeRepository : IEmployeeRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();



        public List<GroupViewModel> getUsergroups()
        {
            var usergroup = BInDB.usergroups.ToList();
            var lstGroupView = Mapper.Map<List<GroupViewModel>>(usergroup);
            return lstGroupView;
        }

        public List<EmployeeViewModel> GetAllEmployees()
        {
            var emp = BInDB.employees.OrderBy(a => a.FirstName).ToList();
            var lstEmployeeView = Mapper.Map<List<EmployeeViewModel>>(emp);

            foreach (var emps in lstEmployeeView)
            {
                emps.FullName = emps.FirstName + ' ' + emps.LastName;

            }
            return lstEmployeeView;
        }

    }
}