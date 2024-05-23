using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
    public class SetUpRepo
    {
        public List<Events> SelectAll()
        {
            try
            {
                return new SetUpDAL().SelectAllEvent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EmployeesHierarchyVM> SelectAllEmp()
        {
            try
            {
                return new SetUpDAL().SelectAllEmployee();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
