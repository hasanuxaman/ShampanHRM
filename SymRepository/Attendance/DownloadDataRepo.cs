using SymServices.Attendance;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using System;
using System.Collections.Generic;


namespace SymRepository.Attendance
{
    public class DownloadDataRepo
    {
        DownloadDataDAL _dal = new DownloadDataDAL();
        #region Methods
        public List<DownloadDataVM> DropDown()
        {
            try
            {
                return _dal.DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> Autocomplete(string term)
        {
            try
            {
                return _dal.Autocomplete(term);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //==================SelectAll=================
        public List<DownloadDataVM> SelectNotMigrated()
        {
            try
            {
                return new DownloadDataDAL().SelectIsMigrated();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<DownloadDataVM> SelectAll()
        {
            try
            {
                return new DownloadDataDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public DownloadDataVM SelectById(string Id)
        {
            try
            {
                return new DownloadDataDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(DownloadDataVM vm)
        {
            try
            {
                return new DownloadDataDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertManual(DownloadDataVM vm)
        {
            try
            {
                return new DownloadDataDAL().InsertManual(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(DownloadDataVM vm)
        {
            try
            {
                return new DownloadDataDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(DownloadDataVM vm, string[] ids)
        {
            try
            {
                return new DownloadDataDAL().Delete(vm, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
