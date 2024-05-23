using SymServices.Attendance;
using SymServices.HRM;
using SymViewModel.Attendance;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Attendance
{
    public class AttLogsRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<AttLogsVM> SelectAll(string Id="")
        {
            try
            {
                return new AttLogsDAL().SelectAll(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectAllData with Fetch =================
        public List<AttLogsVM> SelectAllData(int pageNo,int pageSize)
        {
            try
            {
                return new AttLogsDAL().SelectAllData(pageNo, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //==================SelectByID=================
        public AttLogsVM SelectById(string Id)
        {
            try
            {
                return new AttLogsDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(AttLogsVM VM)
        {
            try
            {
                return new AttLogsDAL().Insert(VM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(AttLogsVM attLogsVM)
        {
            try
            {
                return new AttLogsDAL().Update(attLogsVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================Delete =================
        public string[] Delete(AttLogsVM bankVM, string[] ids)
        {
            try
            {
                return new AttLogsDAL().Delete(bankVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

}
