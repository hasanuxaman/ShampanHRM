using Microsoft.Office.Interop.Excel;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Leave
{
   public class LeaveOpeningBalanceDAL
    {
        //public bool ImportExcelFile(string fileName, EmployeeLeaveVM vm)
        //{
        //    string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
        //                         "Data Source=" + fileName + ";" + "Extended Properties=" + "\"" +
        //                         "Excel 12.0;HDR=YES;" + "\"";
        //    OleDbConnection theConnection = new OleDbConnection(connectionString);
        //    try
        //    {

        //        theConnection.Open();
        //        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [DataSheet$]", theConnection);
        //        DataSet dt = new DataSet();
        //        da.Fill(dt);
        //        var a = "";
        //        EmployeeLeaveStructureVM el = new EmployeeLeaveStructureVM();
        //        List<EmployeeLeaveStructureVM> els = new List<EmployeeLeaveStructureVM>();// = new EmployeeLeaveVM();

        //        foreach (DataRow item in dt.Tables[0].Rows)
        //        {
        //            el = new EmployeeLeaveStructureVM();
        //            el.Id = Convert.ToInt32(item["Id"].ToString());
        //            el.LeaveYear = Convert.ToInt32(item["LeaveYear"].ToString());
        //            el.LeaveDays = Convert.ToDecimal(item["LeaveDays"].ToString());
        //            els.Add(el);
        //        }
        //        vm.EmployeeLeaveStructures = els;
        //        string[] retResults = new string[6];
        //        EmployeeLeaveDAL dal = new EmployeeLeaveDAL();
        //        retResults = dal.UpdateLeaveBalance(vm, null, null);
        //        if (retResults[0] == "Fail")
        //        {
        //            return false;

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    finally
        //    {
        //        theConnection.Close();

        //    }

        //    return true;
        //}
        //public bool ExportExcelFile(System.Data.DataTable dt, string Filepath, string FileName)
        //{
        //    _Application app = new Application();
        //    _Workbook workbook = app.Workbooks.Add(System.Type.Missing);
        //    _Worksheet worksheet = new Worksheet();
        //    app.Visible = false;

        //    worksheet = workbook.Sheets["Sheet1"] as _Worksheet;
        //    worksheet = workbook.ActiveSheet as _Worksheet;
        //    worksheet.Name = "DataSheet";

        //    #region DataRead From DB

        //    int startRow = 1;
        //    int colnum = 1;
        //    string data = null;
        //    int i = 0;
        //    int j = 0;

        //    foreach (DataColumn column in dt.Columns)
        //    {
        //        worksheet.Cells[startRow, colnum] = column.ColumnName;
        //        colnum++;
        //    }

        //    for (i = 0; i <= dt.Rows.Count - 1; i++)
        //    {
        //        for (j = 0; j <= dt.Columns.Count - 1; j++)
        //        {
        //            data = dt.Rows[i].ItemArray[j].ToString();
        //            worksheet.Cells[startRow + 1, j + 1] = data;
        //        }
        //        startRow++;
        //    }


        //    #endregion
        //    string xportFileName = string.Format(@"{0}" + FileName, Filepath);

        //    // save the application
        //    workbook.SaveAs(xportFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //                    Type.Missing,
        //                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing,
        //                    Type.Missing, Type.Missing, Type.Missing);

        //    // Exit from the application
        //    app.Quit();
        //    releaseObject(worksheet);
        //    releaseObject(workbook);
        //    releaseObject(app);
        //    return true;
        //}
        //private void releaseObject(object obj)
        //{

        //    try
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        //        obj = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        string exMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
        //                        ex.StackTrace;

        //        }
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}
    }
}
