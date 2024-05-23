using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class Appraisal360UserFeedbackRepo
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

        public List<EnumValue> GetFeedbackValue(string periodName, string feedBackYear)
        {
            try
            {
                return new Appraisal360UserFeedbackDAL().GetFeedbackValue(periodName, feedBackYear);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //==================SelectAll=================
        public List<Appraisal360FeedBackVM> SelectAll(Appraisal360FeedBackVM vm)
        {
            try
            {
                return new Appraisal360UserFeedbackDAL().SelectAll(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<Appraisal360DetailVM> SelectAllList(Appraisal360FeedBackVM vm)
        {
            try
            {
                return new Appraisal360UserFeedbackDAL().SelectAllList(vm);
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
        public string[] Update(Appraisal360FeedBackVM vm)
        {
            try
            {
                return new Appraisal360UserFeedbackDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //================== CompletedFeedBack =================
        public string[] CompletedFeedBack(Appraisal360FeedBackVM vm)
        {
            try
            {
                return new Appraisal360UserFeedbackDAL().CompletedFeedBack(vm, null, null);
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
