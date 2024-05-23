using SymServices.Common;
using SymServices.Tax;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class TaxStructureRepo
    {
        TaxStructureDAL _dal = new TaxStructureDAL();
        #region Methods
        public List<TaxStructureVM> DropDown(int branch)
        {
            try
            {
                return _dal.DropDown(branch);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<TaxStructureVM> SelectAll()
        {
            try
            {
                return new TaxStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<TaxStructureVM> SelectTaxStructureMasterId(string Id)
        {
            try
            {
                return new TaxStructureDAL().SelectTaxStructureMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public List<TaxStructureVM> SelectByMasterId(string Id)
        {
            try
            {
                return new TaxStructureDAL().SelectByMasterId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public TaxStructureVM SelectById(string Id)
        {
            try
            {
                return new TaxStructureDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(TaxStructureVM vm)
        {
            try
            {
                return new TaxStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(TaxStructureVM vm)
        {
            try
            {
                return new TaxStructureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(TaxStructureVM vm, string[] Ids)
        {
            try
            {
                return new TaxStructureDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
