using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.HRM.Controllers
{
    public class SetUpController : Controller
    {
        //
        // GET: /HRM/SetUp/
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SetUpRepo _repo = new SetUpRepo();

         [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }


         public ActionResult _index()
         {
             string[] result = new string[6];
             try
             {
                 Events vm = new Events();


                 List<Events> data = _repo.SelectAll();

                 return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
             }
             catch (Exception)
             {
                 Session["result"] = "Fail~Data Not Succeessfully!";
                 FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                 return RedirectToAction("Index");
             }
         
         }



        public ActionResult Emp()
         {
             return View();
         }


         public ActionResult EmpHierarchy()
         {
             string[] result = new string[6];
             try
             {
                 EmployeesHierarchyVM vm = new EmployeesHierarchyVM();


                 List<EmployeesHierarchyVM> data = _repo.SelectAllEmp();

                 List<object> chartData = new List<object>();

                 foreach (var obj in data)
                 {
                     //Console.WriteLine(obj.Name + obj.Age);

                     chartData.Add(new object[]
                            {
                                obj.EmployeeId.ToString(),obj.Name,obj.Designation,obj.ReportingManager
                      //      sdr["EmployeeId"], sdr["Name"], sdr["Designation"] , sdr["ReportingManager"]
                            });

                 }
                 return Json(chartData);


                 //return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
             }
             catch (Exception)
             {
                 Session["result"] = "Fail~Data Not Succeessfully!";
                 FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                 return RedirectToAction("Index");
             }

         }

    }
}
