using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class Schedule3InvestmentMonthlyRepo
    {
        public List<Schedule3InvestmentVM> DropDown()
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Schedule3InvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, string tType="")
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().SelectAll(Id, conditionFields, conditionValues, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule3InvestmentVM vm)
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule3InvestmentVM vm)
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(Schedule3InvestmentVM vm, string[] ids)
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(Schedule3InvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule3InvestmentMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
