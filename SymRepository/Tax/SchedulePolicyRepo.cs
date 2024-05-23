using SymServices.Common;
using SymServices.Tax;
using SymViewModel.Common;
using SymViewModel.Tax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SymRepository.Tax
{
    public class SchedulePolicyRepo
    {
        SchedulePolicyDAL _dal = new SchedulePolicyDAL();
        #region Methods
        //==================SelectAll=================
        public List<SchedulePolicyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SchedulePolicyDAL().SelectAll(Id, conditionFields, conditionValues);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] Update(List<SchedulePolicyVM> VMs)
        {
            try
            {
                return new SchedulePolicyDAL().Update(VMs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
