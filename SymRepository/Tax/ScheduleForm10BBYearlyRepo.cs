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
    public class ScheduleForm10BBYearlyRepo
    {
        public List<ScheduleForm10BBYearlyVM> DropDown()
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ScheduleForm10BBYearlyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(ScheduleForm10BBYearlyVM vm)
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(ScheduleForm10BBYearlyVM vm)
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(ScheduleForm10BBYearlyVM vm, string[] ids)
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(ScheduleForm10BBYearlyVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new ScheduleForm10BBYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
