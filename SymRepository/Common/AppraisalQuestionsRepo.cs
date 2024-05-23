using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class AppraisalQuestionsRepo
   {
       #region Methods

       public List<AppraisalQuestionsVM> DropDown()
       {
           try
           {
               return new AppraisalQuestionsDAL().DropDown();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //==================SelectAll=================
       public List<AppraisalQuestionsVM> SelectAll()
       {
           try
           {
               return new AppraisalQuestionsDAL().SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }

       }
       //==================SelectByID=================
       public AppraisalQuestionsVM SelectById(int Id)
       {
           try
           {
               return new AppraisalQuestionsDAL().SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Insert =================
       public string[] Insert(AppraisalQuestionsVM appraisalQuestionsVM)
       {
           try
           {
               return new AppraisalQuestionsDAL().Insert(appraisalQuestionsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Update =================
       public string[] Update(AppraisalQuestionsVM appraisalQuestionsVM)
       {
           try
           {
               return new AppraisalQuestionsDAL().Update(appraisalQuestionsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Select =================
       public AppraisalQuestionsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
       {

           try
           {
               return new AppraisalQuestionsDAL().Select(query, Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Delete =================
       public string[] Delete(AppraisalQuestionsVM appraisalQuestionsVM, string[] ids)
       {
           try
           {
               return new AppraisalQuestionsDAL().Delete(appraisalQuestionsVM, ids, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       #endregion
   }
}
