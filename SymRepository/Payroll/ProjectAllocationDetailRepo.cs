using SymServices.Payroll;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Payroll
{
   public class ProjectAllocationDetailRepo
    {
        ProjectAllocationDetailDAL _dal = new ProjectAllocationDetailDAL();
        #region Methods

        //==================SelectAll=================
        public List<ProjectAllocationDetailVM> SelectAll()
        {
            try
            {
                return new ProjectAllocationDetailDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public ProjectAllocationDetailVM SelectById(string Id)
        {
            try
            {
                return new ProjectAllocationDetailDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================SelectByMasterID=================
        public List<ProjectAllocationDetailVM> SelectByMasterId(string masterId)
        {
            try
            {
                return new ProjectAllocationDetailDAL().SelectByMasterId(masterId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================Insert=================
        public string[] Insert(ProjectAllocationDetailVM vm)
        {
            try
            {
                return new ProjectAllocationDetailDAL().Insert(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(ProjectAllocationDetailVM vm)
        {
            try
            {
                return new ProjectAllocationDetailDAL().Update(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(ProjectAllocationDetailVM vm, string[] Ids)
        {
            try
            {
                return new ProjectAllocationDetailDAL().Delete(vm, Ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
