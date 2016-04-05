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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ProntoWiki
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated) loginStatus1.Visible = false;

            //handle the page header, etc.
            Page.Title = ConfigurationManager.AppSettings["pageTitle"].ToString();
            lblHeader.Text = ConfigurationManager.AppSettings["pageTitle"].ToString();

            //hide the extra web part zones if not needed.
            if (!mgrMaster.Personalization.CanEnterSharedScope)
            {
                DisplayMenu1.Visible = false;
                if (wpzLeftTop.WebParts.Count < 1) wpzLeftTop.Visible = false;
                if (wpzRightTop.WebParts.Count < 1) wpzRightTop.Visible = false;
                if (wpzCenterBottom.WebParts.Count < 1) wpzCenterBottom.Visible = false;
                if (wpzLeftBottom.WebParts.Count < 1) wpzLeftBottom.Visible = false;
                if (wpzRightBottom.WebParts.Count < 1) wpzRightBottom.Visible = false;
            }
            else
            {
                DisplayMenu1.Visible = true;
                wpzLeftTop.Visible = true;
                wpzRightTop.Visible = true;
                wpzCenterBottom.Visible = true;
                wpzLeftBottom.Visible = true;
                wpzRightBottom.Visible = true;
            }
            litFooter.Text = "<span class=menutextindent>" + ConfigurationManager.AppSettings["pageFooter"].ToString()
                + "</span>";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if ((mgrMaster.Personalization.Scope == PersonalizationScope.User)
                && (mgrMaster.Personalization.CanEnterSharedScope))
            {
                mgrMaster.Personalization.ToggleScope();
            }
            else if (mgrMaster.Personalization.Scope ==
                  PersonalizationScope.Shared)
            {
                mgrMaster.Personalization.ToggleScope();
            }
            else
            {
                // If the user cannot enter shared scope you may want
                // to notify them on the page.
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            string errorMessage = "Error occurred" + Server.GetLastError();

            Server.ClearError();
            string LogName = "ProntoWikiLog";
            string SourceName = "ProntoWikiWebPage";
            if (!EventLog.SourceExists(SourceName))
            {

                EventLog.CreateEventSource(SourceName, LogName);

            }
            EventLog MyLog = new EventLog();
            MyLog.Source = SourceName;
            MyLog.WriteEntry(errorMessage, EventLogEntryType.Error);
        }

    }
}
