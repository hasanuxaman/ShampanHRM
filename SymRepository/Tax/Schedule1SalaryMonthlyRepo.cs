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
    public class Schedule1SalaryMonthlyRepo
    {
        public List<Schedule1SalaryVM> DropDown()
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EmployeeInfoVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, string tType = "")
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().SelectEmployeeList(conditionFields, conditionValues, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Schedule1SalaryVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null, string tType = "")
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().SelectFiscalPeriod(conditionFields, conditionValues, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Schedule1SalaryVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, string tType = "")
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().SelectAll(Id, conditionFields, conditionValues, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<YearlyTAXVM> YearlyTax(int Id = 0, string[] conditionFields = null,
            string[] conditionValues = null, List<EmloyeeTAXSlabVM> vms = null)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().YearlyTax(Id, conditionFields, conditionValues);
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
                return new Schedule1SalaryMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule1SalaryVM vm, string tType = "")
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().Update(vm, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] UpdateAdvanceTax(YearlyTAXVM vm)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().UpdateAdvanceTax(vm);
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
                return new Schedule1SalaryMonthlyDAL().Delete(vm, ids);
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
                return new Schedule1SalaryMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable InvestmentTaxReport(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().InvestmentTaxReport(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] InsertProcessUpdate(string FiscalYearDetailId, string FiscalYearDetailIdTo, string tType = "",
            ShampanIdentityVM auditvm = null, List<EmloyeeTAXSlabVM> VMs = null, string advanceTax = "N", string fYear = "", string effectForm = "")
        {
            try
            {
                if (tType=="Salary")
                {
                    return new Schedule1SalaryMonthlyDAL().InsertProcessUpdate(FiscalYearDetailId, FiscalYearDetailIdTo,
                        tType, auditvm, null, null, VMs, advanceTax, fYear, effectForm);
                }
                else
                {
                    return new Schedule1SalaryMonthlyDAL().InsertProcessUpdateNew(FiscalYearDetailId, FiscalYearDetailIdTo,
                 tType, auditvm, null, null, VMs, advanceTax, fYear, effectForm);
                }    
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable TaxReportNBRMonthly(string vFiscalYearDetailId)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().TaxReportNBRMonthly(vFiscalYearDetailId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable TaxIncomeReport(Schedule1SalaryVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().TaxIncomeReport(vm, conditionFields, conditionValues);
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

        public string[] UpdateSalaryTaxDetail(string FiscalYearDetailId, string tType = "", ShampanIdentityVM auditvm = null,string advanceTAX = "N")
        {
            try
            {
                return new Schedule1SalaryMonthlyDAL().UpdateSalaryTaxDetail(FiscalYearDetailId, tType, auditvm, null, null, advanceTAX);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
