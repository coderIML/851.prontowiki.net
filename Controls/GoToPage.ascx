<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoToPage.ascx.cs" Inherits="ProntoWiki.Controls.GoToPage" %>
<asp:Panel runat=server ID=pnlGoTo DefaultButton="btnGoTo">
    <table>
        <tr>
            <td align=center>
                Enter a Page Name:
                <br />
                <asp:TextBox ID=txtGoTo runat=server></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td align=right>
                <asp:Button ID=btnGoTo runat=server Text="Go" OnClick="btnGoTo_Click" />
            </td>
        </tr>    

    </table>
</asp:Panel>