using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;

namespace SymRepository.Payroll
{
   public class BonusNameRepo

    {
       BonusNameDAL _dal = new BonusNameDAL();

       #region Methods

       public List<BonusNameVM> DropDown(int BranchId)
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
        public List<BonusNameVM> SelectAll()
        {
            try
            {
                return new BonusNameDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public BonusNameVM SelectById(string Id)
        {
            try
            {
                return new BonusNameDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(BonusNameVM vm)
        {
            try
            {
                return new BonusNameDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Update =================
        public string[] Update(BonusNameVM vm)
        {
            try
            {
                return new BonusNameDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(BonusNameVM vm, string[] ids)
        {
            try
            {
                return new BonusNameDAL().Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //==================Bonus Process=================
        public string[] BonusProcess(int BonusTypeId, string BonusStructureId, string ProcessDate)
        {
            try
            {
                return new BonusNameDAL().BonusProcess(BonusTypeId, BonusStructureId, ProcessDate, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
