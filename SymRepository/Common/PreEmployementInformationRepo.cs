using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class PreEmployementInformationRepo
    {
        public List<PreEmployementInformationVM> DropDownEmpName()
        {
            try
            {
                return new PreEmployementInformationDAL().DropDownEmpName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PreEmployementInformationVM> DropDownRef()
        {
            try
            {
                return new PreEmployementInformationDAL().DropDownRef();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PreEmployementInformationVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PreEmployementInformationDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(PreEmployementInformationVM vm)
        {
            try
            {
                return new PreEmployementInformationDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(PreEmployementInformationVM vm)
        {
            try
            {
                return new PreEmployementInformationDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] Delete(PreEmployementInformationVM vm, string[] ids)
        {
            try
            {
                return new PreEmployementInformationDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(PreEmployementInformationVM vm, string[] ids, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PreEmployementInformationDAL().Report(vm, ids, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
