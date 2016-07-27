#region Copyright (c) 2014-2016 DevCloud Solutions
/*
{********************************************************************************}
{                                                                                }
{   Copyright (c) 2014-2016 DevCloud Solutions                                   }
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

namespace XAF_Bootstrap.Controllers
{    
    public partial class XafBootstrapFocusController : ViewController
    {
        public XafBootstrapFocusController()
        {
            InitializeComponent();            
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var DetailView = (View as DetailView);
            if (DetailView != null)
            {
                var defProp = DetailView.ObjectTypeInfo.FindAttribute<System.ComponentModel.DefaultPropertyAttribute>();
                if (defProp != null)
                {
                    var member = DetailView.ObjectTypeInfo.FindMember(defProp.Name);
                    if (member != null)
                    {
                        var placeholder = member.Name;
                        var displayAttr = member.FindAttribute<DevExpress.ExpressApp.DC.XafDisplayNameAttribute>();
                        if (displayAttr != null)
                            placeholder = displayAttr.DisplayName;

                        var placeholderAttr = member.FindAttribute<ModelExtensions.Attributes.PlaceholderAttribute>();
                        if (placeholderAttr != null)
                            placeholder = placeholderAttr.Placeholder;

                        if (String.Concat(placeholder) != "")
                            WebWindow.CurrentRequestWindow.RegisterStartupScript("SetFocusToElement", String.Format("$('[placeholder={0}]').focus();", placeholder), true);
                    }
                }
            }
        }
        
    }
}
