using SymServices.Enum;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
   public class EnumDistrictRepo
    {
        EnumDistrictDAL _enumDAL = new EnumDistrictDAL();
        #region Method

        public List<EnumDistrictVM> DropDown()
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
        public List<EnumDistrictVM> DropDown(string division)
        {
            try
            {
                return _enumDAL.DropDown(division);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term, string country, string division)
        {
            try
            {
                return _enumDAL.Autocomplete(term, country, division);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EnumDistrictVM> SelectAll()
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
        public EnumDistrictVM SelectById(int Id)
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
        public string[] Insert(EnumDistrictVM vm)
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
        public string[] Update(EnumDistrictVM vm)
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
        public string[] Delete(EnumDistrictVM vm, string[] ids)
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
