using SymServices.Common;
using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class ELProcessRepo
    {
        ELProcessDAL _dal = new ELProcessDAL();


        //==================Insert =================
        public string[] ELProcess(string attnDate)
        {
            try
            {
                return _dal.ELProcess(attnDate, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      

    }
}
