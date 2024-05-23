using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
  public  class EnumLoanTypeRepo
    {
        EnumLoanTypeDAL _enumDAL = new EnumLoanTypeDAL();
        #region Method

        public List<EnumLoanTypeVM> DropDown()
        {
            try
            {
                return _enumDAL.DropDown();
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
                return _enumDAL.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumLoanTypeVM> SelectAll()
        {
            try
            {
                return _enumDAL.SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EnumLoanTypeVM SelectById(int Id)
        {
            try
            {
                return _enumDAL.SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EnumLoanTypeVM vm)
        {
            try
            {
                return _enumDAL.Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(EnumLoanTypeVM vm)
        {
            try
            {
                return _enumDAL.Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] EditLoan(string Id, string GLAccountCode)
        {
            string[] results = new string[6];
            try
            {
                results = _enumDAL.EditLoan(Id, GLAccountCode, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }

        public string[] Delete(EnumLoanTypeVM vm, string[] ids)
        {
            try
            {
                return _enumDAL.Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
