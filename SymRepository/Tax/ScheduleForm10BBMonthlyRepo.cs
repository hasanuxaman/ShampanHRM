using SymServices.Tax;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Tax
{
    public class ScheduleForm10BBMonthlyRepo
    {
        public List<ScheduleForm10BBMonthlyVM> DropDown()
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ScheduleForm10BBMonthlyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ScheduleForm10BBMonthlyVM vm)
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ScheduleForm10BBMonthlyVM vm)
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(ScheduleForm10BBMonthlyVM vm, string[] ids)
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(ScheduleForm10BBMonthlyVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ScheduleForm10BBMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
