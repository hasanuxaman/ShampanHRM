using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class SymUserAppraisal360repo
   {
       #region Methods
       //==================DropDownModule=================
       public List<SymUserRollVM> DropDownModule()
        {
            try
            {
                return new SymUserAppraisal360DAL().DropDownsymArea();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       //==================DropDownMenu=================
        public List<SymUserRollVM> DropDownMenu()
        {
            try
            {
                return new SymUserAppraisal360DAL().DropDownsymController();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<SymUserRollVM> SelectSymUserRollAll()
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymUserRollVM> SelectAllSymUserRollByGroupId(string GroupId, string SymArea = null)
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectAllByGroupId(GroupId, SymArea);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<UserLogsVM> SelectAllUser()
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectAllUser();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<SymUserDefaultRollVM> SelectSymUserById(string Id)
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectSymUserById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserLogsVM SelectUserById(string Id)
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectGroupId(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public SymUserRollVM SelectById(string Id)
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================InsertWithSelectAll =================
         public string[] SelectAllSymRollwithInsert(SymUserRollVM vm)
        {
            try
            {
                return new SymUserAppraisal360DAL().SelectAllSymRollwithInsert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(SymUserRollVM SymUserRollVM)
        {
            try
            {
                return new SymUserAppraisal360DAL().Insert(SymUserRollVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(UserGroupVM VM)
        {
            try
            {
                return new SymUserAppraisal360DAL().Update(VM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(SymUserRollVM SymUserRollVM, string[] ids)
        {
            try
            {
                return new SymUserAppraisal360DAL().Delete(SymUserRollVM, ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymUserDefaultRollVM> UserRollDetail(string empId, string Module)
        {
            try
            {
                return new SymUserAppraisal360DAL().UserRollDetail(empId, Module);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool SymRollSessionBackup(string UserId, string symArea, string symController, string IndexAddEditDeleteProcessReport)
        {
            try
            {
                return new SymUserAppraisal360DAL().SymRollSessionBackup(UserId, symArea, symController, IndexAddEditDeleteProcessReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool  SymRoleSession(string UserId, string DefaultRollId, string IndexAddEditDeleteProcessReport)
        {
            try
            {
                return new SymUserAppraisal360DAL().SymRoleSession(UserId, DefaultRollId, IndexAddEditDeleteProcessReport);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool Appraisal360(string FiscalPeriodDetailId, string FYId, string DId)
        {
            try
            {
                return new SymUserAppraisal360DAL().Appraisal360(FiscalPeriodDetailId, FYId, DId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable RollByUserId(string userId)
        {
            try
            {
                return new SymUserAppraisal360DAL().RollByGroupId(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        
   }
}
