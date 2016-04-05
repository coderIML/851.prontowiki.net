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
    public partial class SearchPage : WikiBasePage
    {

        string query = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["query"] != null)
                query = HttpUtility.UrlDecode(Request.Params["query"].ToString());
            else
                query = "";

            WikiSiteNav nav1 = (WikiSiteNav)Master.FindControl("nav1");
            try
            {
                nav1.Add(""); //add default page to nav
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            if (!IsPostBack)
                BindResultsGrid();

        }

        public void BindResultsGrid()
        {
            grdSearch.DataSource = GetResults();                
            grdSearch.DataBind();
        }

        public DataSet GetResults()
        {
            query = "%" + query + "%";

            using (
                SqlDataAdapter da = new SqlDataAdapter(
                    "w_SearchSimpleTerm",
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString
                ))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (query != "") da.SelectCommand.Parameters.Add("@parameter", SqlDbType.VarChar).Value = query;
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "SearchResults");
                    //show only the first part of the page text
                    string pageText = "";

                    foreach (DataRow row in ds.Tables["SearchResults"].Rows)
                    {
                        pageText = "\n" + row["PageText"].ToString();
                        row["PageText"] = (pageText.Length > 100) ? pageText.Substring(0, 100) : pageText.Substring(0, pageText.Length);
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                    return null;
                }
                return ds;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("search.aspx?query={0}", HttpUtility.UrlEncode(txtSearch.Text)));
        }
        protected void grdSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx?page=" + HttpUtility.UrlEncode( (((GridView)sender).SelectedValue).ToString()));
        }
        protected void grdSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdSearch.DataSource = GetResults();
            grdSearch.PageIndex = e.NewPageIndex;
            grdSearch.DataBind();
        }
}
}
