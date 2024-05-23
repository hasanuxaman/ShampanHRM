using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class DatabaseTableController : Controller
    {

        // GET: /Common/DatabaseTable/

        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        DatabaseTableRepo _repo = new DatabaseTableRepo();

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            DatabaseTableVM vm = new DatabaseTableVM();
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Update(DatabaseTableVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region UserId, Password Matching
                string UserId = "symphony";
                if (vm.UserId.ToLower() != UserId.ToLower())
                {
                    Session["result"] = "Fail" + "~" + "User Id Not Matched!";
                    return View("Create", vm);
                }
                string Password = Ordinary.Encrypt("01730047765", true);
                string GivenPassword = Ordinary.Encrypt(vm.Password, true);

                if (GivenPassword != Password)
                {
                    Session["result"] = "Fail" + "~" + "Password Not Matched!";
                    return View("Create", vm);
                }
                #endregion

                result = _repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return View("Create", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DownloadData(DatabaseTableVM vm)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                #region UserId, Password Matching
                string UserId = "symphony";
                if (vm.UserId.ToLower() != UserId.ToLower())
                {
                    Session["result"] = "Fail" + "~" + "User Id Not Matched!";
                    return View("Create", vm);
                }
                string Password = Ordinary.Encrypt("01730047765", true);
                string GivenPassword = Ordinary.Encrypt(vm.Password, true);

                if (GivenPassword != Password)
                {
                    Session["result"] = "Fail" + "~" + "Password Not Matched!";
                    return View("Create", vm);
                }
                #endregion


                dt = _repo.SelectAllData(vm);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "DownloadData";

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
               return View("Create", vm);
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

    }
}
