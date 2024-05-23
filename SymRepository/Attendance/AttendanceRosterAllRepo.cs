using SymServices.Attendance;
using SymViewModel.Attendance;
using System;
using System.Collections.Generic;

namespace SymRepository.Attendance
{
    public class AttendanceRosterAllRepo
    {
        #region Methods
        //==================SelectAll=================
        public List<AttendanceRosterAllVM> SelectAll(int BranchId)
        {
            try
            {
                return new AttendanceRosterAllDAL().SelectAll(BranchId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<AttendanceRosterAllVM> SelectDetailAll()
        {
            try
            {
                return new AttendanceRosterAllDAL().SelectDetailAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public AttendanceRosterAllVM SelectById(string Id)
        {
            try
            {
                return new AttendanceRosterAllDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AttendanceRosterAllVM SelectByDetailId(string AttendanceStructureId, string AttendanceGroupId)
        {
            try
            {
                return new AttendanceRosterAllDAL().SelectByDetailId(AttendanceStructureId,AttendanceGroupId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(AttendanceRosterAllVM bVM)
        {
            try
            {
                return new AttendanceRosterAllDAL().Insert(bVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] InsertDetail(AttendanceRosterAllVM vm)
        {
            try
            {
                return new AttendanceRosterAllDAL().InsertDetail(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(AttendanceRosterAllVM vm)
        {
            try
            {
                return new AttendanceRosterAllDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
 
        //==================Delete =================
        public string[] Delete(string Name)
        {
            try
            {
                return new AttendanceRosterAllDAL().Delete(Name, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
