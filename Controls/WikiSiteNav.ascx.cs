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
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ProntoWiki.Controls
{
    public partial class WikiSiteNav : System.Web.UI.UserControl
    {
        /// <summary>
        /// Adds a page to the list
        /// </summary>
        /// <param name="pageName"></param>
        public void Add(string pageName)
        {
            string[] pages = GetPageList();
            if (pageName == null || pageName == "")
                pageName = ConfigurationManager.AppSettings["defaultPage"].ToString();

            bool isPresent = false;

            //if it is already there, move it to the start of the list (highest index)
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == pageName)
                {
                    isPresent = true;
                    pages[i] = null;
                    break;
                }
            }

            //collapse the list, leaving the last index open
            if (isPresent)
            {
                for (int i = 0; i < pages.Length - 1; i++)
                {
                    if (pages[i] == null)
                    {
                        pages[i] = pages[i + 1];
                        pages[i + 1] = null;
                    }
                }
            }

            //add to the first free index, counting up
            bool inserted = false;
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] == null)
                {
                    pages[i] = pageName;
                    inserted = true;
                    break;
                }
            }
            if (!inserted)
            {
                for (int i = 0; i < pages.Length - 1; i++)
                    pages[i] = pages[i + 1];
                pages[pages.Length - 1] = pageName;
            }
            Session["PageList"] = pages;


        }

        private string[] GetPageList()
        {
            int pageCount = int.Parse(ConfigurationManager.AppSettings["pageListCount"]);
            string[] pages = null;

            if (Session["PageList"] != null)
                pages = (string[])Session["PageList"];
            else
                pages = new string[pageCount];

            return pages;
        }

        private string divider = ConfigurationManager.AppSettings["pageListDivider"].ToString();

        public string Divider
        {
            get { return divider; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string displayText = "";

            string[] pages = GetPageList();

            //add the newest first to the visible list
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                    displayText += string.Format(@"<a href=""default.aspx?page={0}"">{1}</a>&nbsp;{2}&nbsp;",
                        HttpUtility.UrlEncode(pages[i]), pages[i], divider);
            }

            if (displayText.Length > 0)
            {
                //drop the divider off the end of the string
                displayText = displayText.Substring(0, displayText.Length - 6 - divider.Length);

                //add a pipe to separate from logout text if authenticated.
                if (Request.IsAuthenticated) displayText += "&nbsp;|&nbsp;";

                litPageList.Text = displayText;
            }
        }
    }
}