using SymServices.PF;
using SymServices.Common;

using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.PF
{
    public class PFDetailRepo
    {
       
        public List<PFDetailVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectEmployeeList(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFDetailVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectFiscalPeriod(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectFiscalPeriodHeader(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<PFDetailVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PFProcess(string FiscalYearDetailId, string ProjectId ,ShampanIdentityVM auditvm)
        {
            try
            {
                return new PFDetailDAL().PFProcess(FiscalYearDetailId, ProjectId, auditvm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] Post(PFDetailVM vm)
        {
            try
            {
                return new PFDetailDAL().Post(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] PostHeader(PFHeaderVM vm)
        {
            try
            {
                return new PFDetailDAL().PostHeader(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable Report(PFDetailVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().Report(vm, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable PFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new PFDetailDAL().PFEmployersProvisionsReport(conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
