using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class BlockEmployerRepo
    {
        BlockEmployerDAL _dal = new BlockEmployerDAL();
        //==================SelectAll=================
        public List<BlockEmployerVM> SelectAll()
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
