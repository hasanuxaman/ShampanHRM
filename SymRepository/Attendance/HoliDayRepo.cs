using SymServices.Attendance;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class HoliDayRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<HoliDayVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new HoliDayDAL().SelectAll(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAll=================
        public List<HoliDayVM> SelectAll()
        {
            try
            {
                return new HoliDayDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<DateTime> SelectAllHoliDate()
        {
            try
            {
                return new HoliDayDAL().SelectAllHoliDate();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public HoliDayVM SelectById(string Id)
        {
            try
            {
                return new HoliDayDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HoliDayVM SelectByDate(string date)
        {
            try
            {
                return new HoliDayDAL().SelectByDate(date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Insert =================
        public string[] Insert(HoliDayVM vm)
        {
            try
            {
                return new HoliDayDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(HoliDayVM vm)
        {
            try
            {
                return new HoliDayDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(HoliDayVM vm, string[] ids)
        {
            try
            {
                return new HoliDayDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

}
