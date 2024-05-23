using SymServices.GF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.GF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymRepository.GF
{
    public class GFPolicyRepo
    {
        public List<GFPolicyVM> DropDown(string tType = null, int branchId = 0)
        {
            try
            {
                return new GFPolicyDAL().DropDown(tType, branchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GFPolicyVM> SelectAllByJobYear(int jobYear = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFPolicyDAL().SelectAllByJobYear(jobYear, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GFPolicyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new GFPolicyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(GFPolicyVM vm)
        {
            try
            {
                return new GFPolicyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(GFPolicyVM vm)
        {
            try
            {
                return new GFPolicyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(GFPolicyVM vm, string[] ids)
        {
            try
            {
                return new GFPolicyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
