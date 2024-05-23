using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class Appraisal360Repo
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

        //==================SelectAllAppraisal360VM=================
        public List<Appraisal360VM> SelectAllAppraisal360()
        {
            try
            {
                return new Appraisal360DAL().SelectAllAppraisal360();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //==================SelectAll=================
        public List<ViewAppraisal360VM> SelectAll()
        {
            try
            {
                return new Appraisal360DAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public Appraisal360VM SelectById(int Id)
        {
            try
            {
                return new Appraisal360DAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(Appraisal360VM Appraisal360)
        {
            try
            {
                return new Appraisal360DAL().Insert(Appraisal360, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(Appraisal360VM appraisal360VM)
        {
            try
            {
                return new Appraisal360DAL().Update(appraisal360VM, null, null);
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

       //========================== Appraisal360DAL =================

        public bool Appraisal360DataProcess(string FiscalPeriodDetailId, string FYId, string DId)
        {
            try
            {
                return new Appraisal360DAL().Appraisal360DataProcess(FiscalPeriodDetailId, FYId, DId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion


    }
}
