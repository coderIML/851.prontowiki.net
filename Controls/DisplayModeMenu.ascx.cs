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
    public partial class DisplayModeMenu : System.Web.UI.UserControl 
    {
        // Use a field to reference the current WebPartManager control.
        WebPartManager _manager;

        void Page_Init(object sender, EventArgs e)
        {
            Page.InitComplete += new EventHandler(InitComplete);
        }

        void InitComplete(object sender, System.EventArgs e)
        {

            _manager = WebPartManager.GetCurrentWebPartManager(Page);
            if (_manager.Personalization.CanEnterSharedScope)
            {
                //Page layout changes will propogate to all users
                if (_manager.Personalization.Scope == PersonalizationScope.User)
                    _manager.Personalization.ToggleScope();

                String browseModeName = WebPartManager.BrowseDisplayMode.Name;

                // Fill the drop-down list with the names of supported display modes.
                foreach (WebPartDisplayMode mode in
                  _manager.SupportedDisplayModes)
                {
                    String modeName = mode.Name;
                    // Make sure a mode is enabled before adding it.
                    if (mode.IsEnabled(_manager))
                    {
                        ListItem item = new ListItem(modeName, modeName);
                        DisplayModeDropdown.Items.Add(item);
                    }
                }
            }

        }

        // Change the page to the selected display mode.
        protected void DisplayModeDropdown_SelectedIndexChanged(object sender,
          EventArgs e)
        {
            String selectedMode = DisplayModeDropdown.SelectedValue;

            WebPartDisplayMode mode =
             _manager.SupportedDisplayModes[selectedMode];
            if (mode != null)
                _manager.DisplayMode = mode;
        }

        // Set the selected item equal to the current display mode.
        void Page_PreRender(object sender, EventArgs e)
        {
            ListItemCollection items = DisplayModeDropdown.Items;
            int selectedIndex =
              items.IndexOf(items.FindByText(_manager.DisplayMode.Name));
            DisplayModeDropdown.SelectedIndex = selectedIndex;
        }

    }
}
