<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopTen.ascx.cs" Inherits="ProntoWiki.Controls.TopTen" %>
<asp:Repeater ID=rptTopTen runat=server>
    <ItemTemplate>
        <a href="<%# "default.aspx?page=" + HttpUtility.UrlEncode(Eval("PageName").ToString()) %>"><%# Eval("PageName") %></a>
        
        <br />
    </ItemTemplate>
</asp:Repeater>