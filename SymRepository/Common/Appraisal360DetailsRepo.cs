using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class Appraisal360DetailsRepo
   {
       #region Methods

        public List<Appraisal360DetailsVM> DropDown()
       {
           try
           {
               return new Appraisal360DetailsDAL().DropDown();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //==================SelectAll=================
        public List<Appraisal360DetailsVM> SelectAll()
       {
           try
           {
               return new Appraisal360DetailsDAL().SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================SelectByID=================
        public Appraisal360DetailsVM SelectById(int Id)
       {
           try
           {
               return new Appraisal360DetailsDAL().SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Insert =================
        public string[] Insert(Appraisal360DetailsVM appraisal360DetailsVM)
       {
           try
           {
               return new Appraisal360DetailsDAL().Insert(appraisal360DetailsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Update =================
        public string[] Update(Appraisal360DetailsVM appraisal360DetailsVM)
       {
           try
           {
               return new Appraisal360DetailsDAL().Update(appraisal360DetailsVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Select =================
        public Appraisal360DetailsVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
       {

           try
           {
               return new Appraisal360DetailsDAL().Select(query, Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Delete =================
        public string[] Delete(Appraisal360DetailsVM appraisal360DetailsVM, string[] ids)
       {
           try
           {
               return new Appraisal360DetailsDAL().Delete(appraisal360DetailsVM, ids, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       #endregion
   }
}
