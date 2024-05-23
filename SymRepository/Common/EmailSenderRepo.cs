using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class EmailSenderRepo
    {
        EmailSenderDAL _dal = new EmailSenderDAL();
        public List<EmailSenderVM> SelectAll()
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
