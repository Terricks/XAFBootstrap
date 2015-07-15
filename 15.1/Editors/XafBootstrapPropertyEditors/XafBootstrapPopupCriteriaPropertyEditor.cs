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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.String), "XafBootstrapPopupCriteriaPropertyEditor", false)]
    public class XafBootstrapPopupCriteriaPropertyEditor : ASPxPopupCriteriaPropertyEditor, IXafBootstrapEditor
    {
        public XafBootstrapPopupCriteriaPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {   
        }

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            var control = base.CreateEditModeControlCore();
            if (control is ASPxButtonEdit)
            {
                var edit = (control as ASPxButtonEdit);
                edit.CssClass = "form-control input-sm";
                edit.ReadOnly = false;
                edit.ValueChanged += new EventHandler(this.EditValueChangedHandler);
                edit.EnableTheming = false;
                edit.ButtonStyle.CssClass = "";
            }
            return control;
        }
    }
}
