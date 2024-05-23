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
    public class EmployeeOtherEarningRepo
    {
        EmployeeOtherEarningDAL _dal = new EmployeeOtherEarningDAL();

        //=================Get Distinct Period Name ================
        public List<EmployeeOtherEarningVM> GetPeriodName()
        {
            try
            {
                return new EmployeeOtherEarningDAL().GetPeriodName();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         //=================Get Employee Otherearning Information by fiscal year id ================
        public List<EmployeeOtherEarningVM> GetEmpOtherEaringByFid(int fid)
        {
            try
            {
                return new EmployeeOtherEarningDAL().GetEmpOtherEaringByFid(fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<EmployeeOtherEarningVM> SelectAll(string empid = null, int? fid = null, int? ETId = null)
        {
            try
            {
                return new EmployeeOtherEarningDAL().SelectAll(empid, fid, ETId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public EmployeeOtherEarningVM SelectById(string Id)
        {
            try
            {
                return new EmployeeOtherEarningDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public EmployeeOtherEarningVM SelectByIdandFiscalyearDetail(string empId, string FiscalYearDetailId = "0",string edType="0", string salaryMonth = "")
        {
            try
            {
                return new EmployeeOtherEarningDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType, salaryMonth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeOtherEarningVM vm)
        {
            try
            {
                return new EmployeeOtherEarningDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(EmployeeOtherEarningVM vm)
        {
            try
            {
                return new EmployeeOtherEarningDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(EmployeeOtherEarningVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeOtherEarningDAL().Delete(vm, Ids, null, null);
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
                return new EmployeeOtherEarningDAL().ImportExcelFile(fullpath, fileName, vm, null, null, FYDId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int ETId = 0, string Orderby=null)
        {
            return new EmployeeOtherEarningDAL().ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, ETId, Orderby);
        }
        public DataTable ExportExcelFilePF(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, string Orderby = null)
        {
            return new EmployeeOtherEarningDAL().ExportExcelFilePF(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby);
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
        public List<EmployeeOtherEarningVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int ETid = 0, string Orderby=null)
        {
            try
            {
                return new EmployeeOtherEarningDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, ETid, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}
