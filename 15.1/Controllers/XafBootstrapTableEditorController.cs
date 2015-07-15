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
using DevExpress.ExpressApp.Editors;
using XAF_Bootstrap.Editors.XafBootstrapTableEditor;
using DevExpress.ExpressApp.Web.Templates;

namespace XAF_Bootstrap.Controllers
{    
    public partial class XafBootstrapTableEditorController : ViewController
    {
        public XafBootstrapTableEditorController()
        {
            InitializeComponent();            
        }        
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View != null && View is ListView && (View as ListView).Editor is XafBootstrapTableEditor)
            {
                var editor = (View as ListView).Editor as XafBootstrapTableEditor;
                editor.IsLookup = IsLookupTemplate();       
            }
        }
        
        protected bool IsLookupTemplate()
        {
            var page = Frame.Template as BaseXafPage;
            if (page != null)
                return page.TemplateContent is ILookupPopupFrameTemplate;
            return false;
        }
    }
}
