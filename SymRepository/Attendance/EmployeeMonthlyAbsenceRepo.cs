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
    public class EmployeeMonthlyAbsenceRepo
    {
        EmployeeMonthlyAbsenceDAL _dal = new EmployeeMonthlyAbsenceDAL();

        public List<EmployeeMonthlyAbsenceVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, int fid)
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

        public string[] Insert(List<EmployeeMonthlyAbsenceVM> VMs, string fid, ShampanIdentityVM vm)
        {
            {
                try
                {
                    return _dal.Insert(VMs, fid, vm);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string[] InsertFromDailyAbsence(string fid, ShampanIdentityVM auditvm)
        {
            {
                try
                {
                    return _dal.InsertFromDailyAbsence(fid, auditvm);
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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
        public DataTable ExportExcelFile(EmployeeMonthlyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
        public DataTable Report(EmployeeMonthlyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null)
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

    }
}
