using SymServices.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Enum
{
    public class EnumRepo
    {
        EnumDAL _enumDAL = new EnumDAL();

        #region PF - Module

        public IEnumerable<object> GetPFWithdrawTypeList()
        {
            try
            {
                return _enumDAL.GetPFWithdrawTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region GL - GDIC - Module
        public IEnumerable<object> GLAuditStatusList()
        {
            try
            {
                return _enumDAL.GLAuditStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GLDateTypeList()
        {
            try
            {
                return _enumDAL.GLDateTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GLYesNoList()
        {
            try
            {
                return _enumDAL.GLYesNoList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GLStatusList()
        {
            try
            {
                return _enumDAL.GLStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GLDocumentTypeList()
        {
            try
            {
                return _enumDAL.GLDocumentTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<object> GLGetAccountNatureList()
        {
            try
            {
                return _enumDAL.GLGetAccountNatureList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region HRM - Payroll

        public IEnumerable<object> ReportTypesList()
        {
            try
            {
                return _enumDAL.ReportTypesList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> BonusStatusList()
        {
            try
            {
                return _enumDAL.BonusStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> HoldStatusList()
        {
            try
            {
                return _enumDAL.HoldStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> DaysOfWeekList()
        {
            try
            {
                return _enumDAL.DaysOfWeekList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> ForceList()
        {
            try
            {
                return _enumDAL.ForceList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> NoChildrenList()
        {
            try
            {
                return _enumDAL.NoChildrenList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> FeetList()
        {
            try
            {
                return _enumDAL.FeetList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> HeightInchList()
        {
            try
            {
                return _enumDAL.HeightInchList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> RetirementList()
        {
            try
            {
                return _enumDAL.RetirementList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> EMPTypeList()
        {
            try
            {
                return _enumDAL.EMPTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> ImportList()
        {
            try
            {
                return _enumDAL.ImportList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> EmpCategoryList()
        {
            try
            {
                return _enumDAL.EmpCategoryList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetStructureTypeList()
        {
            try
            {
                return _enumDAL.GetStructureTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Acc 

        public IEnumerable<object> GetPostStatusList()
        {
            try
            {
                return _enumDAL.GetPostStatusList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetAreaNameList()
        {
            try
            {
                return _enumDAL.GetAreaNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetAccountNatureList()
        {
            try
            {
                return _enumDAL.GetAccountNatureList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetReportTypeList()
        {
            try
            {
                return _enumDAL.GetReportTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetAccountTypeList()
        {
            try
            {
                return _enumDAL.GetAccountTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetTransactionTypeList()
        {
            try
            {
                return _enumDAL.GetTransactionTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetSaleCollectionGroupByList()
        {
            try
            {
                return _enumDAL.GetSaleCollectionGroupByList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetNonStockTypeList()
        {
            try
            {
                return _enumDAL.GetNonStockTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetGroupTypeList()
        {
            try
            {
                return _enumDAL.GetGroupTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetProductTypeList()
        {
            try
            {
                return _enumDAL.GetProductTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetLetterNameList()
        {
            try
            {
                return _enumDAL.GetLetterNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> AbsentDeductFromList()
        {
            try
            {
                return _enumDAL.AbsentDeductFromList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> DaysCountList()
        {
            try
            {
                return _enumDAL.DaysCountList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetSalaryHeadTypeList()
        {
            try
            {
                return _enumDAL.GetSalaryHeadTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetSalarySheetNameList()
        {
            try
            {
                return _enumDAL.GetSalarySheetNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetHoliDayTypeNameList()
        {
            try
            {
                return _enumDAL.GetHoliDayTypeNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetAttnStatusNameList()
        {
            try
            {
                return _enumDAL.GetAttnStatusNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public IEnumerable<object> GetLoanInterestPolicyList()
        {
            try
            {
                return _enumDAL.GetLoanInterestPolicyList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<object> GetSalaryProcessNameList()
        {
            try
            {
                return _enumDAL.GetSalaryProcessNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<object> GetEarningRptParamNameList()
        {
            try
            {
                return _enumDAL.GetEarningRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetDeductionRptParamNameList()
        {
            try
            {
                return _enumDAL.GetDeductionRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetPFTaxRptParamNameList()
        {
            try
            {
                return _enumDAL.GetPFTaxRptParamNameList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
