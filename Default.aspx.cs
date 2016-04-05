/*
Copyright (c) 2005, Clay Alberty
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions 
      and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
      and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of Clay Alberty nor the names of his contributors may be used to endorse or 
      promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
OF SUCH DAMAGE.
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ProntoWiki.Controls;

namespace ProntoWiki
{
    public partial class _Default : WikiBasePage
    {
        private DateTime wikiStartTime;
        private DateTime wikiEndTime;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            pageName = GetPageName();
            lblHeader.Text = pageName;

            WikiSiteNav nav1 = (WikiSiteNav)Master.FindControl("nav1");  
                                 
            try
            {
                nav1.Add(pageName);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            if (!IsPostBack)
            {
                BindPageText(true);
            }

            PerformSecurityChecks();                        

        }

        protected void PerformSecurityChecks()
        {
            if (!Context.User.IsInRole("Editor"))
            {
                btnCreate.Visible = false;
                if (DataList1.Items.Count > 0)
                {
                    object o = DataList1.Items[0].FindControl("pnlEditButton");
                    if (o != null)
                        ((Panel)o).Visible = false;
                }
            }

            //if they aren't in the Reader role and anonymous access is not allowed, send to default page as specified in web.config
            if (!Context.User.IsInRole("Reader"))
            {
                if (ConfigurationManager.AppSettings["anonymousAccess"].ToUpper() != "TRUE" &&
                    pageName != ConfigurationManager.AppSettings["defaultPage"])
                {
                    Response.Redirect(string.Format("default.aspx?page={0}", ConfigurationManager.AppSettings["defaultPage"]));
                }
            }

            if (!User.Identity.IsAuthenticated)
            {
                //show page tag controls
                if (DataList1.Items.Count > 0)
                {
                    object o = DataList1.Items[0].FindControl("pnlTags");
                    if (o != null)
                        ((Panel)o).Visible = false;
                }
            }
        }

        protected void BindPageText(bool FormatWikiText)
        {
            DataSet ds = GetPageText(FormatWikiText);

            //if we got a page, display it
            if (ds != null)
            {
                pnlPageNotFound.Visible = false;
                pnlNewText.Visible = false;
                pnlAttachments.Visible = false;
                pnlQuickRef.Visible = false;

                DataList1.DataSource = ds;
                DataList1.DataBind();
            }
            else //otherwise offer to create it
            {
                DataList1.Visible = false;
                pnlNewText.Visible = false;
                pnlQuickRef.Visible = false;
                pnlAttachments.Visible = false;
                pnlPageNotFound.Visible = true;
            }

        }

        protected DataSet GetPageText(bool FormatWikiText)
        {
            using (
                SqlDataAdapter da = new SqlDataAdapter(
                    "w_GetPage",
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString
                ))
            {

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                da.SelectCommand.Parameters.Add("@IncrementHitCount", SqlDbType.Bit).Value = Page.IsPostBack ? 0 : 1;

                DataSet ds = new DataSet();
                da.Fill(ds, "Wiki");
                if (ds.Tables["Wiki"] != null && ds.Tables["Wiki"].Rows.Count > 0)
                {
                    try
                    {
                        //Get the links from the table
                        da.SelectCommand.CommandText = "w_GetLinks";
                        da.SelectCommand.Parameters.Clear();
                        da.Fill(ds, "Links");

                        //Add the wiki formatted text to the dataset
                        ds.Tables["Wiki"].Columns.Add("PageView");
                        wikiStartTime = System.DateTime.Now;
                        if (FormatWikiText)
                            ds.Tables["Wiki"].Rows[0]["PageView"] = WikiParser.GenerateWikiText(ds.Tables["Wiki"].Rows[0]["PageText"].ToString(), ds.Tables["Links"]);
                        wikiEndTime = System.DateTime.Now;
                        if (ConfigurationManager.AppSettings["displayPageStats"].ToString().ToLower() == "true")
                            lblPageStats.Text = string.Format("Wiki formatted text generation: {0}", wikiEndTime - wikiStartTime);                        
                        return ds;
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex);
                        return null;
                    }
                }
                else
                    return null;

            }
        }

        protected void login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            Session["Login"] = true;
            Response.Redirect(Request.Url.ToString());
        }

        protected void DataList1_EditCommand(object source, DataListCommandEventArgs e)
        {
            DataList1.EditItemIndex = (int)e.Item.ItemIndex;
            BindPageText(false);
            ShowFileList();

            pnlQuickRef.Visible = true;
            
            Button b = (Button)(DataList1.Items[0].FindControl("btnDelete"));
            if (!Context.User.IsInRole("Administrator"))
            {
                //hide the delete button if user does not have rights
                if (DataList1.Items.Count > 0)
                {
                    if (b != null)
                    {
                        b.Visible = false;
                    }
                }
            }

            AddEditModeButtonAttributes();
           
        }

        protected void AddEditModeButtonAttributes()
        {
            Button b = (Button)(DataList1.Items[0].FindControl("btnDelete"));

            if (b != null && b.Visible) 
                b.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this page?');");

            //add confirmation message to cancel button
            b = (Button)(DataList1.Items[0].FindControl("btnCancel"));
            if (b != null && b.Visible) 
                b.Attributes.Add("onclick", "return confirm('Are you sure you want to discard changes to the page text?');");

            //add js functions to file grid
            GridViewRow row;
            LinkButton l;
            if (grdAttachments.Visible)
            {
                for (int i = 0; i < grdAttachments.Rows.Count; i++)
                {
                    row = grdAttachments.Rows[i];
                    l = (LinkButton)(row.Cells[4].Controls[0]);
                    l.Attributes.Add("onclick", "return confirm('Deleting this file will permanently remove it from the system and break any associated links.');");

                    l = (LinkButton)(row.Cells[3].Controls[0]);
                    l.Attributes.Add("onclick", string.Format("InsertFile('{0}','{1}');return false;", row.Cells[2].Text, row.Cells[0].Text));
                }
            }
        }

        protected void DataList1_CancelCommand(object source, DataListCommandEventArgs e)
        {
            DataList1.EditItemIndex = -1;
            BindPageText(true);
            pnlPreview.Visible = false;
            pnlQuickRef.Visible = false;
            ResetFileList();
        }

        protected void DataList1_UpdateCommand(object source, DataListCommandEventArgs e)
        {
            //prevent changes if the user has logged out and used the back button or if the session expired
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    throw new Exception("Session has expired.  Please sign in to make further changes");
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                return;
            }

            DataList d = (DataList)source;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                try
                {
                    SavePageText(conn, d, e);
                    SaveAttachments(conn);
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            }

            DataList1.EditItemIndex = -1;
            BindPageText(true);
            ResetFileList();
            pnlPreview.Visible = false;
            pnlQuickRef.Visible = false;
        }

        protected void SavePageText(SqlConnection conn, DataList d, DataListCommandEventArgs e)
        {
            try
            {

                SqlCommand cmd = new SqlCommand("w_UpdatePage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PageText", SqlDbType.Text);

                //check length
                string pageText = ((TextBox)DataList1.Items[0].FindControl("txtPageText")).Text;
                int maxPageLength = Convert.ToInt32(ConfigurationManager.AppSettings["maxPageLength"]);
                if (maxPageLength > 0)
                {
                    if (pageText.Length > maxPageLength)
                    {
                        cmd.Parameters["@PageText"].Value = pageText.Substring(0, maxPageLength);
                        Response.Write(@"<table width=100% CellPadding=0 CellSpacing=0><tr><td class=""errormessage"">" +
                            "Warning - page text was truncated to maximum page length of " + maxPageLength.ToString() +
                            " bytes.</td></tr></table>");
                    }
                    else
                    {
                        cmd.Parameters["@PageText"].Value = pageText;
                    }
                }
                else
                {
                    cmd.Parameters["@PageText"].Value = pageText;
                }

                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = d.DataKeys[e.Item.ItemIndex].ToString();
                cmd.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = Context.User.Identity.Name.ToString();

                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        protected void SaveAttachments(SqlConnection conn)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(
                            "Insert into Attachments (PageName, AttachmentName, AttachmentData, Extension, ChangedBy) VALUES " +
                            "(@PageName, @AttachmentName, @AttachmentData, @Extension, @ChangedBy)", conn);
                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                cmd.Parameters.Add("@AttachmentName", SqlDbType.VarChar);
                cmd.Parameters.Add("@AttachmentData", SqlDbType.Image);
                cmd.Parameters.Add("@Extension", SqlDbType.VarChar);
                cmd.Parameters.Add("@ChangedBy", SqlDbType.NVarChar).Value = User.Identity.Name;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                foreach (RepeaterItem i in rptAttachments.Items)
                {
                    FileUpload f = (FileUpload)(i.FindControl("fileAttachments"));
                    if (f != null && f.FileContent.Length > 0)
                    {
                        //insert a row into the attachments table
                        cmd.Parameters["@AttachmentName"].Value = f.FileName;
                        cmd.Parameters["@AttachmentData"].Value = f.FileBytes;
                        string[] temp = f.FileName.Split('.');
                        cmd.Parameters["@Extension"].Value = temp[temp.Length - 1];
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        protected void btnCreate_Click(object sender, EventArgs e)
        {
            pnlNewText.Visible = true;
            pnlPageNotFound.Visible = false;
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("w_InsertPage", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PageText", SqlDbType.Text);
                    
                    //limit the page length if maxPageLength set in config file
                    int maxPageLength = Convert.ToInt32(ConfigurationManager.AppSettings["maxPageLength"]);
                    if (maxPageLength > 0)
                    {
                        if (txtNewPage.Text.Length > maxPageLength)
                        {
                            cmd.Parameters["@PageText"].Value = txtNewPage.Text.Substring(0,maxPageLength);
                            Response.Write(@"<table width=100% CellPadding=0 CellSpacing=0><tr><td class=""errormessage"">" +
                                "Warning - page text was truncated to maximum page length of " + maxPageLength.ToString() + 
                                " bytes.</td></tr></table>");
                        }
                        else
                        {
                            cmd.Parameters["@PageText"].Value = txtNewPage.Text;
                        }
                    }
                    else
                    {
                        cmd.Parameters["@PageText"].Value = txtNewPage.Text;
                    }
                    
                    cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                    cmd.Parameters.Add("@User", SqlDbType.NVarChar).Value = Context.User.Identity.Name;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            }

            DataList1.Visible = true;
            pnlNewText.Visible = false;
            pnlPageNotFound.Visible = false;
            pnlPreview.Visible = false;

            BindPageText(true);
        }
        protected void btnCancelInsert_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.ToString());
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            pnlPreview.Visible = true;
            
            //truncate if over max length
            int maxPageLength = Convert.ToInt32(ConfigurationManager.AppSettings["maxPageLength"]);
            string previewText = ((TextBox)DataList1.Items[0].FindControl("txtPageText")).Text; 
            if (maxPageLength > 0)
            {
                if (previewText.Length > maxPageLength)
                {
                    previewText = "! Warning - Results truncated to not exceed " + maxPageLength.ToString() + " bytes.\r\n" +
                        previewText.Substring(0, maxPageLength);
                }
            }

            litPreview.Text = WikiParser.GenerateWikiText(previewText);
        }

        protected void btnClosePreview_Click(object sender, EventArgs e)
        {
            pnlPreview.Visible = false;
        }

        protected void btnPreviewInsert_Click(object sender, EventArgs e)
        {
            pnlPreview.Visible = true;

            string previewText = txtNewPage.Text;
            int maxPageLength = Convert.ToInt32(ConfigurationManager.AppSettings["maxPageLength"]);
            if (maxPageLength > 0)
            {
                if (previewText.Length > maxPageLength)
                {
                    previewText = "! Warning - Results truncated to not exceed " + maxPageLength.ToString() + " bytes.\r\n" +
                        previewText.Substring(0, maxPageLength);
                }
            }

            litPreview.Text = WikiParser.GenerateWikiText(previewText);
        }

        protected void DataList1_DeleteCommand(object source, DataListCommandEventArgs e)
        {
            //prevent changes if the user has logged out and used the back button or if the session expired
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    throw new Exception("Session has expired.  Please sign in to make further changes");
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                return;
            }

            DataList d = (DataList)source;

            using (SqlConnection conn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("w_DeletePage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = d.DataKeys[e.Item.ItemIndex].ToString();
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }

            DataList1.EditItemIndex = -1;
            BindPageText(false);

        }

        protected void ShowFileList()
        {
            //add an arbitrary dataset to the repeater to create the file attachment list
            pnlAttachments.Visible = true;
            if (rptAttachments.Items.Count < 1)
            {                
                DataSet d = new DataSet();
                DataTable t = new DataTable("newfiles");
                t.Columns.Add("FileName", typeof(String));
                DataRow row;
                //I tried to dynamically allow the addition of files but it was messy. Added config option instead
                for (int i = 0; i < Convert.ToInt32(ConfigurationManager.AppSettings["numberOfAttachments"]); i++)
                {
                    row = t.NewRow();
                    t.Rows.Add(row);
                }                
                d.Tables.Add(t);

                rptAttachments.DataSource = d;                
                rptAttachments.DataBind();

            }            

        }

        protected void ResetFileList()
        {
            rptAttachments.DataSource = null;
            rptAttachments.DataBind();
            grdAttachments.DataBind();
        }

        protected void rptAttachments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            
            //upload files so a page version update is not required to get guids, etc.
            if (e.CommandName == "Upload")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    try
                    {
                        SaveAttachments(conn);
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex);
                    }

                }
                grdAttachments.DataBind();
            }
        }

        protected void grdAttachments_DataBound(object sender, EventArgs e)
        {
            //for some reason these were getting blown away after the delete event; finally found the right
            //place to hook them up.
            GridView grid = (GridView)sender;
            GridViewRow row;
            LinkButton l;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                row = grid.Rows[i];
                l = (LinkButton)(row.Cells[4].Controls[0]);
                l.Attributes.Add("onclick", "return confirm('Deleting this file will permanently remove it from the system and break any associated links.');");

                l = (LinkButton)(row.Cells[3].Controls[0]);
                l.Attributes.Add("onclick", string.Format("InsertFile('{0}','{1}');return false;", row.Cells[2].Text, row.Cells[0].Text));
            }
        }

    }
}
