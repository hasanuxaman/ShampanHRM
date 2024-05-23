using SymServices.Tax;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class Schedule1SalaryYearlyRepo
    {
        public List<Schedule1SalaryVM> DropDown()
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Schedule1SalaryVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule1SalaryVM vm)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule1SalaryVM vm)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(Schedule1SalaryVM vm, string[] ids)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertProcessUpdate(string FiscalYear, ShampanIdentityVM auditvm)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().InsertProcessUpdate(FiscalYear, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().SelectEmployeeList(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeInfoVM> SelectEmployeeListMonthlies(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().SelectEmployeeListMonthlies(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Schedule1SalaryVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().SelectFiscalYear(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Schedule1SalaryVM> SelectFiscalYearMonthlies(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryYearlyDAL().SelectFiscalYearMonthlies(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Schedule1SalaryVM ProcessSASalary(string LineA, int taxSlabId, bool isMonth = false)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().ProcessSASalary(LineA, taxSlabId, isMonth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
