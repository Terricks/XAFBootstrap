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
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Templates;
using System.Linq;
using XAF_Bootstrap.Controls;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web;

namespace XAF_Bootstrap.Templates
{
    public partial class DialogTemplate : TemplateContent, ILookupPopupFrameTemplate, IXafPopupWindowControlContainer
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Helpers.AddMeta(Page);
            Page.Header.Controls.Add(new HTMLText(@"<style> body { background: none; } </style>"));
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
            return SAC.ActionContainers;
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

        public DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl XafPopupWindowControl
        {
            get { return PopupWindowControl; }
        }
    }
}
