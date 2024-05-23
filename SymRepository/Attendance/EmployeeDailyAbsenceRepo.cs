using SymServices.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
   public class EmployeeDailyAbsenceRepo
    {
        EmployeeDailyAbsenceDAL _dal = new EmployeeDailyAbsenceDAL();

        public List<EmployeeDailyAbsenceVM> SelectAll(string ProjectId, string DepartmentId, string SectionId, string date = "", string tType = "")
        {
            try
            {
                return _dal.SelectAll( ProjectId, DepartmentId, SectionId, date, tType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Insert(List<EmployeeDailyAbsenceVM> VMs, string AbsentDate)
        {
            try
            {
                return _dal.Insert(VMs, AbsentDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(EmployeeDailyAbsenceVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
