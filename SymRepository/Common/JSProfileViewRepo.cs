using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class JSProfileViewRepo
    {
        JSProfileViewDAL _dal = new JSProfileViewDAL();
        public List<JSProfileViewVM> SelectAll()
        {
            try
            {
                return _dal.SelectAll();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
