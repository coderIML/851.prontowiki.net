<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Tags.aspx.cs" Inherits="Tags" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1><asp:Label runat=server ID=lblTagHeader></asp:Label></h1>
    <table>
        <tr>
            <td>
                <asp:Panel ID=pnlPage runat=server DefaultButton="btnPageGo">Tags by page:  <asp:TextBox ID=txtPage runat=server></asp:TextBox>&nbsp;<asp:Button runat=server ID=btnPageGo Text=">" OnClick="btnPageGo_Click" />    </asp:Panel>        
            </td>
            <td>            
                <asp:Panel ID=pnlTag runat=server DefaultButton="btnTagGo">Pages by tag:  <asp:TextBox ID=txtTag runat=server></asp:TextBox>&nbsp;<asp:Button runat=server ID=btnTagGo Text=">" OnClick="btnTagGo_Click" /></asp:Panel>
            </td>
            <td>            
                <asp:Panel ID=Panel1 runat=server DefaultButton="btnUserGo">Tags by user:  <asp:TextBox ID=txtUser runat=server></asp:TextBox>&nbsp;<asp:Button runat=server ID=btnUserGo Text=">" OnClick="btnUserGo_Click" /></asp:Panel>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td width=100% align=center>
                <asp:Repeater ID=rptPages runat=server >                    
                    <ItemTemplate>                        
                        <span style="font-size:<%# Eval("WeightedScore").ToString() + "pt" %>;">&nbsp;<a href="default.aspx?page=<%# HttpUtility.UrlEncode(Eval("PageName").ToString()) %>" ><%# Eval("PageName") %></a>&nbsp;</span> 
                    </ItemTemplate>
                </asp:Repeater>      
                <asp:Repeater ID=rptTags runat=server >                    
                    <ItemTemplate>                        
                        <span style="font-size:<%# Eval("WeightedScore").ToString() + "pt" %>;">&nbsp;<a href="tags.aspx?t=<%# HttpUtility.UrlEncode(Eval("Tag").ToString()) %>" ><%# Eval("Tag") %></a>&nbsp;</span> 
                    </ItemTemplate>
                </asp:Repeater>       
            </td>
        </tr>
    </table>
</asp:Content>

