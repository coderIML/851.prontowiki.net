<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="ProntoWiki.Controls.Search" %>
<asp:Panel runat=server ID=pnlSearch DefaultButton="btnSearch">
    <table>
        <tr>
            <td align=center>
                Please enter text to search in the Wiki:
                <br />
                <asp:TextBox ID=txtSearch runat=server></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td align=right>
                <asp:Button ID=btnSearch runat=server Text="Search" OnClick="btnSearch_Click" />
            </td>
        </tr>    

    </table>
</asp:Panel>