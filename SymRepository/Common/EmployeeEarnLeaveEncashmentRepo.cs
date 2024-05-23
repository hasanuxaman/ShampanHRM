using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class EmployeeEarnLeaveEncashmentRepo
    {
        EmployeeEarnLeaveEncashmentDAL _dal = new EmployeeEarnLeaveEncashmentDAL();

        //=================Get Distinct Period Name ================
        public List<EmployeeLeaveEncashmentVM> GetYear()
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().GetYear();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         //=================Get Employee Otherearning Information by fiscal year id ================
        public List<EmployeeLeaveEncashmentVM> GetEmpOtherEaringByFid(int fid)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().GetEmpOtherEaringByFid(fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeLeaveEncashmentVM> SelectAll(string empid = null, int? fid = null, int? ETId = null)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().SelectAll(empid, fid, ETId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveEncashmentBalance(string employeeId, string leaveyear)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().EmployeeLeaveEncashmentBalance(employeeId, leaveyear);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public EmployeeLeaveEncashmentVM SelectById(string Id)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeLeaveEncashmentVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0",string edType="0", string salaryMonth = "")
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType, salaryMonth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeLeaveEncashmentVM vm)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeLeaveEncashmentVM vm)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Post(EmployeeLeaveEncashmentVM vm)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().Post(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeLeaveEncashmentVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string fullpath, string fileName, ShampanIdentityVM vm, int FYDId = 0)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().ImportExcelFile(fullpath, fileName, vm, null, null, FYDId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int ETId = 0, string Orderby = null)
        {
            return new EmployeeEarnLeaveEncashmentDAL().ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, ETId, Orderby);
        }

        //==================OvertimeAmount=================
        public string OvertimeAmount(string OTHrs, string OTHrsSpecial, string Code, string PayrollPeriodId = "0", string EmployeeId="0")
        {
            try
            {
                return _dal.OvertimeAmount(OTHrs, OTHrsSpecial, Code, PayrollPeriodId, EmployeeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //==================SelectAllForReport=================
        public List<EmployeeLeaveEncashmentVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int ETid = 0, string Orderby=null)
        {
            try
            {
                return new EmployeeEarnLeaveEncashmentDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, ETid, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}
