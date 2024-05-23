using SymServices.Payroll;
using SymViewModel.Common;
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
    public class SalaryEarningLeaveRepo
    {
        EmployeeEarningLeaveDAL _dal = new EmployeeEarningLeaveDAL();
        #region Methods



        //==================Get All Distinct FiscalPeriodName =================
        public List<EmployeeEarningLeaveVM> GetPeriodname() {
            try
            {
                return new EmployeeEarningLeaveDAL().GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAll=================
        public List<EmployeeEarningLeaveVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectAll(empid, fid, DTId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public EmployeeEarningLeaveVM SelectById(string Id)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEarningLeaveVM SelectEmployeeBasicSalary(string Id, string SalaryPreiodId)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectEmployeeBasicSalary(Id, SalaryPreiodId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEarningLeaveVM SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0, string edType = "0", string SalaryPreiodId = "")
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType, SalaryPreiodId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeEarningLeaveVM vm)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Update =================
        public string[] Update(EmployeeEarningLeaveVM vm)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(EmployeeEarningLeaveVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0, int SFYId = 0)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().ImportExcelFile(Fullpath, fileName, vm, null, null, FYDId, SFYId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby=null)
        {
            return new EmployeeEarningLeaveDAL().ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
        }
         //==================SelectAllForReport=================
         public List<EmployeeEarningLeaveVM> SelectAllForReport(int fid, int fidTo, string ProjectId,
             string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTId = 0,
             string Orderby = null, string reportName = "")
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId,
                    DesignationId, CodeF, CodeT, DTId, Orderby, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<EmployeeEarningLeaveVM> SelectAllForReportSummary(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTId = 0, string Orderby = null)
        {
            try
            {
                return new EmployeeEarningLeaveDAL().SelectAllForReportSummary(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, DTId, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


       
        #endregion

       

    }
}
