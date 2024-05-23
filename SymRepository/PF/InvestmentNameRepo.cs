using SymServices.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;

namespace SymRepository.PF
{
    public class InvestmentNameRepo
    {

        public List<InvestmentNameVM> DropDown(string TransType = "PF")
        {
            try
            {
                return new InvestmentNameDAL().DropDown(TransType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<InvestmentNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new InvestmentNameDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<InvestmentNameDetailsVM> SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {

                return new InvestmentNameDAL().SelectAllDetails(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Insert(InvestmentNameVM vm)
        {
            try
            {
                return new InvestmentNameDAL().Insert(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] Update(InvestmentNameVM vm)
        {
            try
            {
                return new InvestmentNameDAL().Update(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Delete(InvestmentNameVM vm, string[] ids)
        {
            try
            {
                return new InvestmentNameDAL().Delete(vm, ids);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
    }
}
