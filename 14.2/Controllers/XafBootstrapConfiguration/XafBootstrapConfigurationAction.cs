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
using DevExpress.ExpressApp.Actions;
using XAF_Bootstrap.Templates;
using DevExpress.ExpressApp.DC;

namespace XAF_Bootstrap.Controllers.XafBootstrapConfiguration
{    
    public partial class XafBootstrapConfigurationAction : ViewController
    {
        public XafBootstrapConfigurationAction()
        {
            InitializeComponent();            
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var actionVisible = SecuritySystem.CurrentUser == null;
            if (!actionVisible) {
                IMemberInfo memberInfo;
                var roles = ObjectFormatValues.GetValueRecursive("Roles", SecuritySystem.CurrentUser, out memberInfo) as IEnumerable<object>;
                foreach (var role in roles)
                {
                    Boolean isAdministrative;
                    if (Boolean.TryParse(String.Concat(ObjectFormatValues.GetValueRecursive("IsAdministrative", role, out memberInfo)), out isAdministrative))
                    {
                        if (isAdministrative)
                        {
                            actionVisible = true;
                            break;
                        }
                    }
                }
            }
            XafBootstrapConfigurationActionItem.Active["IsActionActive"] = actionVisible;
        }
        protected override void OnDeactivated()
        {
            XafBootstrapConfigurationActionItem.Active.RemoveItem("IsActionActive");
            base.OnDeactivated();
        }
        private void XafBootstrapConfigurationActionItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var os = Application.CreateObjectSpace();            
            var view = Application.CreateDetailView(os, XAF_Bootstrap.DatabaseUpdate.Updater.Configuration(os));
            Application.MainWindow.SetView(view);
        }
    }
}
