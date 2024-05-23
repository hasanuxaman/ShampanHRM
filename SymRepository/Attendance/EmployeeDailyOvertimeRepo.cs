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
    public class EmployeeDailyOvertimeRepo
    {
        EmployeeDailyOvertimeDAL _dal = new EmployeeDailyOvertimeDAL();

        public List<EmployeeDailyOvertimeVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, string date = "")
        {
            try
            {
                return _dal.SelectAll(ProjectId, DepartmentId, SectionId, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(List<EmployeeDailyOvertimeVM> VMs, string OTDate)
        {
            try
            {
                return _dal.Insert(VMs, OTDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable Report(EmployeeDailyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
        public DataTable ExportExcelFile(EmployeeDailyOvertimeVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
