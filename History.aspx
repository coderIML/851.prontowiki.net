<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="History.aspx.cs" Inherits="ProntoWiki.History" Title="Untitled Page" %>
<%@ Register TagPrefix="wiki" TagName="WikiSiteNav" Src="Controls\WikiSiteNav.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:Label ID=lblHeader runat=server></asp:Label>
<asp:Button ID=btnPage runat=server Text="Page View" OnClick="btnPage_Click" />
<asp:Button ID=btnDiff runat=server Text="Show Differences" OnClick="btnDiff_Click"/>
<asp:Label ID=lblDiffError runat=server CssClass=errormessage>Please select two versions using the radio buttons, first</asp:Label>
<br />
<asp:GridView ID=grdHistory runat=server AutoGenerateColumns=false OnSelectedIndexChanged="grdHistory_SelectedIndexChanged">
    <Columns>
        <asp:BoundField DataField="Version" HeaderText="Version" />
        <asp:BoundField DataField="ChangedBy" HeaderText="Created By" />
        <asp:BoundField DataField="CreatedTime" HeaderText="Created Time" />
        <asp:BoundField DataField="HitCount" HeaderText="Cumulative Hits" />
        <asp:TemplateField>
            <ItemTemplate>
                <input type=radio name="diff1" value='<%# Eval("Version") %>' />
            </ItemTemplate>            
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <input type=radio name="diff2" value='<%# Eval("Version") %>' />
            </ItemTemplate>            
        </asp:TemplateField>
        <asp:ButtonField ButtonType=link CommandName=Select Text="View" />
    </Columns>
</asp:GridView>

<asp:DetailsView ID=detailsHistory runat=server>
    <Fields>
        <asp:BoundField DataField="PageView" />
    </Fields>
</asp:DetailsView>

<asp:Panel ID=pnlPageText runat=server Visible=false>
    <asp:Label ID=lblVersionHeader runat=server></asp:Label>
    <asp:Button ID=btnPromote runat=server Text="Promote" OnClick="btnPromote_Click" />
    <asp:Button ID=btnClose runat=server Text="Close" OnClick="btnClose_Click" />
    <asp:Literal ID=litPageHistoryView runat=server></asp:Literal></asp:Panel>

</asp:Content>

