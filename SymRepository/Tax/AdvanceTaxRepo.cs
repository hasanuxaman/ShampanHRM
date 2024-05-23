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
    public class AdvanceTaxRepo
    {
        public List<AdvanceTaxVM> DropDown()
        {
            try
            {
                return new AdvanceTaxDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AdvanceTaxVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AdvanceTaxDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(AdvanceTaxVM vm)
        {
            try
            {
                return new AdvanceTaxDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(AdvanceTaxVM vm)
        {
            try
            {
                return new AdvanceTaxDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(AdvanceTaxVM vm, string[] ids)
        {
            try
            {
                return new AdvanceTaxDAL().Delete(vm, ids);
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
                return new AdvanceTaxDAL().GetExcelExportData(fid, orderBy, particular);
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
                return new AdvanceTaxDAL().GetExcelSelfExportData(fid, orderBy, particular);
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
                return new AdvanceTaxDAL().ImportData(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(AdvanceTaxVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new AdvanceTaxDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
