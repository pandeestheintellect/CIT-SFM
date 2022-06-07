//using FirstProject.Models.ViewModels;
//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;

//namespace FirstProject.Models.Respository
//{
//    public class EmployeeRepository:IEmployeeRepository
//    {
//        private readonly sampleEntities _dbContext;

//        public EmployeeRepository()
//        {
//            _dbContext = new sampleEntities();
//        }
//        public EmployeeRepository(sampleEntities context)
//        {
//            _dbContext = context;
//        }

//        public List<EmployeeViewModel> GetEmployees()
//        {
//            return _dbContext.Employees.ToList();
//        }

//    }
//}