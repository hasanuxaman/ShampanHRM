using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class OtherInfoRepo
    {
        public List<string> Autocomplete(string term, string infoType = "")
        {
            try
            {
                return new OtherInfoDAL().Autocomplete(term, infoType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> VaccineInfo(string term, string infoType = "")
        {
            try
            {
                return new OtherInfoDAL().VaccineAutocomplete(term, infoType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OtherInfoVM> DropDown(string infoType = "")
        {
            try
            {
                return new OtherInfoDAL().DropDown(infoType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OtherInfoVM> EMPTypeList(string infoType = "")
        {
            try
            {
                return new OtherInfoDAL().EMPTypeList(infoType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OtherInfoVM> EmpCategoryList()
        {
            try
            {
                return new OtherInfoDAL().EmpCategoryList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public List<OtherInfoVM> SelectAll(string Id = "", string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new OtherInfoDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(OtherInfoVM vm)
        {
            try
            {
                return new OtherInfoDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(OtherInfoVM vm)
        {
            try
            {
                return new OtherInfoDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(OtherInfoVM vm, string[] ids)
        {
            try
            {
                return new OtherInfoDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
