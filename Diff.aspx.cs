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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ProntoWiki;

public partial class Diff : WikiBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string page = "";
        int v1 = 0;
        int v2 = 0;

        try
        {
            page = Request.Params["page"].ToString();
            v1 = int.Parse(Request.Params["v1"].ToString());
            v2 = int.Parse(Request.Params["v2"].ToString());
        }
        catch
        {
            HandleError(new Exception("This page needs a page and version parameters passed to it.  If you are " +
                "reading this message now, these parameters were not properly supplied.  Please return to " +
                "the history view for a wiki page and select the 'Show Differences' option from there, or " +
                "contact your system administrator"));
            Response.End();
            return;
        }       

        lblVersion1.Text = v1.ToString();
        lblVersion2.Text = v2.ToString();
        lnkPage.Text = page;

        //now get the page text from the wiki

        string page1Text = "";
        string page2Text = "";

        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("w_GetPageByVersion", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@PageName", SqlDbType.VarChar).Value = page;
            cmd.Parameters.Add("@Version", SqlDbType.Int).Value = v1;
            SqlDataReader reader;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    page1Text = reader["PageText"].ToString();
                reader.Close();
                cmd.Parameters["@Version"].Value = v2;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    page2Text = reader["PageText"].ToString();
                reader.Close();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        //display the results
        string results = HTMLDiff(page1Text, page2Text);
        //HACK:  handle UL and OL elements and headers
        results = Regex.Replace(results, @"(<span class=text(?:added|missing)>)\s?(\++|#+|!+)", "$2$1", RegexOptions.Compiled);        

        results = WikiParser.GenerateWikiText(results);
        litVersion1.Text = (results).Replace("&lt;span class=textadded&gt;",
            "<span class=textadded>").Replace("&lt;/span&gt;", "</span>").Replace("&lt;span class=textmissing&gt;",
            "<span class=textmissing>");
        
        

    }

    /// <summary>
    /// Performs a rudimentary diff on 2 strings.
    /// </summary>
    /// <param name="text1"></param>
    /// <param name="text2"></param>
    /// <returns>the first and second strings passed in, marked up with differences</returns>
    private string HTMLDiff(string text1, string text2)
    {
        text1 = text1.Replace("\r", "");
        text2 = text2.Replace("\r", "");
        char[] crlf = ((string)("\n")).ToCharArray();
        string lineBreak = "\n";

        string[] t1Lines = text1.Split(crlf);
        string[] t2Lines = text2.Split(crlf);

        StringBuilder t1Final = new StringBuilder();

        int lastPos = 0;
        bool foundLine = false;
        
        for (int count = 0; count < t1Lines.Length; count++)
        {
            for (int count2 = lastPos; count2 < t2Lines.Length; count2++)
            {

                if ( (t1Lines[count] == t2Lines[count2]) && (t1Lines[count] != "" && t2Lines[count2] != ""))
                {
                    //if there are lines in doc 2 that weren't in doc 1, mark
                    if ((count2 - lastPos) > 0)
                    {
                        for (int i = lastPos; i < count2; i++)
                            if (t2Lines[i] != "") t1Final.Append("<span class=textmissing>" + t2Lines[i] + "</span>" + lineBreak);      
                    }

                    t1Final.Append(t1Lines[count] + lineBreak);

                    lastPos = count2 + 1;
                    foundLine = true;
                    break;
                }
            }

            if (!foundLine) //mark lines in doc1 as additions                            
                if (t1Lines[count] != "") 
                    t1Final.Append("<span class=textadded>" + t1Lines[count] + "</span>" + lineBreak);
                else                
                    t1Final.Append(crlf);
                
            foundLine = false;
        }

        //catch any remaining lines in the second version
        if (lastPos < t2Lines.Length)
        {
            for (int i = lastPos; i < t2Lines.Length; i++)
                t1Final.Append("<span class=textmissing>" + t2Lines[i] + "</span>" + lineBreak);          
        }


        return t1Final.ToString();

    }

    protected void lnkPage_Click(object sender, EventArgs e)
    {
        Response.Redirect(string.Format("default.aspx?page={0}", HttpUtility.UrlEncode(((LinkButton)sender).Text)) );
    }
}
