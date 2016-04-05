<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="UserMembership.aspx.cs" Inherits="UserMembership" Title="Untitled Page" %>
<%@ Register TagPrefix="wiki" TagName="WikiSiteNav" Src="Controls\WikiSiteNav.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">    
    <asp:GridView ID="grdUsers" runat="server" DataKeyNames="UserName" AllowPaging=True AllowSorting=True AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" GridLines="None" OnPageIndexChanging="grdUsers_PageIndexChanging" OnSelectedIndexChanged="grdUsers_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="UserName" HeaderText="UserName" SortExpression="UserName" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:CheckBoxField DataField="IsApproved" HeaderText="Active" SortExpression="IsApproved" />
            <asp:BoundField DataField="CreationDate" HeaderText="Create Date" SortExpression="CreationDate" />
            <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login Date" SortExpression="LastLoginDate" />
            <asp:BoundField DataField="Comment" HeaderText="Comments" SortExpression="Comment" />                        
            <asp:ButtonField Text="Edit Details" CommandName="select" />            
        </Columns>
    </asp:GridView>
    
    <asp:Panel ID=pnlUserDetails runat=server>
        <table>
            <tr>
                <td valign=top>
                    <asp:DetailsView DefaultMode=Edit ID=dtvUsers runat=server AutoGenerateRows="False" CellPadding="4"                         
                        GridLines="None" OnItemDeleting="dtvUsers_ItemDeleting" OnItemUpdating="dtvUsers_ItemUpdating" HeaderText="Edit User Details" OnItemCommand="dtvUsers_ItemCommand" OnModeChanging="dtvUsers_ModeChanging" >
                        <Fields>                       
                            <asp:BoundField DataField="UserName" HeaderText="UserName" SortExpression="UserName" ReadOnly=True />
                            <asp:CheckBoxField DataField="IsApproved" HeaderText="Active" SortExpression="IsApproved" />                
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />                                    
                            <asp:TemplateField HeaderText="Comments">
                                <ItemTemplate>
                                    <asp:TextBox id=txtUserComments runat=server Rows=5 TextMode=MultiLine Text='<%# Bind("Comment") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>             
                            <asp:ButtonField ButtonType=Button CommandName="update" Text="Update" />
                            <asp:ButtonField ButtonType=Button CommandName="delete" Text="Delete" />  
                            <asp:ButtonField ButtonType=button CommandName="cancel" Text="Cancel" />              
                        </Fields>
                    </asp:DetailsView>
                </td>
                <td valign=top>
                    <asp:DataList SkinID="RolesList" ID=dlstUserRoles runat=server CellPadding="4" ForeColor="#333333">
                        <HeaderTemplate>
                            Edit User Roles
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID=chkRole runat=server Checked='<%# (Eval("IsInRole").ToString() == "true") ? true: false  %>' Text='<%# Eval("RoleName") %>' />
                        </ItemTemplate>
                    </asp:DataList>
                </td>
            </tr>        
        </table>
    </asp:Panel>

</asp:Content>
