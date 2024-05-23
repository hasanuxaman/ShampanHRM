using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;

namespace SymWebUI.Areas.Payroll.Controllers
{
    public class CommonWebMethod
    {
        public void FormulaField(ReportDocument objrpt, FormulaFieldDefinitions crFormulaF, string fieldName, string fieldValue)
        {
            try
            {
                FormulaFieldDefinition fs;
                fs = crFormulaF[fieldName];
                objrpt.DataDefinition.FormulaFields[fieldName].Text = "'" + fieldValue + "'";
            }
            catch (Exception ex)
            {


            }


        }

    }
}