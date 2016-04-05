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
using ProntoWiki;

public partial class Tags : WikiBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string tag = "";
        string page = "";
        string user = "";
        if (Request["t"] != null) tag = Request["t"].ToString();
        if (Request["page"] != null) page = Request["page"].ToString();
        if (Request["u"] != null) user = Request["u"].ToString();

        if (tag != "") //page selection is for a tag by default, even if both specified.
        {
            rptTags.Visible = false;
            lblTagHeader.Text = "Tag:  " + tag;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("w_GetPageNamesByTag", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag;
                cmd.Parameters.Add("@MaxFontSize", SqlDbType.Int).Value = 20;
                cmd.Parameters.Add("@MinFontSize", SqlDbType.Int).Value = 3;
                SqlDataReader reader = null;
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    rptPages.DataSource = reader;
                    rptPages.DataBind();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            }
        }
        else if (page != "") //page
        {
            rptPages.Visible = false;
            lblTagHeader.Text = "Page:  " + page;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("w_GetTagsByPage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = page;
                cmd.Parameters.Add("@MaxFontSize", SqlDbType.Int).Value = 20;
                cmd.Parameters.Add("@MinFontSize", SqlDbType.Int).Value = 3;
                SqlDataReader reader = null;
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    rptTags.DataSource = reader;
                    rptTags.DataBind();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            }
        }
        else if (user != "")
        {
            rptPages.Visible = false;
            lblTagHeader.Text = "Tags for user:  " + user;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("w_GetTagsByUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = user;
                cmd.Parameters.Add("@MaxFontSize", SqlDbType.Int).Value = 20;
                cmd.Parameters.Add("@MinFontSize", SqlDbType.Int).Value = 3;
                SqlDataReader reader = null;
                try
                {
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    rptTags.DataSource = reader;
                    rptTags.DataBind();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

            }
        }//else do nothing

    }
    protected void btnPageGo_Click(object sender, EventArgs e)
    {
            Response.Redirect("Tags.aspx?page=" + HttpUtility.UrlEncode(txtPage.Text), false);
    }

    protected void btnTagGo_Click(object sender, EventArgs e)
    {
        Response.Redirect("Tags.aspx?t=" + HttpUtility.UrlEncode(txtTag.Text), false);
    }

    protected void btnUserGo_Click(object sender, EventArgs e)
    {
        Response.Redirect("Tags.aspx?u=" + HttpUtility.UrlEncode(txtUser.Text), false);
    }
}
