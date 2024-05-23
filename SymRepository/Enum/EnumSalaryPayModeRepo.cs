using SymServices.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
  public  class EnumSalaryPayModeRepo
    {
        EnumSalaryPayModeDAL _enumDAL = new EnumSalaryPayModeDAL();
        #region Method

        public List<EnumSalaryPayModeVM> DropDown()
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
        public List<EnumSalaryPayModeVM> SelectAll()
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
        public EnumSalaryPayModeVM SelectById(int Id)
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
        public string[] Insert(EnumSalaryPayModeVM vm)
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
        public string[] Update(EnumSalaryPayModeVM vm)
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
        public string[] Delete(EnumSalaryPayModeVM vm, string[] ids)
        {
            try
            {
                return _enumDAL.Delete(vm,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
