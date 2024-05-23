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
    public class Schedule2HousePropertyYearlyRepo
    {
        public List<Schedule2HousePropertyVM> DropDown()
        {
            try
            {
                return new Schedule2HousePropertyYearlyDAL().DropDown();
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
                return new Schedule2HousePropertyYearlyDAL().SelectAll(Id, conditionFields, conditionValues);
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
                return new Schedule2HousePropertyYearlyDAL().Insert(vm);
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
                return new Schedule2HousePropertyYearlyDAL().Update(vm);
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
                return new Schedule2HousePropertyYearlyDAL().Delete(vm, ids);
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
                return new Schedule2HousePropertyYearlyDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
