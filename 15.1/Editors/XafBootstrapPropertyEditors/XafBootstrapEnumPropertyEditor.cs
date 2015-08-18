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

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(Enum), "XafBootstrapEnumPropertyEditor", true)]
    public class XafBootstrapEnumPropertyEditor : ASPxPropertyEditor, IXafBootstrapEditor, IComplexViewItem
    {
        public XafBootstrapEnumPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }

        #region IComplexViewItem Members

        public IObjectSpace ObjectSpace;
        public XafApplication Application;

        void IComplexViewItem.Setup(DevExpress.ExpressApp.IObjectSpace objectSpace, DevExpress.ExpressApp.XafApplication application)
        {
            this.ObjectSpace = objectSpace;
            this.Application = application;
        }

        #endregion

        public XafBootstrapDropdownEdit Edit;        
        protected override void SetupControl(System.Web.UI.WebControls.WebControl control)
        {
            base.SetupControl(control);            
            Edit.OnClickScript = GetImmediatePostDataScript();
        }

        public String GetXafDisplayName(Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(XafDisplayNameAttribute), false);

            if (attributes.Count() > 0 && attributes[0] is XafDisplayNameAttribute)
                return ((XafDisplayNameAttribute)attributes[0]).DisplayName;
            return String.Concat(enumVal);
        }

        private void InitEdit()
        {   
            Edit = new XafBootstrapDropdownEdit();
            Edit.Items.List.Clear();
            if (typeof(Enum).IsAssignableFrom(MemberInfo.MemberType))
            {
                foreach (var item in Enum.GetValues(MemberInfo.MemberType))
                {
                    int value = (int)item;
                    string displayValue = GetXafDisplayName((Enum)item);
                    Edit.Items.Add(new XafBootstrapDropdownItem() { Text = displayValue, Value = value });
                }
            }

            Edit.PropertyName = PropertyName;            
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
        
        protected override object GetControlValueCore()
        {
            int value = 0;            
            if (int.TryParse(String.Concat(Edit.Value), out value))
                return value;
            return 0;
        }
        
        protected override void ReadEditModeValueCore()
        {
            Edit.Value = (PropertyValue == null ? 0 : (int)PropertyValue);
        }
        
        protected override void ReadViewModeValueCore()
        {
            Edit.Value = (PropertyValue == null ? 0 : (int)PropertyValue);
        }

        public string GetImmediatePostDataScript()
        {
            return Helpers.GetImmediatePostDataScript(MemberInfo);
        }
    }
}
