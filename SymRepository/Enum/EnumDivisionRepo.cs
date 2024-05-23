using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
  public  class EnumDivisionRepo
    {
        EnumDivisionDAL _enumDAL = new EnumDivisionDAL();
        #region Method

        public List<EnumDivisionVM> DropDown()
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
        public List<EnumDivisionVM> DropDown(string country)
        {
            try
            {
                return _enumDAL.DropDown(country);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term, string country)
        {
            try
            {
                return _enumDAL.Autocomplete(term, country);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumDivisionVM> SelectAll()
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
        public EnumDivisionVM SelectById(int Id)
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
        public string[] Insert(EnumDivisionVM vm)
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
        public string[] Update(EnumDivisionVM vm)
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
        public string[] Delete(EnumDivisionVM vm, string[] ids)
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
