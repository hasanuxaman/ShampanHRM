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
    public class EmployeeOtherDeductionRepo
    {
        EmployeeOtherDeductionDAL _dal = new EmployeeOtherDeductionDAL();
        #region Methods



        //==================Get All Distinct FiscalPeriodName =================
        public List<EmployeeOtherDeductionVM> GetPeriodname() {
            try
            {
                return new EmployeeOtherDeductionDAL().GetPeriodname();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAll=================
        public List<EmployeeOtherDeductionVM> SelectAll(string empid = null, int? fid = null, int? DTId = null)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().SelectAll(empid, fid, DTId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public EmployeeOtherDeductionVM SelectById(string Id)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeOtherDeductionVM SelectByIdandFiscalyearDetail(string empId, int FiscalYearDetailId = 0, string edType = "0")
        {
            try
            {
                return new EmployeeOtherDeductionDAL().SelectByIdandFiscalyearDetail(empId, FiscalYearDetailId, edType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(EmployeeOtherDeductionVM vm)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Update =================
        public string[] Update(EmployeeOtherDeductionVM vm)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(EmployeeOtherDeductionVM vm, string[] Ids)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().ImportExcelFile(Fullpath, fileName, vm, null, null, FYDId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int DTId = 0, string Orderby=null)
        {
            return new EmployeeOtherDeductionDAL().ExportExcelFile(Filepath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, DTId, Orderby);
        }
         //==================SelectAllForReport=================
        public List<EmployeeOtherDeductionVM> SelectAllForReport(int fid, int fidTo, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int DTId = 0, string Orderby=null)
        {
            try
            {
                return new EmployeeOtherDeductionDAL().SelectAllForReport(fid, fidTo, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, DTId, Orderby);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
