#region Copyright (c) 2014-2015 DevCloud Solutions
/*
{********************************************************************************}
{                                                                                }
{   Copyright (c) 2014-2015 DevCloud Solutions                                   }
{                                                                                }
{   Licensed under the Apache License, Version 2.0 (the "License");              }
{   you may not use this file except in compliance with the License.             }
{   You may obtain a copy of the License at                                      }
{                                                                                }
{       http://www.apache.org/licenses/LICENSE-2.0                               }
{                                                                                }
{   Unless required by applicable law or agreed to in writing, software          }
{   distributed under the License is distributed on an "AS IS" BASIS,            }
{   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.     }
{   See the License for the specific language governing permissions and          }
{   limitations under the License.                                               }
{                                                                                }
{********************************************************************************}
*/
#endregion

using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using System.Linq;
using DevExpress.ExpressApp;
using XAF_Bootstrap.Controls;

namespace XAF_Bootstrap.Templates
{
    public partial class DialogTemplate : TemplateContent, ILookupPopupFrameTemplate
    {
        CallbackHandler handler;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            handler = new CallbackHandler(VSC.CurrentView.Id + "_dialog");
            handler.OnCallback += handler_OnCallback;

            WebWindow window = WebWindow.CurrentRequestWindow;
            if (window != null)
            {   
                window.PagePreRender += new EventHandler(window_PagePreRender);
                PAC.MenuItemsCreated += PAC_MenuItemsCreated;
                OCC.MenuItemsCreated += OCC_MenuItemsCreated;
                SAC.MenuItemsCreated += SAC_MenuItemsCreated;
            }

            Helpers.AddMeta(Page);
            Page.Header.Controls.Add(new HTMLText(@"<style> body { background: none; } </style>"));
        }

        void SAC_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(2);
        }

        void OCC_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(1);
        }

        void PAC_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(0);
        }
        protected override void OnUnload(EventArgs e)
        {
            if (WebWindow.CurrentRequestWindow != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(window_PagePreRender);
            }
            base.OnUnload(e);
        }
        private void window_PagePreRender(object sender, EventArgs e)
        {   
            UpdateActions();
        }
        #region ILookupPopupFrameTemplate Members

        public bool IsSearchEnabled
        {
            get { return SAC.Visible; }
            set { SAC.Visible = value; }
        }

        public void SetStartSearchString(string searchString) { }

        #endregion
        #region IFrameTemplate Members

        public ICollection<DevExpress.ExpressApp.Templates.IActionContainer> GetContainers()
        {
            return null;
        }
        public void SetView(DevExpress.ExpressApp.View view)
        {
        }
        #endregion
        public override object ViewSiteControl
        {
            get
            {
                return VSC;
            }
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
        }
        public override IActionContainer DefaultContainer
        {
            get { return null; }
        }
        public void FocusFindEditor() { }

        public void UpdateActions(int Type = -1)
        {
            if (Type == -1 || Type == 0)
            {
                Actions.EncodeHtml = false;
                Actions.Text = Helpers.BuildActionsMenu(PAC, VSC.CurrentView.Id + "_dialog", false, "btn btn-primary btn-sm", "button", "startProgress(); ");
            }

            if (Type == -1 || Type == 1)
            {
                LeftActions.EncodeHtml = false;
                LeftActions.Text = Helpers.BuildActionsMenu(OCC, VSC.CurrentView.Id + "_dialog", true, "btn btn-primary btn-sm", "button", "");
            }

            if (Type == -1 || Type == 2)
            {
                RightActions.EncodeHtml = false;
                RightActions.Text = Helpers.BuildActionsMenu(SAC, VSC.CurrentView.Id + "_dialog", false, "btn btn-primary btn-sm", "button", "");
            }
        }

        void handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            Frame Frame = ((Frame)Session[VSC.CurrentView.Id + "_Frame"]);
            Helpers.ProcessMenuAction(e.Parameter, Frame);
        }
    }
}
