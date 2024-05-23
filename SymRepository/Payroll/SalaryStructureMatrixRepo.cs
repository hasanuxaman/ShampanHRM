using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class SalaryStructureMatrixRepo
    {
        SalaryStructureMatrixDAL _dal = new SalaryStructureMatrixDAL();
        #region Methods

        public string BasicAmount(string Grade, string Step, string year, string StepSL = "",string yearpart="")
        {
            try
            {
                return _dal.BasicAmount(Grade, Step, year, StepSL, yearpart);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryStructureMatrixVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SalaryStructureMatrixVM> CurrentYearPartDropDown()
        {
            try
            {
                return _dal.CurrentYearPartDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<SalaryStructureMatrixVM> SelectAll(string SalaryTypeName =null, string GradeId = null,string  desGroupId = null, string currentYear = "", string currentYearPart = "")
        {
            try
            {
                return new SalaryStructureMatrixDAL().SelectAll(SalaryTypeName, GradeId, desGroupId, currentYear, currentYearPart);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public SalaryStructureMatrixVM SelectById(string Id)
        {
            try
            {
                return new SalaryStructureMatrixDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(SalaryStructureMatrixVM vm)
        {
            try
            {
                return new SalaryStructureMatrixDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(SalaryStructureMatrixVM vm)
        {
            try
            {
                return new SalaryStructureMatrixDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] MatrixCreate(SalaryStructureMatrixVM vm)
        {
            try
            {
                return new SalaryStructureMatrixDAL().MatrixCreate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MatrixEffectSave(SalaryStructureMatrixVM vm)
        {
            try
            {
                return new SalaryStructureMatrixDAL().MatrixEffectSave(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
       //==================Delete =================
        public string[] Delete(SalaryStructureMatrixVM vm, string[] Ids)
        {
            try
            {
                return new SalaryStructureMatrixDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SalaryStructureMatrixVM> SelectFiscalYearMonthlies(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new SalaryStructureMatrixDAL().SelectFiscalYearMonthlies(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable MatrixCreateCheck(SalaryStructureMatrixVM VM)
        {
            try
            {
                return new SalaryStructureMatrixDAL().MatrixCreateCheck(VM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable SelectSalaryStructureMatrixDownload(string CurrentYear = "", string CurrentYearPart = "")
        {
            try
            {
                return new SalaryStructureMatrixDAL().SelectSalaryStructureMatrixDownload(CurrentYear, CurrentYearPart);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
