using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
  public class TaxSetupRepo
    {
        TaxSetupDAL _dal = new TaxSetupDAL();
        #region Methods
        public List<TaxSetupVM> DropDown(int branch)
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
        public List<TaxSetupVM> SelectAll()
        {
            try
            {
                return new TaxSetupDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        //==================SelectByID=================
        public TaxSetupVM SelectById(string Id)
        {
            try
            {
                return new TaxSetupDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(TaxSetupVM vm,int BranchId)
        {
            try
            {
                return new TaxSetupDAL().Insert(vm,BranchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(TaxSetupVM vm,int BranchId)
        {
            try
            {
                return new TaxSetupDAL().Update(vm,BranchId, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(TaxSetupVM vm, string[] Ids)
        {
            try
            {
                return new TaxSetupDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
