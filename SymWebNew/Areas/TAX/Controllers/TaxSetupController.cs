using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.TAX.Controllers
{
    public class TaxSetupController : Controller
    {
        TaxSetupRepo _repo = new TaxSetupRepo();
        TaxStructureRepo _repoStructure = new TaxStructureRepo();
        TaxSlabRepo _repoSlab = new TaxSlabRepo();
        #region Actions
        public ActionResult Index()
        {
            return View();
        }
        #endregion Actions


    }
}
