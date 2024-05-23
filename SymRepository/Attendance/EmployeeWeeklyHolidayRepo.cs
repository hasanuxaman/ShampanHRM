using SymServices.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;

namespace SymRepository.Attendance
{
    public class EmployeeWeeklyHolidayRepo
    {
        
        public List<EmployeeWeeklyHolidayVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new EmployeeWeeklyHolidayDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(EmployeeWeeklyHolidayVM vm)
        {
            try
            {
                return new EmployeeWeeklyHolidayDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(EmployeeWeeklyHolidayVM vm, string[] ids)
        {
            try
            {
                return new EmployeeWeeklyHolidayDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
