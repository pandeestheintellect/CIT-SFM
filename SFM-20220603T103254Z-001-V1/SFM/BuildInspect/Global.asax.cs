using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using BuildInspect.App_Start;
using BuildInspect.Models;
using BuildInspect.Models.Domain;
using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Authentication;
using Ninject;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Repository.Interface;
using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Interface;


namespace BuildInspect
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StartAutoMapper();
            GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationMessageHandler());
        }

        private void StartAutoMapper()
        {
            Mapper.CreateMap<short, bool>().ConvertUsing<BooleanTypeConverter>();
            Mapper.CreateMap<bool, short>().ConvertUsing<ShortTypeConverter>();
            Mapper.CreateMap<int, bool>().ConvertUsing<BooleanIntTypeConverter>();
            Mapper.CreateMap<bool, int>().ConvertUsing<IntTypeConverter>();

           
            Mapper.CreateMap<UserViewModel, user>();
            Mapper.CreateMap<user, UserViewModel>();

            Mapper.CreateMap<EmployeeViewModel, employee>();
            Mapper.CreateMap<employee, EmployeeViewModel>();

            Mapper.CreateMap<module, ModuleViewModel>();
            Mapper.CreateMap<ModuleViewModel, module>();

            Mapper.CreateMap<ScreenViewModel, screen>();
            Mapper.CreateMap<screen, ScreenViewModel>();

            Mapper.CreateMap<usergroup, GroupViewModel>();
            Mapper.CreateMap<GroupViewModel, usergroup>();

        
            Mapper.CreateMap<chklst_item_title_master, CheckListItemTitleMasterViewModel>();
            Mapper.CreateMap<CheckListItemTitleMasterViewModel, chklst_item_title_master>();


            Mapper.CreateMap<company_master, CompanyMasterViewModel>();
            Mapper.CreateMap<CompanyMasterViewModel, company_master>();

            Mapper.CreateMap<building_master, BuildingMasterViewModel>();
            Mapper.CreateMap<BuildingMasterViewModel, building_master>();

            Mapper.CreateMap<chklst_item_master, CheckListItemMasterViewModel>();
            Mapper.CreateMap<CheckListItemMasterViewModel, chklst_item_master>();

            Mapper.CreateMap<project_master, ProjectMasterViewModel>();
            Mapper.CreateMap<ProjectMasterViewModel, project_master>();

            Mapper.CreateMap<chklst_trn_master, CheckListTransMasterViewModel>();
            Mapper.CreateMap<CheckListTransMasterViewModel, chklst_trn_master>();

            Mapper.CreateMap<chklst_trn_detail, CheckListTransDetailViewModel>();
            Mapper.CreateMap<CheckListTransDetailViewModel, chklst_trn_detail>();

            Mapper.CreateMap<servicetype_master, ServiceTypeMasterViewModel>();
            Mapper.CreateMap<ServiceTypeMasterViewModel, servicetype_master>();

            Mapper.CreateMap<service_form, ServiceFormViewModel>();
            Mapper.CreateMap<ServiceFormViewModel, service_form>();

            Mapper.CreateMap<service_form_files, ServiceFormFileViewModel>();
            Mapper.CreateMap<ServiceFormFileViewModel, service_form_files>();

            Mapper.CreateMap<chklst_trn_master, CheckListTransMasterMobileViewModel>();
            Mapper.CreateMap<CheckListTransMasterMobileViewModel, chklst_trn_master>();

            Mapper.CreateMap<ServiceFormMobileViewModel, service_form>();
            Mapper.CreateMap<service_form, ServiceFormMobileViewModel>();

            Mapper.CreateMap<chklst_trn_files, CheckListTransFileViewModel>();
            Mapper.CreateMap<CheckListTransFileViewModel, chklst_trn_files>();

            //Mapper.CreateMap<SchoolLiftoprMappingViewModel, school_liftopr_mapping>();
            //Mapper.CreateMap<school_liftopr_mapping, SchoolLiftoprMappingViewModel>();

            Mapper.CreateMap<checklist_files, ChecklistFileViewModel>();
            Mapper.CreateMap<ChecklistFileViewModel, checklist_files>();

            Mapper.CreateMap<school_user_mapping, SchoolSpecialistMappingViewModel>();
            Mapper.CreateMap<SchoolSpecialistMappingViewModel, school_user_mapping>();

            Mapper.CreateMap<License_Master, LicenseViewModel>();
            Mapper.CreateMap<LicenseViewModel, License_Master>();


            Mapper.CreateMap<servicetype_master, CheckListTypeMasterViewModel>();
            Mapper.CreateMap<CheckListTypeMasterViewModel, servicetype_master>();

            Mapper.CreateMap<usergroup, GroupViewModel>();
            Mapper.CreateMap<GroupViewModel, usergroup> ();

            
        }
        private class BooleanTypeConverter : TypeConverter<short, bool>
        {
            protected override bool ConvertCore(short source)
            {
                if (source == 1)
                    return true;
                return false;
            }
        }

        private class BooleanIntTypeConverter : TypeConverter<int, bool>
        {
            protected override bool ConvertCore(int source)
            {
                if (source == 1)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Converts bool to short
        /// </summary>
        private class ShortTypeConverter : TypeConverter<bool, short>
        {
            protected override short ConvertCore(bool source)
            {
                if (source)
                    return 1;
                return 0;
            }
        }

        private class IntTypeConverter : TypeConverter<bool, int>
        {
            protected override int ConvertCore(bool source)
            {
                if (source)
                    return 1;
                return 0;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                logger.Error("Error:");
                logger.Error(exception.Message);
                logger.Error(exception.StackTrace);

                int i = 0;
            }
        }

        protected void Session_Start()
        {

        }
        protected void Session_End()
        {
            Session.Abandon();
        }
    }
}
