<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Diff.aspx.cs" Inherits="Diff" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>Differences in version <asp:Label runat=server ID=lblVersion1></asp:Label> from version
    <asp:Label runat=server ID=lblVersion2></asp:Label> of <asp:LinkButton runat=server ID=lnkPage OnClick="lnkPage_Click"></asp:LinkButton></h1>
    <br />
    <table width=90% runat=server id=tblMain>
        <tr>
            <td width=100%>
                <span class=textmissing>Old text is in red.</span>
                <span class=textadded>New text is in green.</span>
                <hr />
            </td>
        </tr>               
        <tr>
           <td width=100% valign=top>
                <asp:Literal runat=server ID=litVersion1></asp:Literal>
            </td>       
        </tr>
    
    </table>
</asp:Content>

