using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class HomeRepo
    {
       HomeDAL _DAL = new HomeDAL();

        //==================SelectAllEmployees Employee=================
        public List<EmployeeInfoVM> SelectAllEmployee()
        {
            List<EmployeeInfoVM> dataSet = new List<EmployeeInfoVM>();
            try
            {
                dataSet = _DAL.TotalEmployeeGender();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }
        public decimal TotalEmployeeSectionGender(string section, string gender)
        {
            decimal result = 0;
            try
            {
                result = _DAL.TotalEmployeeSectionGender(section, gender);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public DataSet TotalEmployeeSectionGender()
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet = _DAL.TotalEmployeeSectionGender();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }
        public DataSet TotalEmployeeJoinDate(string FromDate, string ToDate)
        {
            DataSet dataSet = new DataSet();
            try
            {
                dataSet = _DAL.TotalEmployeeJoinDate(FromDate,ToDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
    }
}
