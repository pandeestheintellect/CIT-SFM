using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Imp
{
    public interface IEmployeeServices
    {
        List<GroupViewModel> getUsergroups();
        List<EmployeeViewModel> GetAllEmployees();
    }
}