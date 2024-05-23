using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Tax;
using SymRepository.Common;
using SymViewModel.Tax;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class EmloyeeTAXSlabController : Controller
    {
        //
        // GET: /TAX/EmloyeeTAXSlab/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EmloyeeTAXSlabRepo _repo = new EmloyeeTAXSlabRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/TAX/Home");
            }
            return View();
        }

        public ActionResult _index(string codeFrom, string codeTo, string departmentId, string projectId
            , string sectionId, string designationId, string gender, string taxSlabId
            )
        {
            #region Declare Variable
            EmloyeeTAXSlabVM vm = new EmloyeeTAXSlabVM();
            if (codeFrom == "0_0" || codeFrom == "0" || codeFrom == "" || codeFrom == "null" || codeFrom == null)
            {
                codeFrom = "";
            }
            if (codeTo == "0_0" || codeTo == "0" || codeTo == "" || codeTo == "null" || codeTo == null)
            {
                codeTo = "";
            }

            if (projectId == "0_0" || projectId == "0" || projectId == "" || projectId == "null" || projectId == null)
            {
                projectId = "";
            }
            if (departmentId == "0_0" || departmentId == "0" || departmentId == "" || departmentId == "null" || departmentId == null)
            {
                departmentId = "";
            }
            if (sectionId == "0_0" || sectionId == "0" || sectionId == "" || sectionId == "null" || sectionId == null)
            {
                sectionId = "";
            }
            #endregion Declare Variable
            List<EmloyeeTAXSlabVM> VMs = new List<EmloyeeTAXSlabVM>();

            string[] conFields = { "ve.Code>", "ve.Code<", "ve.DepartmentId", "ve.ProjectId", "ve.SectionId", "ve.DesignationId", "ve.Gender", "ets.TaxSlabId" };
            string[] conValues = { codeFrom, codeTo, departmentId, projectId, sectionId, designationId, gender, taxSlabId };

            VMs = _repo.SelectAll(conFields, conValues);
            return PartialView("_index", VMs);
        }
        #region Backup
        //public ActionResult _indexBackup(JQueryDataTableParamModel param)
        //{
        //    //00     //EmployeeId 
        //    //01     //EmployeeCode
        //    //02     //EmployeeName
        //    //03     //Designation
        //    //04     //Department
        //    //05     //Section
        //    //06     //Project
        //    //07     //Gender
        //    //08     //TaxSlabName

        //    #region Search and Filter Data
        //    var getAllData = _repo.SelectAll();
        //    IEnumerable<EmloyeeTAXSlabVM> filteredData;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
        //        var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
        //        var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
        //        var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
        //        var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
        //        var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
        //        var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
        //        var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
        //        filteredData = getAllData.Where(c =>
        //               isSearchable1 && c.EmployeeCode.ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable2 && c.EmployeeName.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable5 && c.Section.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable6 && c.Project.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable7 && c.Gender.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable8 && c.TaxSlabName.ToString().ToLower().Contains(param.sSearch.ToLower())
        //            );
        //    }
        //    else
        //    {
        //        filteredData = getAllData;
        //    }
        //    #endregion Search and Filter Data
        //    var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
        //    var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
        //    var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
        //    var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
        //    var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
        //    var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<EmloyeeTAXSlabVM, string> orderingFunction = (c =>
        //        sortColumnIndex == 1 && isSortable_1 ? c.EmployeeName :
        //        sortColumnIndex == 2 && isSortable_2 ? c.EmployeeName :
        //        sortColumnIndex == 3 && isSortable_3 ? c.Designation :
        //        sortColumnIndex == 4 && isSortable_4 ? c.Department :
        //        sortColumnIndex == 5 && isSortable_5 ? c.Section :
        //        sortColumnIndex == 6 && isSortable_6 ? c.Project :
        //        sortColumnIndex == 7 && isSortable_7 ? c.Gender :
        //        sortColumnIndex == 8 && isSortable_8 ? c.TaxSlabName :
        //        "");
        //    var sortDirection = Request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filteredData = filteredData.OrderBy(orderingFunction);
        //    else
        //        filteredData = filteredData.OrderByDescending(orderingFunction);
        //    var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = from c in displayedCompanies
        //                 select new[] { 
        //          c.EmployeeId
        //        , c.EmployeeCode
        //        , c.EmployeeName
        //        , c.Designation
        //        , c.Department
        //        , c.Section
        //        , c.Project
        //        , c.Gender
        //        , c.TaxSlabName

        //    };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}
        #endregion Backup


        [HttpPost]
        public ActionResult Create(List<EmloyeeTAXSlabVM> VMs, string taxSlabId)
        {
            string[] result = new string[6];
            string mgs = "";
            IEnumerable<EmloyeeTAXSlabVM> updatedVMs = VMs;
            VMs = updatedVMs.Where(c => c.IsEmployeeChecked == true).ToList();

            EmloyeeTAXSlabVM vm = new EmloyeeTAXSlabVM();
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;

            vm.TaxSlabId = taxSlabId;
            result = _repo.Insert(VMs, vm);
            mgs = result[0] + "~" + result[1];
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

    }
}
