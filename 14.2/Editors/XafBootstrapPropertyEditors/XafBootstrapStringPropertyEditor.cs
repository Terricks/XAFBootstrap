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
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using XAF_Bootstrap.Templates;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.String), "XafBootstrapStringPropertyEditor", true)]
    public class XafBootstrapStringPropertyEditor : ASPxPropertyEditor, IXafBootstrapEditor
    {
        public XafBootstrapStringPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) {             
        }

        public XafBootstrapStringEdit Edit;
        private void InitEdit()
        {   
            Edit = new XafBootstrapStringEdit();
            Edit.Placeholder = EditorHelper.GetPlaceholder(this, this.Caption);
            Edit.IsPassword = Model.IsPassword;
            Edit.PropertyName = PropertyName;
            Edit.RowCount = Model.RowCount;                        
        }

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = !AllowEdit;
            Edit.EditValueChanged += new EventHandler(EditValueChangedHandler);                
            return Edit;
        }

        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = true;
            return Edit;
        }

        protected override void SetImmediatePostDataCompanionScript(string script)
        {        
        }
        
        protected override object GetControlValueCore()
        {
            return Edit.Value;
        }
        
        protected override void ReadEditModeValueCore()
        {
            Edit.Value = String.Concat(PropertyValue);
        }
        
        protected override void ReadViewModeValueCore()
        {
            Edit.Value = String.Concat(PropertyValue);
        }
    }
}
