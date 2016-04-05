<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="ProntoWiki.SearchPage" Title="Untitled Page" %>
<%@ Register TagPrefix="wiki" TagName="WikiSiteNav" Src="Controls\WikiSiteNav.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    &nbsp;<asp:TextBox ID=txtSearch runat=server></asp:TextBox><asp:Button ID=btnSearch runat=server OnClick="btnSearch_Click" Text="Search" />
    <br />
    <br />
    <asp:Label ID=lblNoResults runat=server Visible=false></asp:Label>
    <asp:GridView ID="grdSearch" EmptyDataText="<h2>Sorry, no results were found</h2>"
        runat="server" AllowPaging=true DataKeyNames="PageName" 
        OnSelectedIndexChanged="grdSearch_SelectedIndexChanged" OnPageIndexChanging="grdSearch_PageIndexChanging">
        <Columns>
            <asp:ButtonField ButtonType=link CommandName="Select" DataTextField="PageName" HeaderText="Page"  />
            <asp:BoundField DataField="PageText" HeaderText="Text" />
        </Columns>                
    </asp:GridView>
</asp:Content>

