using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class AppraisalMarkSetupsRepo
   {
       #region Methods

        public List<AppraisalMarkSetupsVM> DropDown()
       {
           try
           {
               return new AppraisalMarkSetupsDAL().DropDown();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //==================SelectAll=================
        public List<AppraisalMarkSetupsVM> SelectAll()
       {
           try
           {
               return new AppraisalMarkSetupsDAL().SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }

       }
       //==================SelectByID=================
        public AppraisalMarkSetupsVM SelectById(int Id)
       {
           try
           {
               return new AppraisalMarkSetupsDAL().SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Insert =================
        public string[] Insert(AppraisalMarkSetupsVM appraisalMarkSetupsVM)
       {
           try
           {
               return new AppraisalMarkSetupsDAL().Insert(appraisalMarkSetupsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Update =================
        public string[] Update(AppraisalMarkSetupsVM appraisalMarkSetupsVM)
       {
           try
           {
               return new AppraisalMarkSetupsDAL().Update(appraisalMarkSetupsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Select =================
        public AppraisalMarkSetupsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
       {
           try
           {
               return new AppraisalMarkSetupsDAL().Select(query, Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Delete =================
        public string[] Delete(AppraisalMarkSetupsVM appraisalMarkSetupsVM, string[] ids)
       {
           try
           {
               return new AppraisalMarkSetupsDAL().Delete(appraisalMarkSetupsVM, ids, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       #endregion
   }
}
