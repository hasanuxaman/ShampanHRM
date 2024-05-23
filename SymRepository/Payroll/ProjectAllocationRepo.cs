using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class ProjectAllocationRepo
    {
        #region Methods
        public List<ProjectAllocationVM> DropDown()
        {
            try
            {
                return new ProjectAllocationDAL().DropDown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectAll=================
        public List<ProjectAllocationVM> SelectAll()
        {
            try
            {
                return new ProjectAllocationDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByID=================
        public ProjectAllocationVM SelectById(string Id)
        {
            try
            {
                return new ProjectAllocationDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        //==================Insert =================
        public string[] Insert(ProjectAllocationVM vm)
        {
            try
            {
                return new ProjectAllocationDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(ProjectAllocationVM vm)
        {
            try
            {
                //throw new ArgumentNullException("ProjectAllocation Update", "Could not found any item.");
                return new ProjectAllocationDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(ProjectAllocationVM vm, string[] Ids)
        {
            try
            {
                return new ProjectAllocationDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
