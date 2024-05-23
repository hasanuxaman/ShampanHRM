using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class PublicApplicantRepo
    {
        #region Methods
        //==================SelectAll=================
       public List<PublicApplicantVM> DropDown()
        {
            try
            {
                return new PublicApplicantDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectAll=================
        public List<PublicApplicantVM> SelectAll()
        {
            try
            {
                return new PublicApplicantDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public PublicApplicantVM SelectById(string Id)
        {
            try
            {
                return new PublicApplicantDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(PublicApplicantVM PublicApplicantVM)
        {
            try
            {
                return new PublicApplicantDAL().Insert(PublicApplicantVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(PublicApplicantVM PublicApplicantVM)
        {
            try
            {
                return new PublicApplicantDAL().Update(PublicApplicantVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(PublicApplicantVM PublicApplicantVM, string[] ids)
        {
            try
            {
                return new PublicApplicantDAL().Delete(PublicApplicantVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
