using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class SalaryOtherEarningRepo
    {
        SalaryOtherEarningDAL _dal = new SalaryOtherEarningDAL();
        #region Methods
        public List<SalaryOtherEarningVM> GetPeriodname()
        {
            try
            {
                return new SalaryOtherEarningDAL().GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<SalaryOtherEarningVM> SelectAll(string empid = null, int? fid = null, int? ETId = null)
        {
            try
            {
                return new SalaryOtherEarningDAL().SelectAll(empid, fid, ETId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<SalaryOtherEarningVM> SelectAllByFIDPeriod(int? fid = null)
        {
            try
            {
                return new SalaryOtherEarningDAL().SelectAllByFIDPeriod(fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public SalaryOtherEarningVM SelectById(string Id)
        {
            try
            {
                return new SalaryOtherEarningDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SalaryOtherEarningVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0", string edType = "0")
        {
            try
            {
                return new SalaryOtherEarningDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(SalaryOtherEarningVM vm)
        {
            try
            {
                return new SalaryOtherEarningDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================

        //==================Delete =================
        public string[] Delete(SalaryOtherEarningVM vm, string[] Ids)
        {
            try
            {
                return new SalaryOtherEarningDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportOtherExcelFile(string fileName, ShampanIdentityVM vm)
        {
            try
            {
                return new SalaryOtherEarningDAL().ImportOtherExcelFile(fileName, vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ExportOtherExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId
            , string CodeF, string CodeT, int fid = 0, int ETId = 0)
        {
            return new SalaryOtherEarningDAL().ExportotherExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, ETId);
        }
        //==================SelectAllForReport=================
        public List<SalaryOtherEarningVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int ETid = 0, string Orderby=null)
        {
            try
            {
                return new SalaryOtherEarningDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, ETid, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
