using SymOrdinary;
using SymRepository.Common;
using SymRepository.HRM;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Config.Controllers
{
    public class DataMigrationController : Controller
    {
        //
        // GET: /Config/DataMigration/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DataMigration(string structureType)
        {
            string[] result = new string[6];
            
            ShampanIdentityVM siVM = new ShampanIdentityVM();

            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                siVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                siVM.LastUpdateBy = identity.Name;
                siVM.LastUpdateFrom = identity.WorkStationIP;
                siVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                siVM.CreatedBy = identity.Name;
                siVM.CreatedFrom = identity.WorkStationIP;
                siVM.BranchId = identity.BranchId;
                DataMigrationRepo _dmRepo = new DataMigrationRepo();
                if (structureType.ToLower().Replace(" ","") == "leaveposting")
                {
                    result = _dmRepo.OpeningLeavePost(siVM);
                }
                else
                {
                    result = _dmRepo.Insert(structureType, siVM);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Session["result"] = result[0] + "~" + result[1];
            return RedirectToAction("Index");
        }

    }
}
