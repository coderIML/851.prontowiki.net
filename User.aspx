<%@ Page Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="User.aspx.cs" Inherits="ProntoWiki.User" Title="Untitled Page" %>
<%@ Register TagPrefix="wiki" TagName="WikiSiteNav" Src="Controls\WikiSiteNav.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table>

        <tr>
            <td valign=top>
                <asp:Panel runat=server ID=pnlLogin DefaultButton="Login1$LoginButton">
                    <asp:Login SkinID="userLogin"  ID=Login1  runat=server VisibleWhenLoggedIn=False  Font-Names="Verdana" >
                    </asp:Login>  
                </asp:Panel>       
                <asp:Panel runat=server ID=pnlChangePassword DefaultButton="ChangePassword1$ChangePasswordContainerID$ChangePasswordPushButton">
                    <asp:ChangePassword ID="ChangePassword1" runat="server" Visible=False  ContinueDestinationPageUrl="~/Default.aspx" Font-Names="Verdana" CancelDestinationPageUrl="~/Default.aspx" >
                    </asp:ChangePassword>
                </asp:Panel>   
            </td>
            <td valign=top>
                <asp:Panel ID=pnlRecovery runat=server DefaultButton="PasswordRecovery1$UserNameContainerID$SubmitButton" >
                    <asp:PasswordRecovery  ID="PasswordRecovery1" runat="server" >
                    </asp:PasswordRecovery>
                </asp:Panel>
            </td>
            <td valign=top>
                <asp:Panel ID=pnlCreateUser runat=server DefaultButton="CreateUserWizard1$__CustomNav0$StepNextButtonButton">
                    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" OnCreatedUser="CreateUserWizard1_CreatedUser" ContinueDestinationPageUrl="~/Default.aspx"  >
                        <WizardSteps>
                            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                            </asp:CreateUserWizardStep>
                            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                            </asp:CompleteWizardStep>
                        </WizardSteps>
                    </asp:CreateUserWizard>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>

