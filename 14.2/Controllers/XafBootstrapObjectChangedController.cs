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
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using XAF_Bootstrap.Templates;

namespace XAF_Bootstrap.Controllers
{
    public partial class XafBootstrapObjectChangedController : ViewController
    {
        public XafBootstrapObjectChangedController()
        {
            InitializeComponent();            
        }
        protected override void OnActivated()
        {
            XafBootstrapObjectChangedControllerHelper.Checker().ListClear();
            View.ObjectSpace.ObjectChanged += ObjectChangedExecute;
            View.ObjectSpace.Committed += ObjectSpaceRemoveChanges;
            View.ObjectSpace.RollingBack += ObjectSpaceRemoveChanges;
            View.ObjectSpace.Refreshing += ObjectSpaceRemoveChanges;
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
            if (XafBootstrapObjectChangedControllerHelper.Checker().CheckModified(View.ObjectSpace))
                WebWindow.CurrentRequestWindow.RegisterStartupScript("WindowDataChanged", "window.DataChanged = true;", true);
            else
                WebWindow.CurrentRequestWindow.RegisterStartupScript("WindowDataChanged", "window.DataChanged = false;", true);
        }
        protected override void OnDeactivated()
        {
            View.ObjectSpace.ObjectChanged -= ObjectChangedExecute;
            View.ObjectSpace.Committed -= ObjectSpaceRemoveChanges;
            View.ObjectSpace.RollingBack -= ObjectSpaceRemoveChanges;
            View.ObjectSpace.Refreshing -= ObjectSpaceRemoveChanges;
            base.OnDeactivated();
        }

        public void ObjectSpaceRemoveChanges(object sender, EventArgs e)
        {
            WebWindow.CurrentRequestWindow.RegisterStartupScript("WindowDataChanged", " window.DataChanged = false;", true);
            if (View != null)
                XafBootstrapObjectChangedControllerHelper.Checker().ClearOS(View.ObjectSpace);
        }

        public void ObjectChangedExecute(object sender, ObjectChangedEventArgs e)
        {
            if ((View != null))
                XafBootstrapObjectChangedControllerHelper.Checker().SetModified(View.ObjectSpace);
        }
    }

    public class XafBootstrapObjectSpaceChangedStatus
    {
        private IList<IObjectSpace> osList;
        public XafBootstrapObjectSpaceChangedStatus()
        {
            osList = new List<IObjectSpace>();
        }

        public void SetModified(IObjectSpace os)
        {
            if (!osList.Contains(os))
                osList.Add(os);
        }

        public Boolean CheckModified(IObjectSpace os)
        {
            return osList.Contains(os);
        }

        public void ListClear()
        {
            osList.Clear();
        }

        public void ClearOS(IObjectSpace os)
        {
            osList.Remove(os);
        }
    }

    public static class XafBootstrapObjectChangedControllerHelper
    {
        public static XafBootstrapObjectSpaceChangedStatus Checker()
        {
            if (Helpers.Session["ObjectSpaceChangedStatus"] == null)
                Helpers.Session["ObjectSpaceChangedStatus"] = new XafBootstrapObjectSpaceChangedStatus();
            return Helpers.Session["ObjectSpaceChangedStatus"] as XafBootstrapObjectSpaceChangedStatus;
        }

    }
}
