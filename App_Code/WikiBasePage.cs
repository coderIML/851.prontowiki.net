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
using System.Diagnostics;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace ProntoWiki
{

    /// <summary>
    /// Summary description for WikiBasePage
    /// </summary>
    public class WikiBasePage : System.Web.UI.Page
    {

        protected string pageName;

        protected string GetPageName()
        {
            if (Request.Params["page"] == null || Request.Params["page"] == "")
                return ConfigurationManager.AppSettings["defaultPage"];
            else
                return HttpUtility.UrlDecode(Request.Params["page"].ToString());
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            HandleError(Server.GetLastError());
            Server.ClearError();
        }

        protected void HandleError(Exception ex)
        {               
            string errorMessage = "Page: " + Request.Url.ToString() +
                    "\nUser: " +
                    ((Request.IsAuthenticated) ? User.Identity.Name : Request.ServerVariables["REMOTE_HOST"].ToString()) + "\n" +
                    ex.Message + "\n" + ex.StackTrace;
            Response.Write(@"<table width=100% CellPadding=0 CellSpacing=0><tr><td class=""errormessage"">An error occurred in the system, and last operation may not have completed: <em>" +
                ex.Message + "</em></td></tr></table>");

            if (ConfigurationManager.AppSettings["loggingEnabled"].ToLower() == "true")
            {
                try
                {
                    switch (ConfigurationManager.AppSettings["logFile"].ToLower())
                    {
                        case "eventlog":
                            //Create the event log if not present
                            string logName = "ProntoWikiLog";
                            string sourceName = "ProntoWikiWebPage";
                            if (!EventLog.SourceExists(sourceName))
                            {
                                EventLog.CreateEventSource(sourceName, logName);
                            }
                            EventLog log = new EventLog();
                            log.Source = sourceName;
                            log.WriteEntry(errorMessage, EventLogEntryType.Error);
                            break;
                        default: //if not "eventlog", save it to the specified file
                            FileInfo f = new FileInfo(ConfigurationManager.AppSettings["logFile"]);
                            using (FileStream s = f.Open(FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write))
                            {
                                errorMessage = "**Error at " + DateTime.Now.ToString() + " : " + errorMessage + "\r\n\r\n";
                                s.Write(System.Text.ASCIIEncoding.UTF8.GetBytes(errorMessage), 0, errorMessage.Length);
                                s.Flush();
                            }
                            break;
                    }
                }
                catch { } //do nothing if logging fails
                        

            }                            

        }

    }

}
