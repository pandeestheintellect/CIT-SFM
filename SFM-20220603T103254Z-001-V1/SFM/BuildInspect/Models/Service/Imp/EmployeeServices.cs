using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly IEmployeeRepository empRepository;
        public EmployeeServices(IEmployeeRepository _empRepository)
        {
            empRepository = _empRepository;
        }

       
        public List<GroupViewModel> getUsergroups()
        {
            return empRepository.getUsergroups();
        }
        public List<EmployeeViewModel> GetAllEmployees()
        {
            return empRepository.GetAllEmployees();
        }



    }
}