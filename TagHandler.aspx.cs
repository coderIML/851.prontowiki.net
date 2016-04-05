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

/// <summary>
/// Server-side handler for ajax tag requests
/// </summary>
public partial class TagHandler : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["Action"] != null && Request["PageName"] != null)
        {
            string result = PerformTagAction();
            if (result != null)
            {
                Response.Write(result);
            }
            Response.End();
            
            
        }        
    }

    private string PerformTagAction()
    {
        string action = Request["Action"].ToString();
        string pageName = Request["PageName"].ToString();
        string tag = "";
        if (Request["Tag"] != null)
            tag = Request["Tag"].ToString();
        string results = "";
   
        SqlCommand cmd = new SqlCommand();

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
        {
            SqlDataReader reader = null;
            try
            {
                cmd.Connection = conn;
                conn.Open();

                switch (action)
                {
                    case "GetAllTags":
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "w_GetTags";
                        cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                        cmd.Parameters.Add("@UserName", SqlDbType.NVarChar);    
                        //if administrator, get all tags for all users, otherwise just this user's tags
                        //if (User.IsInRole("Administrator"))
                            cmd.Parameters["@UserName"].Value = "";
                        //else
                        //    cmd.Parameters["@UserName"].Value = User.Identity.Name;

                        reader = cmd.ExecuteReader();

                        results = "<taglist>";
                        while (reader.Read())
                        {
                            results += reader["Tag"].ToString() + ";";
                        }
                        reader.Close();
                        results += "</taglist>";
                        break;
                    case "AddTag":
                        if (!User.Identity.IsAuthenticated)
                            return null;
                        if (tag.Trim() != "")
                        {
                            tag = tag.Replace("<", "").Replace(">", ""); //strip just these html characters
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "w_AddTag";
                            cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = User.Identity.Name;
                            cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag;
                            cmd.ExecuteNonQuery();
                            results = "<tagadded>" + tag + "</tagadded>";
                        }
                        else
                            results = "<error>No tag specified.</error>";
                        break;
                    case "DeleteTag":
                        if (!User.Identity.IsAuthenticated)
                            return null;
                        if (tag.Trim() != "")
                        {
                            tag = tag.Replace("<", "").Replace(">", ""); //strip just these html characters
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "w_DeleteTag";
                            cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = pageName;
                            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = User.Identity.Name;
                            cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = tag;
                            cmd.Parameters.Add("@DeleteAll", SqlDbType.Bit);
                            //delete all occurrences of this tag for this page if user is admin
                            if (User.IsInRole("Administrator"))
                                cmd.Parameters["@DeleteAll"].Value = 1;
                            else
                                cmd.Parameters["@DeleteAll"].Value = 0;
                            
                            cmd.ExecuteNonQuery();
                            results = "<tagadded>" + tag + "</tagadded>";
                        }
                        else
                            results = "<error>No tag specified.</error>";

                        break;
                    default:
                        break;
                }
            } catch (Exception ex)
            {
                results = "<error>" + ex.Message + "</error>";
            }

            
        }

        return results;
    }
}
