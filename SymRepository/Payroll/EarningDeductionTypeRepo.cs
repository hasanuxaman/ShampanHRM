using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
    public class EarningDeductionTypeRepo
    {
        EarningDeductionTypeDAL _dal = new EarningDeductionTypeDAL();
        #region Methods

        public List<EarningDeductionTypeVM> EarningTypeDropDown()
        {
            try
            {
                return _dal.EarningTypeDropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EarningDeductionTypeVM> DeductionTypeDropDown()
        {
            try
            {
                return _dal.DeductionTypeDropDown();
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
        public List<EarningDeductionTypeVM> SelectAll()
        {
            try
            {
                return new EarningDeductionTypeDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public EarningDeductionTypeVM SelectById(int id)
        {
            try
            {
                return new EarningDeductionTypeDAL().SelectById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] EditOther(string Id, string GLAccountCode)
        {
            string[] results = new string[6];
            try
            {
                results = _dal.EditOther(Id, GLAccountCode, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return results;
        }
        #endregion

    }
}
