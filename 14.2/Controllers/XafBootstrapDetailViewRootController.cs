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
using DevExpress.ExpressApp.Web.Templates;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Layout;

namespace XafBootstrap.Web
{   
    public partial class CustomDetailViewRootController : ViewController
    {
        public CustomDetailViewRootController()
        {
            InitializeComponent();
            RegisterActions(components);            
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();           

            DetailView DetailView = (View as DetailView);
            XafBootstrapView view = (XafBootstrapView)(Frame.Template as BaseXafPage).LoadControl("~/Templates/XafBootstrapView.ascx");
            view.IsRootView = true;
            CustomPanel cp = new CustomPanel();
            IList<Control> ctrls = new List<Control>();

            foreach (Control control in (DetailView.Control as Control).Controls)
                ctrls.Add(control);                

            foreach(var control in ctrls)
                cp.Controls.Add(control);

            view.View = DetailView;
            view.ControlToRender = cp;
            (View.Control as Control).Controls.Add(view);
        }              
    }
}
