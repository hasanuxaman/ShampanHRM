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
    public class TaxDepositRepo
    {
        public List<TaxDepositVM> DropDown()
        {
            try
            {
                return new TaxDepositDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TaxDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TaxDepositDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(TaxDepositVM vm)
        {
            try
            {
                return new TaxDepositDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(TaxDepositVM vm)
        {
            try
            {
                return new TaxDepositDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(TaxDepositVM vm, string[] ids)
        {
            try
            {
                return new TaxDepositDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetExcelExportData(string fid, string orderBy, string particular)
        {
            try
            {
                return new TaxDepositDAL().GetExcelExportData(fid, orderBy, particular);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetExcelSelfExportData(string fid, string orderBy, string particular)
        {
            try
            {
                return new TaxDepositDAL().GetExcelSelfExportData(fid, orderBy, particular);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] ImportData(DataTable table)
        {
            try
            {
                return new TaxDepositDAL().ImportData(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(TaxDepositVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new TaxDepositDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TaxDepositVM> EmployeeSalaryTaxList(TaxDepositVM vm, string orderBy)
        {
            try
            {
                return new TaxDepositDAL().EmployeeSalaryTaxList(vm, orderBy);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertEmployeeMultipleTaxDeposit(DataTable table, TaxDepositVM vm)
        {
            try
            {
                return new TaxDepositDAL().InsertEmployeeMultipleTaxDeposit(table, vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
