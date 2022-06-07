using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Repository.Imp
{
    public interface IEmployeeRepository
    {

        List<GroupViewModel> getUsergroups();
        List<EmployeeViewModel> GetAllEmployees();
    }
}