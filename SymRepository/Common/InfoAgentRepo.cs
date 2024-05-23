using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class InfoAgentRepo
    {
        InfoAgentDAL _dal = new InfoAgentDAL();
            public List<InfoAgentVM> SelectAll()
            {
                try
                {
                    return _dal.SelectAll();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
    }
}
