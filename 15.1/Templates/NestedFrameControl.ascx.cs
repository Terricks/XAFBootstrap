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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Layout;
using System.Linq;

namespace XAF_Bootstrap.Templates
{    
    [ParentControlCssClass("NestedFrameControl")]
    public partial class NestedFrameControl : NestedFrameControlBase, IFrameTemplate, ISupportActionsToolbarVisibility
    {
        private void ToolBar_MenuItemsCreated(object sender, EventArgs e)
        {   
            Frame Frame = ((Frame)Session[View.Id + "_" + ClientID + "_Frame"]);
            Boolean DoUpdate = true;
            if (Frame != null && Frame is NestedFrame)
            {
                var nested = (Frame as NestedFrame);
                if (nested.ViewItem != null && nested.ViewItem.View is DashboardView)
                    DoUpdate = false;
            }
            if (DoUpdate)
                UpdateNestedActions();
        }
        
        protected override void OnInit(EventArgs e)
        {               
            base.OnInit(e);
            Helpers.AddMeta(Page);
        }

        public void UpdateNestedActions(Boolean InvokeCreateItems = true) {
            ToolBar.Visible = toolBarVisibility;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ToolBar != null)
            {
                ToolBar.MenuItemsCreated += new EventHandler(ToolBar_MenuItemsCreated);
                ToolBar.Load += ToolBar_Init;                
            }
            WebWindow window = WebWindow.CurrentRequestWindow;
            window.PagePreRender += new EventHandler(window_PagePreRender);
        }

        void ToolBar_Init(object sender, EventArgs e)
        {
            UpdateNestedActions();
        }

        private void window_PagePreRender(object sender, EventArgs e)
        {
            if (ToolBar != null)
                UpdateNestedActions();
        }
        
        public override void Dispose()
        {
            if (ToolBar != null)
            {
                ToolBar.MenuItemsCreated -= new EventHandler(ToolBar_MenuItemsCreated);
                ToolBar.Dispose();
                ToolBar = null;                
            }
            base.Dispose();
        }
        #region IFrameTemplate Members
        public override IActionContainer DefaultContainer
        {
            get
            {
                if (ToolBar != null)
                {
                    return ToolBar.FindActionContainerById("View");
                }
                return null;
            }
        }
        #endregion
        #region IActionBarVisibilityManager Members
        protected bool toolBarVisibility = true;
        public void SetVisible(bool isVisible)
        {   
            toolBarVisibility = isVisible;            
            if (ToolBar != null)
                ToolBar.Visible = toolBarVisibility;
            UpdateNestedActions();            
            Init -= new EventHandler(NestedFrameControl_Init);
            Init += new EventHandler(NestedFrameControl_Init);
            
        }
        private void NestedFrameControl_Init(object sender, EventArgs e)
        {   
            Init -= new EventHandler(NestedFrameControl_Init);            
        }
        #endregion
        protected override ContextActionsMenu CreateContextMenu()
        {
            return new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
        }

        public Boolean ToolBarHasActions()
        {
            if (!ToolBar.IsMenuItemsCreated)
                ToolBar.CreateMenuItems();
            return ToolBar != null && ToolBar.actionObjects.Count > 0;
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
        }

        public override object ViewSiteControl
        {
            get
            {
                return viewSiteControl;
            }
        }
    }
}
