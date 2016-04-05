<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ProntoWiki._Default" Title="Untitled Page" validateRequest="false" %>
<%@ Register tagprefix="wiki" tagname="WikiSearch" src="controls\search.ascx" %>
<%@ Register TagPrefix="wiki" TagName="DisplayModeMenu" Src="Controls\DisplayModeMenu.ascx" %>
<%@ Register TagPrefix="wiki" TagName="WikiSiteNav" Src="Controls\WikiSiteNav.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">      
    
    <table cellpadding="10">
        <tr>
            <td width="100%" valign="top">
            <h1><asp:Label ID=lblHeader runat=server></asp:Label></h1>
                
                <asp:Panel ID=pnlPreview runat=server Visible="false">
                    <h3>Page Preview</h3><asp:Button ID=btnClosePreview runat=server Text="Close Preview" OnClick="btnClosePreview_Click" />
                    <br />
                    <asp:Literal runat=server ID=litPreview></asp:Literal>
                    <br />
                </asp:Panel> 
                
                <table>
                    <tr>
                        <td>
                    

                            <asp:DataList ID="DataList1" runat="server" DataKeyField="PageName"  Height="72px" OnEditCommand="DataList1_EditCommand" OnCancelCommand="DataList1_CancelCommand" OnUpdateCommand="DataList1_UpdateCommand" OnDeleteCommand="DataList1_DeleteCommand">                    
                                <ItemTemplate>                                                                                         
                                    <asp:Panel ID=pnlEditButton runat=server>
                                         <asp:Button ID=btnEdit runat=server Text="Edit Page" CommandName="edit" />
                                    </asp:Panel>
                               
                                    <asp:Panel ID=pnlPageText runat=server SkinID=PageText><asp:Literal ID=litPageText runat=server Text='<%# Eval("PageView") %>'></asp:Literal></asp:Panel>
                                    <br />
                                    <hr />
                                        <span class=menutextindent>
                                        Last Changed By: &nbsp; <asp:Label runat=server ID=lblCreatedBy Text='<%# Eval("ChangedBy") %>'></asp:Label>&nbsp; 
                                        on &nbsp;<asp:Label runat=server ID=lblVersionTime Text='<%# Eval("CreatedTime") %>' ></asp:Label>&nbsp;|&nbsp;
                                        Version: &nbsp; <asp:Label runat=server ID=lblVersion Text='<%# Eval("Version") %>'></asp:Label>&nbsp;|&nbsp;
                                        Hits: &nbsp; <asp:Label runat=server ID=lblHitCount Text='<%# Eval("HitCount") %>'></asp:Label>&nbsp;|&nbsp;
                                        <a href="<%# string.Format("history.aspx?page={0}",HttpUtility.UrlEncode(Eval("PageName").ToString())) %>">History</a>
                                        
                                        <br />
                                        <br />
                                        <script type="text/javascript">
                                            GetTags(GetPageName());
                                        </script>
                                        <table cellpadding=0 cellspacing=1>
                                            <tr>
                                                <td>
                                                    Tags:
                                                </td>
                                                <td>
                                                    <div id=divTagList></div>
                                                </td>
                                                <td>
                                                    <asp:Panel ID=pnlTags runat=server>
                                                        &nbsp;<input type=text id=txtTag value="" style="font-size:x-small;width:50px;"/>
                                                        <input type=button id=btnAddTag value="Tag" class=inputbutton onclick="AddTag(txtTag.value, GetPageName());" /> 
                                                    <input type=button id=btnDeleteTag value="UnTag" class=inputbutton onclick="DeleteTag(txtTag.value, GetPageName());" />                                                               
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
 
                                                
                                        </span>
                                    <br />                  

                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Button ID=btnSave runat=server Text="Save" CommandName="update" />
                                    <asp:Button ID=btnCancel runat=server Text="Cancel" CommandName="cancel" />
                                    <asp:Button ID=btnDelete runat=server Text="Delete" CommandName="delete" />
                                    <asp:Button ID=btnPreview runat=server Text="Preview" OnClick="btnPreview_Click" />
                                    <br />
                                    <asp:Panel ID=pnlNewText runat=server BorderWidth=1><asp:TextBox BorderStyle=none Columns=80 Rows=30 TextMode=MultiLine ID=txtPageText runat=server Text='<%# Eval("PageText") %>'></asp:TextBox></asp:Panel>
                                    <br />
                                </EditItemTemplate>
                            </asp:DataList>                  
                            &nbsp;
                        </td>
                        <td valign=top>
                            <asp:Panel ID=pnlQuickRef runat=server>                          
                                <h2>Quick Reference</h2>
                                <br />
                                <a href="Javascript:ApplyFormatting('italics',false);"><em>''Italics''</em></a><br />
                                <a href="Javascript:ApplyFormatting('underline',false);">__Underline__</a><br />
                                <a href="Javascript:ApplyFormatting('bold',false);"><b>**Bold**</b></a><br />
                                <a href="Javascript:ApplyFormatting('center',false);">=Center=</a><br />
                                <a href="Javascript:ApplyFormatting('h1',false);">!Heading1</a><br />
                                <a href="Javascript:ApplyFormatting('h2',false);">!!Heading2</a><br />
                                <a href="Javascript:ApplyFormatting('h3',false);">!!!Heading3</a><br />
                                <a href="Javascript:ApplyFormatting('hr',false);">----Line</a><br />
                                <a href="Javascript:ApplyFormatting('ul',false);">+Bullet List</a><br />
                                <a href="Javascript:ApplyFormatting('ol',false);">#Number List</a><br />
                                <a href="Javascript:ApplyFormatting('namedlink',false);">{Named Link}</a><br />
                                <a href="Javascript:ApplyFormatting('wikilink',false);">[Wiki Link]</a><br />
                                <a href="Javascript:ApplyFormatting('custom',false);">&lt;&lt;custom&gt;&gt;</a>
                            </asp:Panel>                                                          
                        </td>
                    </tr>
                
                </table>             
                
                <asp:Panel ID=pnlPageNotFound runat=server>
                    <h2><asp:Label runat=server ID=lblPageNotFound Text="We're sorry, this page could not be found in the wiki."></asp:Label></h2>
                    <asp:Button ID=btnCreate runat=server Text="Create Page" OnClick="btnCreate_Click" />
                </asp:Panel>
                
                <asp:Panel ID=pnlNewText runat=server>
                    <asp:Button ID=btnInsert runat=server Text="Insert Page" OnClick="btnInsert_Click" />
                    <asp:Button ID=btnCancelInsert runat=server Text="Cancel" OnClick="btnCancelInsert_Click" />
                    <asp:Button ID=btnPreviewInsert runat=server Text="Preview" OnClick="btnPreviewInsert_Click" />
                    <br />
                    <table>
                        <tr>
                            <td>
                                <asp:Panel ID=pnlNewTextBox runat=server BorderWidth=1>
                                    <asp:TextBox ID=txtNewPage runat=server Columns=80 Rows=30 TextMode=MultiLine BorderWidth=0></asp:TextBox>
                                </asp:Panel>
                            </td>
                            <td valign=top>                                                                          
                                <h2>Quick Reference</h2>
                                <br />
                                <a href="Javascript:ApplyFormatting('italics',true);"><em>''Italics''</em></a><br />
                                <a href="Javascript:ApplyFormatting('underline',true);">__Underline__</a><br />
                                <a href="Javascript:ApplyFormatting('bold',true);"><b>**Bold**</b></a><br />
                                <a href="Javascript:ApplyFormatting('center',true);">=Center=</a><br />
                                <a href="Javascript:ApplyFormatting('h1',true);">!Heading1</a><br />
                                <a href="Javascript:ApplyFormatting('h2',true);">!!Heading2</a><br />
                                <a href="Javascript:ApplyFormatting('h3',true);">!!!Heading3</a><br />
                                <a href="Javascript:ApplyFormatting('hr',true);">----Line</a><br />
                                <a href="Javascript:ApplyFormatting('ul',true);">+Bullet List</a><br />
                                <a href="Javascript:ApplyFormatting('ol',true);">#Number List</a><br />
                                <a href="Javascript:ApplyFormatting('namedlink',true);">{Named Link}</a><br />
                                <a href="Javascript:ApplyFormatting('wikilink',true);">[Wiki Link]</a><br />
                                <a href="Javascript:ApplyFormatting('custom',true);">&lt;&lt;custom&gt;&gt;</a>
                            </td>
                        </tr>
                    </table>
                            
                </asp:Panel>  
                
                <asp:Panel ID=pnlAttachments runat=server>
                    <h2>File Attachments</h2>
                    <asp:GridView ID=grdAttachments runat=server DataKeyNames="AttachmentID" AutoGenerateColumns=False DataSourceID="sqlAttachments" OnDataBound="grdAttachments_DataBound">
                        <Columns>
                            <asp:BoundField DataField="AttachmentName" HeaderText="File Name" />
                            <asp:BoundField DataField="ChangedBy" HeaderText="Attached By" />
                            <asp:BoundField DataField="AttachmentID" HeaderText="Identifier" />
                            <asp:ButtonField Text="Insert" />
                            <asp:ButtonField Text="Delete" CommandName="Delete" />
                        </Columns>                    
                    </asp:GridView>
                    
                    <asp:Repeater ID=rptAttachments runat=server OnItemCommand="rptAttachments_ItemCommand">
                        <ItemTemplate>
                            <asp:FileUpload ID=fileAttachments runat=server /><br /> 
                        </ItemTemplate>
                        <FooterTemplate>
                            <br />                                                       
                            <asp:Button ID=btnUpload runat=server CommandName="Upload" Text="Upload Files" />
                        </FooterTemplate>
                    </asp:Repeater>                                       
                    
                    <asp:SqlDataSource ID=sqlAttachments runat=server
                        SelectCommand="Select * from Attachments Where PageName=@PageName order by AttachmentName"
                        DeleteCommand="Delete From Attachments Where AttachmentID = @AttachmentID" ConnectionString="<%$ ConnectionStrings:ConnectionString %>">
                        <DeleteParameters>
                            <asp:ControlParameter ControlID="grdAttachments" Name="AttachmentID" PropertyName="SelectedDataKey" />
                        </DeleteParameters>
                        <SelectParameters>
                            <asp:QueryStringParameter Name="PageName" QueryStringField="page" DefaultValue='<%$ appSettings:defaultPage %>' />
                        </SelectParameters>
                    </asp:SqlDataSource>                
                </asp:Panel>                                             
            </td>
        </tr>
    </table>
    <asp:Label ID=lblPageStats runat=server></asp:Label>
</asp:Content>

