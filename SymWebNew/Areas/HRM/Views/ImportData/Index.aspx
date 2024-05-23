<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/HRM/Views/Shared/_MasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = DateTime.Now.ToString();
        Loadbtn.Click += Loadbtn_Click;
        List<string> list = new List<string>()
         {
"EmployeeInfo"
,"EmployeeJob"
,"EmployeePersonalDetail"
,"EmployeePermanentAddress"
,"EmployeeAssets"
,"EmployeeEducation"
,"EmployeeNominee"
,"EmployeeDependent"
,"EmployeeEmergencyContact"
,"EmployeeJobHistory"
,"EmployeeExtraCurriculumActiviti"
,"EmployeeImmigration"
,"EmployeeLanguage"
,"EmployeePF"
,"EmployeePresentAddress"
,"EmployeeReference"
,"EmployeeTax"
,"EmployeeTraining"
,"EmployeeTravel"  
         };
        DropDownList1.DataSource = list;
        DropDownList1.DataBind();
    }
    protected void Loadbtn_Click(object sender, EventArgs e)
    {
        try
        {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileUpload1.FileName;
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            System.IO.FileStream stream = System.IO.File.Open(fullPath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            Excel.IExcelDataReader dataReader = null;
            if (fullPath.EndsWith(".xls"))
            {
                dataReader = Excel.ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (fullPath.EndsWith(".xlsx"))
            {
                dataReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                //Throw exception for things you cannot correct
            }
            System.Data.DataSet ds = new System.Data.DataSet();
            dataReader.IsFirstRowAsColumnNames = true;
            ds = dataReader.AsDataSet();
            var MembersDataSet = dataReader.AsDataSet();
            string droupdownname = DropDownList1.SelectedValue;
            var TempWorkSheet = MembersDataSet.Tables[droupdownname.ToString()];
            GridView1.DataSource = ds;
            GridView1.DataBind();
            
            dataReader.Close();
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void Savebtn_Click(object sender, EventArgs e)
    {

    }
</script>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <form id="form1" runat="server">


    <asp:Button ID="Loadbtn" runat="server" Text="Load" CssClass="sym-btn-save" OnClick="Loadbtn_Click" />
    <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:DropDownList ID="DropDownList1" CssClass="Dropdown" runat="server" AutoPostBack="True">
        </asp:DropDownList>
    <asp:Button ID="Savebtn" runat="server" Text="Save" OnClick="Savebtn_Click" />
        <asp:GridView ID="GridView1" CssClass="display" runat="server"></asp:GridView>
           <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    </form>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptsSection" runat="server">
</asp:Content>
