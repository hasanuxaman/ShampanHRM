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
    public class Schedule2HousePropertyMonthlyRepo
    {
        public List<Schedule2HousePropertyVM> DropDown()
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         public List<Schedule2HousePropertyVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().SelectEmployeeList(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Schedule2HousePropertyVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().SelectFiscalPeriod(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public List<Schedule2HousePropertyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(Schedule2HousePropertyVM vm)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(Schedule2HousePropertyVM vm)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Delete(Schedule2HousePropertyVM vm, string[] ids)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(Schedule2HousePropertyVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new Schedule2HousePropertyMonthlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
