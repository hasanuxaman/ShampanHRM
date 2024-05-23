using SymServices.PF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class InvestmentRepo
    {
        

        public List<InvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       


        public string[] Insert(InvestmentVM vm)
        {
            try
            {
                return new InvestmentDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(InvestmentVM vm)
        {
            try
            {
                return new InvestmentDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(InvestmentVM vm, string[] ids)
        {
            try
            {
                return new InvestmentDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(string[] ids)
        {
            try
            {
                return new InvestmentDAL().Post(ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public DataTable Report(InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable InvestmentStatementReport(InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentDAL().InvestmentStatementReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
