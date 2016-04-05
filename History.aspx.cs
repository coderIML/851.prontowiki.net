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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ProntoWiki.Controls;

namespace ProntoWiki
{

    public partial class History : WikiBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            pageName = GetPageName();

            WikiSiteNav nav1 = (WikiSiteNav)Master.FindControl("nav1");

            try
            {
                nav1.Add(pageName);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            lblHeader.Text = "<H1>History: " + pageName + "</H1>";
            if (!IsPostBack)
            {
                BindHistoryGrid();

            }

            lblDiffError.Visible = false;

        }

        void BindHistoryGrid()
        {
            using (SqlConnection conn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("w_GetHistory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                SqlDataReader reader;

                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    grdHistory.DataSource = reader;
                    grdHistory.DataBind();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }
        protected void grdHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlPageText.Visible = true;


            //TODO:  someone can currently promote the current page - needed if promoting a deleted page
            // but otherwise will create a redundant version.
            if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
                btnPromote.Visible = true;
            else
                btnPromote.Visible = false;



            using (SqlConnection conn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("w_GetPageByVersion", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                int version = int.Parse(((GridView)sender).SelectedRow.Cells[0].Text);
                cmd.Parameters.Add("@Version", SqlDbType.Int).Value = version;
                SqlDataReader reader;

                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    lblVersionHeader.Text = string.Format("<br><H2>Page History for {0}, v{1}</H2><br>", pageName, version);
                    litPageHistoryView.Text = WikiParser.GenerateWikiText(reader["PageText"].ToString());
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            pnlPageText.Visible = false;
        }
        protected void btnPromote_Click(object sender, EventArgs e)
        {
            //Make the selected entry the current one.
            using (SqlConnection conn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                int version = int.Parse(grdHistory.SelectedRow.Cells[0].Text);

                //get the selected version information
                SqlCommand cmdVersion = new SqlCommand("w_GetPageByVersion", conn);
                cmdVersion.CommandType = CommandType.StoredProcedure;
                cmdVersion.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                cmdVersion.Parameters.Add("@Version", SqlDbType.Int).Value = version;

                //check that the page is not deleted
                SqlCommand cmdGetPage = new SqlCommand("w_GetPage", conn);
                cmdGetPage.CommandType = CommandType.StoredProcedure;
                cmdGetPage.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                cmdGetPage.Parameters.Add("@IncrementHitCount", SqlDbType.Bit).Value = 0;

                //make it the current version
                SqlCommand cmdUpdate = new SqlCommand("w_UpdatePage", conn);
                cmdUpdate.CommandType = CommandType.StoredProcedure;
                cmdUpdate.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                cmdUpdate.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = User.Identity.Name;

                //if the page was deleted we will need to insert instead of update
                SqlCommand cmdInsert = new SqlCommand("w_InsertPage", conn);
                cmdInsert.CommandType = CommandType.StoredProcedure;
                cmdInsert.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                cmdInsert.Parameters.Add("@User", SqlDbType.NVarChar).Value = Context.User.Identity.Name;

                SqlDataReader versionReader, getReader;

                try
                {
                    conn.Open();
                    versionReader = cmdVersion.ExecuteReader();
                    versionReader.Read();
                    string pageText = versionReader["PageText"].ToString();
                    versionReader.Close();

                    getReader = cmdGetPage.ExecuteReader();
                    bool isCurrentPage = getReader.Read();
                    getReader.Close();
                    if (isCurrentPage) //if the page exists, update it
                    {
                        cmdUpdate.Parameters.Add("@PageText", SqlDbType.Text).Value = pageText;
                        cmdUpdate.ExecuteNonQuery();
                    }
                    else  //the page was deleted, insert it
                    {
                        cmdInsert.Parameters.Add("@PageText", SqlDbType.Text).Value = pageText;
                        cmdInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }

            pnlPageText.Visible = false;
            grdHistory.SelectedIndex = -1;
            BindHistoryGrid();

        }

        protected void btnPage_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx?page=" + HttpUtility.UrlEncode(pageName));
        }

        protected void btnDiff_Click(object sender, EventArgs e)
        {
            if (Request.Form["diff1"] != null && Request.Form["diff1"] != null)
                Response.Redirect("diff.aspx?page=" + HttpUtility.UrlEncode(pageName) + "&v1=" + Request.Form["diff1"] + "&v2=" + Request.Form["diff2"]);
            else
                lblDiffError.Visible = true;
        }
}
}