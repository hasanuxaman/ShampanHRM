<%--<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewPage" %>--%>
<%@ Control Language="C#" AutoEventWireup="true"  Inherits="System.Web.Mvc.ViewUserControl" %>


<script runat="server">

    protected void AuthorsGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SymRepository.Acc.ProductRepo _repo = new SymRepository.Acc.ProductRepo();
        //AuthorsGridView.DataSource = Model;
        //AuthorsGridView.DataBind();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //SymRepository.Acc.ProductRepo _repo = new SymRepository.Acc.ProductRepo();
        //AuthorsGridView.DataSource = Model;
        //AuthorsGridView.DataBind();
    }
</script>


   <form id="form1" runat="server">
      <asp:gridview id="AuthorsGridView" 
        autogeneratecolumns="true" 
        runat="server" OnSelectedIndexChanged="AuthorsGridView_SelectedIndexChanged">
      </asp:gridview>
       </form>