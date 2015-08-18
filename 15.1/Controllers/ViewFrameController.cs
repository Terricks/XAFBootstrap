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
using DevExpress.ExpressApp.Editors;
using XAF_Bootstrap.Templates;
using XAF_Bootstrap.Editors.XafBootstrapTableEditor;
using DevExpress.ExpressApp.Web;

namespace XAF_Bootstrap.Controllers
{    
    public partial class ViewFrameController : ViewController
    {
        public ViewFrameController()
        {
            InitializeComponent();
            RegisterActions(components);            
        }

        protected override void OnActivated()
        {
            base.OnActivated();           

            var focusController = Frame.GetController<DevExpress.ExpressApp.Web.SystemModule.FocusController>();
            if (focusController != null)
                focusController.Active["XafBootstrapActive"] = false;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            Helpers.AddMeta(WebWindow.CurrentRequestPage);

            if (View is ListView)
            {
                var ListView = (View as ListView);
                if (ListView.Editor is XafBootstrapTableEditor)
                {
                    if (Frame is NestedFrame)
                    {
                        var mode = ViewEditMode.View;
                        if (((NestedFrame)Frame).ViewItem.View is DetailView)
                            mode = (((NestedFrame)Frame).ViewItem.View as DetailView).ViewEditMode;

                        (ListView.Editor as XafBootstrapTableEditor).EditMode = mode;
                    }                    
                }
            }
        }

        protected override void OnDeactivated()
        {
            var focusController = Frame.GetController<DevExpress.ExpressApp.Web.SystemModule.FocusController>();
            if (focusController != null)
                focusController.Active.RemoveItem("XafBootstrapActive");
            base.OnDeactivated();
        }
    }
}
