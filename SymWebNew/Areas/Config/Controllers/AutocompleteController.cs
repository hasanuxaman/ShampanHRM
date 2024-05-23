using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;
using SymRepository.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Config.Controllers
{
    public class AutocompleteController : Controller
    {
        //

        // GET: /Enum/DropDown/
        

        #region HRPayroll / Others
        
        public JsonResult OtherInfo(string term, string infoType = "")
        {
            return Json(new OtherInfoRepo().Autocomplete(term, infoType), JsonRequestBehavior.AllowGet);
        }


        //Acc
      
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Employee(string term)
        {
            return Json(new EmployeeInfoRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult EmployeeCode(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteCode(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeCodeAll(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteCodeAll(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult EmployeeMarge(string term)
        {
            return Json(new EmployeeInfoRepo().AutocompleteMarge(term), JsonRequestBehavior.AllowGet);
        }
     

        public JsonResult BloodGroup(string term)
        {
            return Json(new EnumBloodGroupRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Department(string term)
        {
            return Json(new DepartmentRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult Section(string term)
        {
            return Json(new SectionRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Designation(string term)
        {
            return Json(new DesignationRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
       
        //public JsonResult Project(string term)
        //{
        //    return Json(new ProjectRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult Country(string term)
        {
            return Json(new EnumCountryRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Division(string term, string country)
        {
            return Json(new EnumDivisionRepo().Autocomplete(term, country), JsonRequestBehavior.AllowGet);
        }
        public JsonResult District(string term, string country, string division)
        {
            return Json(new EnumDistrictRepo().Autocomplete(term, country, division), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Degree(string term)
        {
            return Json(new EnumDegreeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult EmploymentStatus(string term)
        {
            return Json(new EnumEmploymentStatusRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult EmploymentType(string term)
        {
            return Json(new EnumEmploymentTypeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Gender(string term)
        {
            return Json(new EnumGenderRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Language(string term)
        {
            return Json(new EnumLanguageRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult LeaveApproveStatus(string term)
        {
            return Json(new EnumLeaveApproveStatusRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult LeaveType(string term)
        {
            return Json(new EnumLeaveTypeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult LeftType(string term)
        {
            return Json(new EnumLeftTypeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Supervisor(string term)
        {
            return Json(new EmployeeInfoRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MeritalStatus(string term)
        {
            return Json(new EnumMeritalStatusRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Religion(string term)
        {
            return Json(new EnumReligionRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult SalaryPayMode(string term)
        {
            return Json(new EnumSalaryPayModeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Salutation(string term)
        {
            return Json(new EnumSalutationRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult TrainingStatus(string term)
        {
            return Json(new EnumTrainingStatusRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);

        }


        public JsonResult TravelType(string term)
        {
            return Json(new EnumTravelTypeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Year(string term)
        {
            return Json(new EnumYearRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ImmigrationType(string term)
        {
            return Json(new EnumImmigrationTypeRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult LanguageCompetency(string term)
        {
            return Json(new EnumLanguageCompetencyRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult LanguageFluency(string term)
        {
            return Json(new EnumLanguageFluencyRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }


        public JsonResult Nationality(string term)
        {
            return Json(new EnumNationalityRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        public JsonResult TrainingPlace(string term)
        {
            return Json(new EnumTrainingPlaceRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DepermentsByProject(string term)
        {
            return Json(new SymRepository.Common.DepartmentRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

       
        #endregion
    }
}
