using SymServices.HRM;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.HRM
{
   public class EmployeeFilesRepo
   {

       #region Methods
       //==================SelectAll=================
       public List<EmployeeFilesVM> SelectAll()
       {
           try
           {
               return new EmployeeFilesDAL().SelectAll();
           }
           catch (Exception ex)
           {
               throw ex;
           }

       }
       //==================SelectByID=================
       public EmployeeFilesVM SelectById(int Id)
       {
           try
           {
               return new EmployeeFilesDAL().SelectById(Id);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================SelectByEmployeeID=================
       public EmployeeFilesVM SelectByEmployeeId(string EmployeeId)
       {
           try
           {
               return new EmployeeFilesDAL().SelectByEmployeeId(EmployeeId);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }

       //==================Insert =================
       public string[] Insert(EmployeeFilesVM employeeFilesVM)
       {
           try
           {
               return new EmployeeFilesDAL().Insert(employeeFilesVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Update =================
       public string[] Update(EmployeeFilesVM employeeFilesVM)
       {
           try
           {
               return new EmployeeFilesDAL().Update(employeeFilesVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       //==================Delete =================
       public string[] Delete(EmployeeFilesVM employeeFilesVM)
       {
           try
           {
               return new EmployeeFilesDAL().Delete(employeeFilesVM, null, null);
           }
           catch (Exception ex)
           {
               throw ex;
           }
       }
       #endregion
   }
}
