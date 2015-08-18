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
using System.Linq;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web;

namespace XafBootstrap.Web
{   
    public partial class CustomDashboardViewRootController : ViewController
    {
        XafBootstrapView view;
        Boolean IsObjectChanged;

        public CustomDashboardViewRootController()
        {
            InitializeComponent();
            RegisterActions(components);
        }        

        protected override void OnActivated()
        {
            base.OnActivated();
            if (WebWindow.CurrentRequestWindow != null && View.ObjectSpace != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender += CurrentRequestWindow_PagePreRender;
                View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
                View.ObjectSpace.Committed += ObjectSpace_Committed;
                View.ObjectSpace.RollingBack += ObjectSpace_RollingBack;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
            }
        }

        void ObjectSpace_Refreshing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsObjectChanged = true;
        }

        void ObjectSpace_RollingBack(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsObjectChanged = true;
        }

        void ObjectSpace_Committed(object sender, EventArgs e)
        {
            IsObjectChanged = true;
        }


        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            IsObjectChanged = true;
        }

        void CurrentRequestWindow_PagePreRender(object sender, EventArgs e)
        {
            if (view != null && IsObjectChanged && !(Frame is PopupWindow))
            {
                view.InnerRender();
                IsObjectChanged = false;
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            Frame.GetController<DashboardCustomizationController>().OrganizeDashboardAction.Active["CustomDashboardViewRootController"] = false;
            IsObjectChanged = false;
            DashboardView DetailView = (View as DashboardView);
            view = new XafBootstrapView();
            view.IsRootView = true;
            CustomPanel cp = new CustomPanel();
            IList<Control> ctrls = new List<Control>();

            foreach (Control control in (DetailView.Control as Control).Controls)
                ctrls.Add(control);

            foreach (var control in ctrls)
                cp.Controls.Add(control);

            view.View = DetailView;
            view.ControlToRender = cp;
            (View.Control as Control).Controls.Add(view);
        }

        protected override void OnDeactivated()
        {
            if (WebWindow.CurrentRequestWindow != null && View.ObjectSpace != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender -= CurrentRequestWindow_PagePreRender;
                View.ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
                View.ObjectSpace.Committed -= ObjectSpace_Committed;
                View.ObjectSpace.RollingBack -= ObjectSpace_RollingBack;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
            }
            Frame.GetController<DashboardCustomizationController>().OrganizeDashboardAction.Active.RemoveItem("CustomDashboardViewRootController");
            base.OnDeactivated();
        }

    }
}
