using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class AssetRepo
    {
        #region Methods
        //==================SelectAll=================
       public List<AssetVM> DropDown()
        {
            try
            {
                return new AssetDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //==================SelectAll=================
        public List<AssetVM> SelectAll()
        {
            try
            {
                return new AssetDAL().SelectAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //==================SelectByID=================
        public AssetVM SelectById(string Id)
        {
            try
            {
                return new AssetDAL().SelectById(Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Insert =================
        public string[] Insert(AssetVM AssetVM)
        {
            try
            {
                return new AssetDAL().Insert(AssetVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Update =================
        public string[] Update(AssetVM AssetVM)
        {
            try
            {
                return new AssetDAL().Update(AssetVM, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Select =================
        public AssetVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            try
            {
                return new AssetDAL().Select(query, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==================Delete =================
        public string[] Delete(AssetVM AssetVM, string[] ids)
        {
            try
            {
                return new AssetDAL().Delete(AssetVM,ids, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
