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
using ProntoWiki.Controls;

public partial class UserMembership : WikiBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.IsInRole("Administrator"))
        {
            Response.Redirect("default.aspx");
        }
        else
        {
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
            {
                grdUsers.DataSource = Membership.GetAllUsers();
                grdUsers.DataBind();
            }
        }
    }

    private DataSet GetUserDetails(string userName)
    {
        DataSet ds = new DataSet();
        try
        {
            MembershipUser user = Membership.GetUser(userName);
            ds.Tables.Add();
            ds.Tables[0].Columns.Add("UserName");
            ds.Tables[0].Columns.Add("Email");
            ds.Tables[0].Columns.Add("IsApproved");
            ds.Tables[0].Columns.Add("Comment");

            ds.Tables[0].Rows.Add(new object[] 
                { user.UserName,
                    user.Email,
                    user.IsApproved,
                    user.Comment
                }
            );
            return ds;
        }
        catch (Exception ex)
        {
            HandleError(ex);
            return null;
        }

    }

    protected void grdUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        dtvUsers.DataSource = GetUserDetails(grdUsers.SelectedDataKey.Value.ToString());
        dtvUsers.DataBind();
        BindUserRoles(grdUsers.SelectedDataKey.Value.ToString());
    }

    protected void BindUserRoles(string username)
    {
        DataSet ds = new DataSet();

        string[] roles = Roles.GetAllRoles();

        ds.Tables.Add();
        ds.Tables[0].Columns.Add("RoleName");
        ds.Tables[0].Columns.Add("IsInRole");
                
        foreach (string role in Roles.GetAllRoles())
        {            
            if (Roles.IsUserInRole(username, role))
                ds.Tables[0].Rows.Add(new object[] { role, "true" });
            else
                ds.Tables[0].Rows.Add(new object[] { role, "false" });
        }

        dlstUserRoles.DataSource = ds;
        dlstUserRoles.DataBind();

    }

    protected void grdUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdUsers.PageIndex = e.NewPageIndex;
        grdUsers.DataBind();
    }

    protected void dtvUsers_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
    {
        Membership.DeleteUser(dtvUsers.Rows[0].Cells[1].Text);
        ResetGrid();
    }

    protected void dtvUsers_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
    {
        //update user
        MembershipUser user = Membership.GetUser(dtvUsers.Rows[0].Cells[1].Text);
        user.IsApproved = ((CheckBox)(dtvUsers.Rows[1].Cells[1].Controls[0])).Checked;
        user.Email = ((TextBox)(dtvUsers.Rows[2].Cells[1].Controls[0])).Text;
        user.Comment = ((TextBox)(dtvUsers.Rows[3].Cells[1].Controls[1])).Text;

        Membership.UpdateUser(user);        

        //update user roles

        foreach (DataListItem i in dlstUserRoles.Items)
        {
            CheckBox c = ((CheckBox)i.Controls[1]);
            if (c.Checked)
            {
                if (!Roles.IsUserInRole(user.UserName, c.Text))
                    Roles.AddUserToRole(user.UserName, c.Text);
            }
            else
            {
                if (Roles.IsUserInRole(user.UserName, c.Text))
                    Roles.RemoveUserFromRole(user.UserName, c.Text);
            }

        }
    }

    protected void dtvUsers_ItemCommand(object sender, DetailsViewCommandEventArgs e)
    {
        if (e.CommandName.ToLower() == "cancel")
        {
            ResetGrid();
        }
    }

    private void ResetGrid()
    {
        dtvUsers.DataSource = null;
        dtvUsers.DataBind();
        dlstUserRoles.DataSource = null;
        dlstUserRoles.DataBind();
        grdUsers.SelectedIndex = -1;
        grdUsers.DataSource = Membership.GetAllUsers();
        grdUsers.DataBind();
    }
    protected void dtvUsers_ModeChanging(object sender, DetailsViewModeEventArgs e)
    {
        //for some reason this is fired on the cancel event
        dtvUsers.ChangeMode(DetailsViewMode.Edit);
    }
}
