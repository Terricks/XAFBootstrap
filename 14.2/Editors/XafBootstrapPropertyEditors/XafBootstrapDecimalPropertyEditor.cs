﻿#region Copyright (c) 2014-2015 DevCloud Solutions
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
using DevExpress.Persistent.Base;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.Decimal), "XafBootstrapDecimalPropertyEditor", true)]
    [PropertyEditor(typeof(System.Double), "XafBootstrapDecimalPropertyEditor", true)]
    public class XafBootstrapDecimalPropertyEditor : ASPxPropertyEditor, IXafBootstrapEditor
    {
        public XafBootstrapDecimalPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {   
        }

        public XafBootstrapNumericEdit Edit;
        private void InitEdit()
        {   
            Edit = new XafBootstrapNumericEdit();
            Edit.Placeholder = this.Caption;
            Edit.IsPassword = Model.IsPassword;
            Edit.PropertyName = PropertyName;
            Edit.RowCount = Model.RowCount;
            Edit.OnClickScript = GetImmediatePostDataScript();

            String displayFormat = String.Concat(DisplayFormat);
            if (displayFormat == "")
            {
                ObjectCaptionFormatAttribute aAttr = (MemberInfo.MemberTypeInfo.FindAttribute<ObjectCaptionFormatAttribute>());
                if (aAttr != null)
                    displayFormat = aAttr.FormatString;
            }
            if (displayFormat == "")
                displayFormat = "{0:n}";
            Edit.DisplayFormat = displayFormat;            
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
            Edit.TextOnly = this.ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.View;
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
            decimal val = 0;
            if (decimal.TryParse(String.Concat(PropertyValue), out val))
                Edit.Value = val;
        }
        
        protected override void ReadViewModeValueCore()
        {
            decimal val = 0;
            if (decimal.TryParse(String.Concat(PropertyValue), out val))
                Edit.Value = val;
        }

        public string GetImmediatePostDataScript()
        {
            return Helpers.GetImmediatePostDataScript(MemberInfo);
        }
    }
}
