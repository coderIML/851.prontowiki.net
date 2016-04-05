<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisplayModeMenu.ascx.cs" Inherits="ProntoWiki.Controls.DisplayModeMenu" %>
<div>
  <asp:Panel ID="Panel1" runat="server" 
    SkinID=pnlWebPartDisplay Width=0>
    <asp:Label ID="Label1" runat="server" 
      Text="&nbsp;Display Mode" 
      Font-Bold="true"
      Font-Size="8" />
    <div>
    <asp:DropDownList ID="DisplayModeDropdown" runat="server"  
      AutoPostBack="true"       
      OnSelectedIndexChanged="DisplayModeDropdown_SelectedIndexChanged" />
    </div>
  </asp:Panel>
</div>