/*
Copyright (c) 2005-2006, Clay Alberty
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

public partial class ShowFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            try
            {
                ShowSelectedFile(Request.QueryString["id"].ToString());
            }
            catch 
            {
                //do nothing
            }
        }

    }

    protected void ShowSelectedFile(string id)
    {
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select * From Attachments Where AttachmentID = @AttachmentID", conn);
                SqlDataReader reader;
                cmd.Parameters.Add("@AttachmentID", SqlDbType.UniqueIdentifier).Value = new Guid(id);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    switch(reader["Extension"].ToString().ToLower())
                    {
                        case "jpeg": 
                            Response.ContentType = "image/jpeg";
                            break;
                        case "gif":
                            Response.ContentType = "image/gif";
                            break;
                        case "png":
                            Response.ContentType = "image/png";
                            break;
                        case "pdf":
                            Response.ContentType = "application/pdf";
                            break;
                        default:
                            Response.ContentType = "application/octet-stream";
                            break;
                    }
                    
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + reader["AttachmentName"].ToString());

                    //byte[] returnval = (byte[])(reader["AttachmentData"]);
                    Response.BinaryWrite((byte[])(reader["AttachmentData"]));
                    Response.End();
                    Response.Flush();
                }
                else
                {
                    throw new Exception("Could not find specified file");
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
