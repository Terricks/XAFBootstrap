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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using XAF_Bootstrap.Templates;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using XAF_Bootstrap.ModelExtensions;
using DevExpress.Persistent.Base;
using System.Collections;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Xpo;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.String), "XafBootstrapTagEditor", false)]
    public class XafBootstrapTagPropertyEditor : ASPxPropertyEditor, IXafBootstrapEditor, IComplexViewItem
    {
        public XafBootstrapTagPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {             
        }

        public XafBootstrapTagSelector Edit;
        private void InitEdit()
        {   
            Edit = new XafBootstrapTagSelector();            
            Edit.PropertyName = PropertyName;            
        }

        #region IComplexViewItem Members

        public IObjectSpace ObjectSpace;
        public XafApplication Application;

        void IComplexViewItem.Setup(DevExpress.ExpressApp.IObjectSpace objectSpace, DevExpress.ExpressApp.XafApplication application)
        {
            this.ObjectSpace = objectSpace;
            this.Application = application;
        }

        #endregion

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = !AllowEdit;
            Edit.EditValueChanged += new EventHandler(EditValueChangedHandler);
            SetValues();
            return Edit;
        }

        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = true;
            SetValues();
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

        public CriteriaOperator PrepareCriteria(CriteriaOperator op)
        {
            if (op is FunctionOperator)
            {
                var funcOp = new FunctionOperator();
                for (int i = 0; i < (op as FunctionOperator).Operands.Count; i++)
                    funcOp.Operands.Add(PrepareCriteria((op as FunctionOperator).Operands[i]));
                return funcOp;
            }
            else if (op is ConstantValue)
            {
                var cnst = new ConstantValue((op as ConstantValue).Value);
                if (String.Concat((op as ConstantValue).Value).ToLower().IndexOf("@this") > -1)
                {                    
                    IMemberInfo info;
                    cnst.Value = ObjectFormatValues.GetValueRecursive((op as ConstantValue).Value.ToString().Replace("@This.", "").Replace("@This", "").Replace("@this.", "").Replace("@this", ""), CurrentObject, out info);
                }
                return cnst;
            }
            else if (op is BinaryOperator)
            {
                var binary = new BinaryOperator();
                binary.LeftOperand = PrepareCriteria((op as BinaryOperator).LeftOperand);
                binary.RightOperand = PrepareCriteria((op as BinaryOperator).RightOperand);
                return binary;
            }
            else
            {
                return op;
            }
        }

        private void SetValues()
        {
            Edit.Items.List.Clear();            
            IList<TagSourceAttribute> tagSources = MemberInfo.FindAttributes<TagSourceAttribute>().ToList();
            var settingsAttribute = MemberInfo.FindAttribute<TagSettingsAttribute>();
            if (settingsAttribute != null)
            {
                Edit.Delimeter = String.Concat(settingsAttribute.ValueSeparator);
                Edit.AllowAddCustomValues = settingsAttribute.AllowNew;
                Edit.AllowSelectValues = settingsAttribute.AllowSelect;
            }

            IMemberInfo info;
            #region Set values from tag sources
            foreach (var tagSource in tagSources)
            {
                switch (tagSource.Kind)
                {
                    case TagSourceKind.TypeSource:
                        if (typeof(Enum).IsAssignableFrom(tagSource.Type)) {
                            IList<String> Names = Enum.GetNames(tagSource.Type);
                            Array Values = Enum.GetValues(tagSource.Type);
                            String format = String.Concat(tagSource.ValueFormat);
                            if (format == "")
                                format = "{0}";

                            for (int i = 0; i < Names.Count; i++)
                            {
                                String imageName = Helpers.GetXafImageName((Enum)Values.GetValue(i));
                                if (Edit.Items.List.Where(f => f.Text == String.Format(format, Names[i])).Count() == 0)
                                {
                                    var item = Edit.Items.Add();
                                    var displayText = Helpers.GetXafDisplayName((Enum)Values.GetValue(i));
                                    item.Text = String.Format(format, displayText);
                                    item.Value = String.Concat(Values.GetValue(i));
                                    if (imageName == "")
                                        imageName = String.Concat(tagSource.ImageName);
                                }
                            }
                        } else {
                            var tagSourceType = tagSource.Type;
                            if (tagSourceType == typeof(SecuritySystemUser))
                                tagSourceType = SecuritySystem.UserType;

                            IObjectSpace os = ObjectSpace;

                            CollectionSource cs = new CollectionSource(os, tagSourceType);
                            if (tagSource.Criteria != null)
                            {
                                cs.Criteria["Criteria"] = PrepareCriteria(tagSource.Criteria);
                            }

                            String format = String.Concat(tagSource.ValueFormat);
                            if (format == "")
                            {
                                ITypeInfo TypeInfo = XafTypesInfo.Instance.FindTypeInfo(tagSourceType);
                                if (TypeInfo != null)
                                {
                                    ObjectCaptionFormatAttribute attr = TypeInfo.FindAttribute<ObjectCaptionFormatAttribute>();
                                    if (attr != null)
                                        format = attr.FormatString;
                                    else
                                    {
                                        var defPropAttr = TypeInfo.FindAttribute<XafDefaultPropertyAttribute>();
                                        if (defPropAttr != null)
                                            format = "{0:" + defPropAttr.Name + "}";
                                    }
                                }
                            }

                            IList list = null;

                            switch (String.Concat(tagSource.Sorting).ToLower())
                            {
                                case "asc":
                                case "ascending":
                                    list = cs.List.OfType<object>().OrderBy(f => String.Format(new ObjectFormatter(), format, f)).ToList();
                                    break;
                                case "desc":
                                case "descending":
                                    list = cs.List.OfType<object>().OrderByDescending(f => String.Format(new ObjectFormatter(), format, f)).ToList();
                                    break;
                                default:
                                    list = cs.List;
                                    break;
                            }

                            foreach (object obj in list)
                            {
                                var text = String.Format(new ObjectFormatter(), format, obj);
                                if (Edit.Items.List.Where(f => f.Text == text).Count() == 0)
                                {
                                    var item = Edit.Items.Add();
                                    item.Text = String.Format(new ObjectFormatter(), format, obj);
                                    item.Value = String.Concat(ObjectFormatValues.GetValueRecursive(tagSource.Key, obj, out info));
                                }
                            }
                        }                        
                        break;
                    case TagSourceKind.Values:                        
                        String Items = "";
                        try
                        {
                            Boolean IsProperty = false;
                            var property = ObjectFormatValues.GetValueRecursive(tagSource.CommaValues, CurrentObject, out info);                            
                            IObjectSpace os = ObjectSpace;

                            if (property is IEnumerable)
                            {
                                var coll = (property as IEnumerable);

                                var format = "";

                                if (info.MemberTypeInfo != null)
                                {
                                    var attr = info.MemberTypeInfo.FindAttribute<ObjectCaptionFormatAttribute>();
                                    if (attr != null)
                                        format = attr.FormatString;
                                    else
                                    {
                                        var defPropAttr = info.MemberTypeInfo.FindAttribute<XafDefaultPropertyAttribute>();
                                        if (defPropAttr != null)
                                            format = "{0:" + defPropAttr.Name + "}";
                                    }
                                }

                                Items = String.Join("*;*", coll.OfType<object>().Select(f => String.Format("{0}|{1}"
                                    , (format == "" ? f : String.Format(new ObjectFormatter(), format, f))
                                    , f is String ? String.Concat(f).Trim() : ObjectFormatValues.GetValueRecursive(os.GetKeyPropertyName(f.GetType()), f, out info))));
                                IsProperty = true;
                                                                
                                if (String.Concat(tagSource.ValueFormat) != "")
                                    format = tagSource.ValueFormat;

                                var selectCollection = coll.OfType<object>().Select(f => new
                                {
                                    text = (format == "" ? f : String.Format(new ObjectFormatter(), format, f)),
                                    value = f is String ? String.Concat(f).Trim() : ObjectFormatValues.GetValueRecursive(String.Concat(tagSource.Key) == "" ? os.GetKeyPropertyName(f.GetType()) : tagSource.Key, f, out info)
                                }).GroupBy(f => f).Select(f => new { f.Key.text, f.Key.value }).OrderBy(f => f.text);
                                
                                foreach (var item in selectCollection)
                                {
                                    var tagItem = Edit.Items.Add();
                                    tagItem.Text = String.Concat(item.text).Trim();
                                    tagItem.Value = String.Concat(item.value).Trim();
                                }
                            }
                            else
                            {
                                Items = String.Concat(property);

                                foreach (var item in String.Concat(tagSource.CommaValues).Split(',').Where(f => String.Concat(f) != ""))
                                {
                                    if (Edit.Items.List.Where(f => f.Text == item.Split('|')[0]).Count() == 0)
                                    {
                                        var tagItem = Edit.Items.Add();
                                        tagItem.Text = item.Split('|')[0];
                                        if (item.Split('|').Count() > 1)
                                            tagItem.Value = item.Split('|')[1];

                                        String imageName = tagSource.ImageName;
                                        if (item.Split('|').Count() > 2)
                                            imageName = item.Split('|')[2];
                                        if (imageName != "")
                                            tagItem.ImageUrl = DevExpress.ExpressApp.Utils.ImageLoader.Instance.GetImageInfo(tagSource.ImageName).ImageUrl;
                                    }
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            Items = tagSource.CommaValues;
                        }
                        
                        break;
                }
                foreach (var item in Edit.Items.List.ToList())
                    if (String.Concat(item.Text) == "" && String.Concat(item.Value) == "")
                        Edit.Items.List.Remove(item);
            }
            #endregion
        }
    }
}
