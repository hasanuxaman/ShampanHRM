<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/HRM/Views/Shared/_MasterPage.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        SymRepository.Acc.ProductRepo _repo = new SymRepository.Acc.ProductRepo();
       
    }

    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e){
    
      
    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Create1
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
   <form id="form1" runat="server">
       <asp:FileUpload ID="FileUpload1" runat="server" />
       <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
       <asp:GridView ID="GridView1" runat="server" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
       </asp:GridView>
 
    
       
       </form>
</asp:Content>
