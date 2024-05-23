using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class DataMigrationRepo
    {
        DataMigrationDAL _dal = new DataMigrationDAL();
        public string[] Insert(string structureType = "", ShampanIdentityVM siVM = null)
        {
            try
            {
                return _dal.Insert(structureType, siVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] OpeningLeavePost( ShampanIdentityVM siVM)
        {
            try
            {
                return _dal.OpeningLeavePost(siVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
