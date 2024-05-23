using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class SageRepo
    {
        SageDAL _dal = new SageDAL();
        public string[] SageIntegration(List<JournalLedgerDetailVM> getAllData)
        {
            try
            {
                return new SageDAL().SageIntegration( getAllData, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<JournalLedgerDetailVM> SageIntegrationVoucher(string FiscalYearDetailId = null, string DepartmentId = null, string SectionId = null, string ProjectId = null, List<string> multiEmployeeId = null, string JournalDesc = null, bool IsReverse = false)
        {
            try
            {
                return _dal.SageIntegrationVoucher(FiscalYearDetailId, DepartmentId, SectionId, ProjectId, multiEmployeeId, JournalDesc, IsReverse,  null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<JournalLedgerDetailVM> SelectAllJournalLedgerDetail(string FiscalYearDetailId = null, string PostingDate = null, string Id = null)
        {
            try
            {
                return _dal.SelectAllJournalLedgerDetail(FiscalYearDetailId, PostingDate, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<JournalLedgerVM> SelectAllJournalLedger()
        {
            try
            {
                return _dal.SelectAllJournalLedger();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
