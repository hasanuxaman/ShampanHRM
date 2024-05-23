using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class BonusStructureRepo
    {
        BonusStructureDAL _dal = new BonusStructureDAL();
        #region Methods
        public List<BonusStructureVM> DropDown(int BranchId)
        {
            try
            {
                return _dal.DropDown(BranchId);
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
        public List<BonusStructureVM> SelectAll()
        {
            try
            {
                return new BonusStructureDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public BonusStructureVM SelectById(string Id)
        {
            try
            {
                return new BonusStructureDAL().SelectAll(Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(BonusStructureVM vm)
        {
            try
            {
                return new BonusStructureDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(BonusStructureVM vm)
        {
            try
            {
                return new BonusStructureDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(BonusStructureVM vm, string[] Ids)
        {
            try
            {
                return new BonusStructureDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
