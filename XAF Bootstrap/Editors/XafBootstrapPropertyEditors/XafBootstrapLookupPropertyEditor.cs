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
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.DC;
using System.Linq.Expressions;
using System.Reflection;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    public enum BootstapLookupKind
    {
        Auto,
        Selector,
        DropDown
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class BootstrapLookupAttribute : Attribute
    {
        private BootstapLookupKind _Kind;

        public BootstrapLookupAttribute(BootstapLookupKind Kind)
        {
            _Kind = Kind;
        }

        public BootstapLookupKind Kind
        {
            get { return _Kind; }
        }
    }

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
            NewObjects = new List<object>();
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

        IList<object> NewObjects;

        private void XafBootstrapDropdownEdit_AddNew(object sender, EventArgs e)
        {   
            var os = CollectionOS;
            os.Owner = null;
            var memberType = MemberInfo.MemberType;
            if (typeof(SecuritySystemUser).IsAssignableFrom(memberType))
                memberType = SecuritySystem.UserType;
            var obj = os.CreateObject(memberType);
            var view = Application.CreateDetailView(os, obj, false);
            var SVP = new ShowViewParameters(view);
            SVP.TargetWindow = TargetWindow.NewModalWindow;

            DialogController dc = Application.CreateController<DialogController>();            
            dc.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(delegate
            {
                var item = obj;
                NewObjects.Add(obj);
                os.CommitChanges();
                if (IsSelector)
                {
                    DataSelector.Value = item;
                    PropertyValue = ObjectSpace.GetObject(item);
                    DataSelector.Value = item;
                }
                else
                {
                    if (cs != null && cs.List != null)
                        cs.List.Add(item);
                    PropertyValue = ObjectSpace.GetObject(item);
                    AddObjToDropDown(item);
                    DropDown.Value = item;
                    DropDown.InnerRender();
                }                
                CreateButtons();
                EditValueChangedHandler(null, EventArgs.Empty);
            });            
            SVP.Controllers.Add(dc);
            
            Application.ShowViewStrategy.ShowView(SVP, new ShowViewSource(null, null));
        }

        private void XafBootstrapDropdownEdit_Remove(object sender, EventArgs e)
        {
            PropertyValue = null;
            CreateButtons();
            if (IsSelector)
            {
                DataSelector.Value = null;
                DataSelector.RegisterOnClickScript();
            }
            else
            {
                DropDown.Value = null;
                DropDown.RegisterOnClickScript();                
            };
        }

        private void CreateButtons()
        {
            if (IsSelector)
            {
                DataSelector.Buttons.Buttons.Clear();
                if (PropertyValue == null)
                {
                    if (ObjectSpace.CanInstantiate(MemberInfo.MemberType) && SecuritySystem.IsGranted(new ClientPermissionRequest(MemberInfo.MemberType, null, null, SecurityOperations.Create)))
                        DataSelector.Buttons.AddButton("<span class='glyphicon glyphicon-plus text-info'></span>").OnExecution += XafBootstrapDropdownEdit_AddNew;
                }
                else
                    DataSelector.Buttons.AddButton("<span class='glyphicon glyphicon-trash text-danger'></span>").OnExecution += XafBootstrapDropdownEdit_Remove;
            } else
            {
                DropDown.Buttons.Buttons.Clear();
                if (PropertyValue == null)
                {
                    if (ObjectSpace.CanInstantiate(MemberInfo.MemberType) && SecuritySystem.IsGranted(new ClientPermissionRequest(MemberInfo.MemberType, null, null, SecurityOperations.Create)))
                        DropDown.Buttons.AddButton("<span class='glyphicon glyphicon-plus text-info'></span>").OnExecution += XafBootstrapDropdownEdit_AddNew;
                }
                else
                    DropDown.Buttons.AddButton("<span class='glyphicon glyphicon-trash text-danger'></span>").OnExecution += XafBootstrapDropdownEdit_Remove;
            }
            
        }

        CollectionSourceBase cs;
        LookupEditorHelper Helper;

        IObjectSpace CollectionOS;

        private void InitEdit()
        {
            var memType = MemberInfo.MemberTypeInfo;
            if (memType.Type == typeof(SecuritySystemUser))
                memType = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.UserType);

            CollectionOS = ObjectSpace.CreateNestedObjectSpace();

            Helper = new LookupEditorHelper(Application, CollectionOS, memType, Model);

            var bootstrapLookup = MemberInfo.FindAttribute<BootstrapLookupAttribute>();

            Boolean IsAuto = Helper.EditorMode == LookupEditorMode.Auto;
            if (Helper.EditorMode == LookupEditorMode.Auto)
                Helper.EditorMode = LookupEditorMode.AllItemsWithSearch;

            IsSelector = Helper.EditorMode == LookupEditorMode.AllItems;

            if (bootstrapLookup != null)
            {
                switch(bootstrapLookup.Kind) {
                    case BootstapLookupKind.Auto:
                        IsSelector = false;
                        Helper.EditorMode = LookupEditorMode.Auto;
                        break;
                    case BootstapLookupKind.DropDown:
                        IsSelector = false;
                        Helper.EditorMode = LookupEditorMode.AllItemsWithSearch;
                        break;
                    case BootstapLookupKind.Selector:
                        IsSelector = true;
                        Helper.EditorMode = LookupEditorMode.AllItemsWithSearch;
                        break;
                }
            }

            if (!IsSelector)
            {
                cs = Helper.CreateCollectionSource(Helper.ObjectSpace.GetObject(CurrentObject));
                if (IsAuto || (Helper.EditorMode == LookupEditorMode.AllItemsWithSearch && bootstrapLookup == null))
                    IsSelector = cs != null && cs.List != null && (cs.List.Count == 0 || cs.List.Count > 20 || typeof(ITreeNode).IsAssignableFrom(MemberInfo.MemberType));
            }

            if (cs != null && cs.List != null)
                foreach (var item in NewObjects)
                    cs.List.Add(cs.ObjectSpace.GetObject(item));

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
                DataSelector.ListView = (IModelListView)App.Model.Views[App.FindLookupListViewId(memType.Type)];                
                DataSelector.DisplayFormat = displayFormat;
                DataSelector.OnCallback += DataSelector_OnCallback;
                DataSelector.OnOk += DataSelector_OnOk;
                DataSelector.OnCancel += DataSelector_OnCancel;
            }
            else
            {
                DropDown = new XafBootstrapDropdownEdit();
                DropDown.OnClickScript = GetImmediatePostDataScript();

                if (cs != null && cs.List != null)
                {
                    var list = cs.List.OfType<object>().ToList();
                    var modelListView = (IModelListView)App.Model.Views[App.FindLookupListViewId(memType.Type)];
                    if (modelListView != null && modelListView.Sorting.Count > 0)
                    {
                        DropDown.SortByText = false;
                        list = Helpers.SortListByViewSorting(modelListView, list).ToList();
                    }
                    foreach (var obj in list)
                        AddObjToDropDown(obj);
                }
            }

            CreateButtons();
        }

        private void DataSelector_OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        private void DataSelector_OnOk(object sender, EventArgs e)
        {
            OnOk();            
        }

        public virtual void OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {

        }

        public virtual void OnOk()
        {

        }

        public virtual void OnCancel()
        {

        }

        private void DataSelector_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            OnCallback(source, e);
        }

        public dynamic Cast(object obj, Type t)
        {
            if (obj is IConvertible)
            {
                return Convert.ChangeType(obj, t) as dynamic;
            }
            else
            {
                try
                {
                    var param = Expression.Parameter(typeof(object));
                    return Expression.Lambda(Expression.Convert(param, t), param)
                        .Compile().DynamicInvoke(obj) as dynamic;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }


        private void AddObjToDropDown(object obj)
        {           
            var memType = MemberInfo.MemberTypeInfo;
            if (memType.Type == typeof(SecuritySystemUser))
            {
                memType = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.UserType);
                try {
                    obj = Cast(obj, memType.Type);
                } catch(Exception e)
                {
                    return;
                }
            }

            String displayFormat = String.Concat(DisplayFormat);
            if (displayFormat == "")
                try
                {
                    ObjectCaptionFormatAttribute aAttr = (memType.FindAttribute<ObjectCaptionFormatAttribute>());
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

            var item = DropDown.Items.Add();
            var App = (WebApplication.Instance as XafApplication);
            var listView = (IModelListView)App.Model.Views[App.FindLookupListViewId(memType.Type)];
            var cols = listView.Columns.Where(f => f.Index == null || f.Index > -1).OrderBy(f => f.Index);
            if (cols.Count() > 1)
            {
                var builder = new List<String>();
                foreach (var col in cols)
                {
                    builder.Add(String.Format(new ObjectFormatter(), "{0:" + col.GetValue<String>("FieldName") + "}", obj));
                }
                builder = builder.Where(f => String.Concat(f) != "").ToList();
                item.Text = builder.First();
                item.Hint = String.Join("<br>", builder.Skip(1).Take(builder.Count - 1));
            }
            else
            {
                item.Text = String.Format(new ObjectFormatter(), String.Concat(displayFormat) == "" ? "{0}" : displayFormat, obj);
            }
            item.Value = obj;
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
