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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Web;
using XAF_Bootstrap.ModelExtensions;

namespace XAF_Bootstrap.Controllers
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic.
    public partial class XafBootstrapPageOptionsController : ViewController
    {
        public XafBootstrapPageOptionsController()
        {
            InitializeComponent();            
        }        

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var opts = (View.Model as IModelBootstrapPageOptions);
            var FullContent = false;
            var HideHeader = false;
            var HideFooter = false;
            if (opts != null && (opts.ContentFullWidth || opts.HideHeader || opts.HideFooter))
            {
                FullContent = opts.ContentFullWidth;
                HideHeader = opts.HideHeader;
                HideFooter = opts.HideFooter;                
            };
            WebWindow.CurrentRequestWindow.RegisterStartupScript("XafBootstrapPageOptionsController", String.Format(@"                                        
                if (window.XafBootstrapHideHeader)
                    window.XafBootstrapHideHeader({0});
                if (window.XafBootstrapHideFooter)
                    window.XafBootstrapHideFooter({1});
                if (window.XafBootstrapFullScreen)
                    window.XafBootstrapFullScreen({2});
            ", HideHeader.ToString().ToLower()
             , HideFooter.ToString().ToLower()
             , FullContent.ToString().ToLower()), true);
        }
    }
}
