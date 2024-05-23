using SymServices.Attendance;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymRepository.Attendance
{
    public class EmployeeMonthlyOvertimeRepo
    {
        EmployeeMonthlyOvertimeDAL _dal = new EmployeeMonthlyOvertimeDAL();

        public List<EmployeeMonthlyOvertimeVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, int fid)
        {
            try
            {
                return _dal.SelectAll(ProjectId, DepartmentId, SectionId, fid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(List<EmployeeMonthlyOvertimeVM> VMs, string fid, ShampanIdentityVM auditvm)
        {
                try
                {
                    return _dal.Insert(VMs, fid, auditvm);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }

        public string[] InsertFromDailyOvertime(string FiscalYearDetailId,  ShampanIdentityVM auditvm)
        {
            try
            {
                return _dal.InsertFromDailyOvertime(FiscalYearDetailId, auditvm);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable Report(EmployeeMonthlyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _dal.Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm)
        {
            try
            {
                return _dal.ImportExcelFile(fullPath, fileName, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable ExportExcelFile(EmployeeMonthlyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return _dal.ExportExcelFile(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
