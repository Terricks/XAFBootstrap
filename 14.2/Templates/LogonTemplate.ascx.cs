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
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;

namespace XAF_Bootstrap.Templates
{
    public partial class LogonTemplate : TemplateContent
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Helpers.AddMeta(Page);                        
        }
        public override IActionContainer DefaultContainer
        {
            get { return null; }
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
        public XafPopupWindowControl XafPopupWindowControl
        {
            get { return PopupWindowControl; }
        }

    }
}
