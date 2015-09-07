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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(Object), "XafBootstrapLookupPropertyEditor", true)]
    public class XafBootstrapLookupPropertyEditor : ASPxPropertyEditor, IXafBootstrapEditor, IComplexViewItem
    {
        #region IComplexViewItem Members

        public IObjectSpace ObjectSpace;
        public XafApplication Application;

        void IComplexViewItem.Setup(DevExpress.ExpressApp.IObjectSpace objectSpace, DevExpress.ExpressApp.XafApplication application)
        {
            this.ObjectSpace = objectSpace;
            this.Application = application;
        }

        #endregion

        public XafBootstrapLookupPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) {             
        }

        public XafBootstrapDataSelectorEdit DataSelector;
        public XafBootstrapDropdownEdit DropDown;

        public Boolean IsSelector;
        
        protected override void SetupControl(System.Web.UI.WebControls.WebControl control)
        {
            base.SetupControl(control);
        }

        private void InitEdit()
        {
            var Helper = new LookupEditorHelper(Application, ObjectSpace.CreateNestedObjectSpace(), MemberInfo.MemberTypeInfo, Model);  
            IsSelector = Helper.EditorMode == LookupEditorMode.Search;
            
            CollectionSourceBase cs = null;

            if (!IsSelector)
            {
                cs = Helper.CreateCollectionSource(Helper.ObjectSpace.GetObject(CurrentObject));
                if (Helper.EditorMode == LookupEditorMode.Auto || Helper.EditorMode == LookupEditorMode.AllItemsWithSearch)
                    IsSelector = cs != null && cs.List.Count > 20;
            }

            String displayFormat = String.Concat(DisplayFormat);
            if (displayFormat == "")
                try
                {
                    ObjectCaptionFormatAttribute aAttr = (MemberInfo.MemberTypeInfo.FindAttribute<ObjectCaptionFormatAttribute>());
                    if (aAttr != null)
                        displayFormat = aAttr.FormatString;
                    else
                    {
                        var lookupProperty = String.Concat(Model.GetValue<String>("LookupProperty"));
                        if (lookupProperty != "")
                            displayFormat = String.Format("{{0:{0}}}", lookupProperty);
                    }
                }
                catch
                {
                }
            if (displayFormat == "")
                displayFormat = "{0}";

            var App = (WebApplication.Instance as XafApplication);
            if (IsSelector)
            {
                DataSelector = new XafBootstrapDataSelectorEdit(this, Application, ObjectSpace, Helper);
                DataSelector.Caption = Model.Caption;
                DataSelector.PropertyName = PropertyName;
                DataSelector.OnClickScript = GetImmediatePostDataScript();                
                DataSelector.ListView = (IModelListView)App.Model.Views[App.FindLookupListViewId(MemberInfo.MemberType)];                
                DataSelector.DisplayFormat = displayFormat;
            }
            else
            {
                DropDown = new XafBootstrapDropdownEdit();
                DropDown.OnClickScript = GetImmediatePostDataScript();                
                var listView = (IModelListView)App.Model.Views[App.FindLookupListViewId(MemberInfo.MemberType)];
                if (cs != null)
                    foreach (var obj in cs.List)
                    {
                        var item = DropDown.Items.Add();
                        var cols = listView.Columns.Where(f => f.Index == null || f.Index > -1).OrderBy(f => f.Index);
                        if (cols.Count() > 1) {
                            var builder = new List<String>();                    
                            foreach (var col in cols)
                            {
                                builder.Add(String.Format(new ObjectFormatter(), "{0:" + col.GetValue<String>("FieldName") +  "}", obj));
                            }
                            builder = builder.Where(f => String.Concat(f) != "").ToList();
                            item.Text = builder.First();                        
                            item.Hint = String.Join("<br>", builder.Skip(1).Take(builder.Count-1));
                        } else {
                            item.Text = String.Format(new ObjectFormatter(), String.Concat(displayFormat) == "" ? "{0}" : displayFormat, obj);
                        }
                        item.Value = ObjectSpace.GetObject(obj);
                    }
            }
        }

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            InitEdit();

            if (IsSelector)
            {
                DataSelector.TextOnly = !AllowEdit;
                DataSelector.EditValueChanged += new EventHandler(EditValueChangedHandler);
                return DataSelector;
            }
            else
            {
                DropDown.TextOnly = !AllowEdit;
                DropDown.EditValueChanged += new EventHandler(EditValueChangedHandler);
                return DropDown;
            }            
        }

        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore()
        {
            InitEdit();

            if (IsSelector)
            {
                DataSelector.TextOnly = true;
                return DataSelector;
            }
            else
            {
                DropDown.TextOnly = true;
                return DropDown;
            }
        }

        protected override void SetImmediatePostDataCompanionScript(string script)
        {            
        }        

        protected override object GetControlValueCore()
        {       
            return ObjectSpace.GetObject(IsSelector ? DataSelector.Value : DropDown.Value);
        }
        
        protected override void ReadEditModeValueCore()
        {
            if (IsSelector)
                DataSelector.Value = PropertyValue;
            else
                DropDown.Value = PropertyValue;
        }

        protected override void ReadViewModeValueCore()
        {
            if (IsSelector)
                DataSelector.Value = PropertyValue;
            else
                DropDown.Value = PropertyValue;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            base.BreakLinksToControl(unwireEventsOnly);            
        }

        public string GetImmediatePostDataScript()
        {
            return Helpers.GetImmediatePostDataScript(MemberInfo);
        }
    }
}
