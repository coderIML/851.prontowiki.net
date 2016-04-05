<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WikiLogin.ascx.cs" Inherits="ProntoWiki.Controls.WikiLogin" %>
<asp:Login ID=login1 runat=server Title="Sign In" VisibleWhenLoggedIn="True"><LoginButtonStyle CssClass=loginbutton /></asp:Login>
<br />
New User? <a href="User.aspx">Register here</a>.