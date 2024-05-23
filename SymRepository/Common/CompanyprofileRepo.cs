using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
    public class CompanyprofileRepo
    {

        CompanyprofileDAL _companyprofileDAL = new CompanyprofileDAL(); 

        public string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles)
        {
             string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            try
            {
                retResults = _companyprofileDAL.UpdateCompanyProfileNew(companyProfiles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
			
			 

            return retResults;
        }

        public DataTable SearchCompanyProfile()
        {
            DataTable dataTable = new DataTable("CProfile");

            try
            {
                dataTable = _companyprofileDAL.SearchCompanyProfile();
            }
            catch (Exception ex)
            {
                throw ex;
            }

           
            return dataTable;
        }

        public DataSet ComapnyProfileString(string CompanyID)
        {
            DataSet dataTable = new DataSet();

            try
            {
                dataTable = _companyprofileDAL.ComapnyProfileString(CompanyID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;

        }

        public DataSet ComapnyProfile(string CompanyID)
        {
            DataSet dataSet = new DataSet("ReportVAT16");

            try
            {
                dataSet = _companyprofileDAL.ComapnyProfile(CompanyID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

             

            return dataSet;
        }

        public DataSet ComapnyProfileSecurity(string CompanyID)
        {
            DataSet dataTable = new DataSet();

            try
            {
                dataTable = _companyprofileDAL.ComapnyProfileSecurity(CompanyID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
             
            return dataTable;

        }
    }
}
