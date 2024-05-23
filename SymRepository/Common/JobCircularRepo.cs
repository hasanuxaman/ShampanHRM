using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class JobCircularRepo
    {
        #region Methods
        //==================SelectAll=================
       public List<JobCircularVM> DropDown()
        {
            try
            {
                return new JobCircularDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<JobCircularVM> SelectAll()
        {
            try
            {
                return new JobCircularDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public JobCircularVM SelectById(string Id)
        {
            try
            {
                return new JobCircularDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(JobCircularVM JobCircularVM)
        {
            try
            {
                return new JobCircularDAL().Insert(JobCircularVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(JobCircularVM JobCircularVM)
        {
            try
            {
                return new JobCircularDAL().Update(JobCircularVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        //==================Delete =================
        public string[] Delete(JobCircularVM JobCircularVM, string[] ids)
        {
            try
            {
                return new JobCircularDAL().Delete(JobCircularVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
