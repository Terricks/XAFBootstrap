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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Xpo;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraPrinting;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace XAF_Bootstrap.Editors.XafBootstrapTableEditor
{
    [ListEditor(typeof(object), true)]
    public class XafBootstrapTableEditor : ListEditor, IComplexListEditor, IXafBootstrapListEditor, IXafCallbackHandler, IProcessCallbackComplete, IExportable
    {
        protected XafBootstrapTable control;
        protected IModelListView ListView;
        protected XafApplication App;
        protected CollectionSourceBase collection;
        protected ViewEditMode _EditMode;        
        
        void IProcessCallbackComplete.ProcessCallbackComplete()
        {
            Refresh();
        }

        public ViewEditMode EditMode
        {
            get
            {
                return _EditMode;
            }
            set
            {
                _EditMode = value;
                if (control != null)
                    control.EditMode = _EditMode;
            }
        }

        public Boolean IsLookup = false;
        protected Boolean DeleteFlag = false;
        private Boolean _AllItemsChecked = false;
        public Boolean AllItemsChecked
        {
            get
            {
                return _AllItemsChecked;
            }
            set
            {
                if (value != _AllItemsChecked)
                   MarkedObjects.Clear();
                _AllItemsChecked = value;
            }
        }        

        public XafBootstrapTableEditor(IModelListView info) : base(info) {
            ListView = info;
            MarkedObjects = new List<String>();
            SelectedObjects = new List<Object>();
        }

        #region IXafCallbackHandler
        public static XafCallbackManager XafCallbackManager
        {
            get
            {
                return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            }
        }

        private void Handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            DoProcessAction(e.Parameter);
        }

        public void ProcessAction(string parameter)
        {
            DoProcessAction(parameter);
        }
        #endregion

        protected IList<String> MarkedObjects;
        protected IList<Object> SelectedObjects;

        public object GetMemberValue(Object obj, String member)
        {   
            if (obj is XPBaseObject)
                return (obj as XPBaseObject).GetMemberValue(member);
            else if (obj != null)
            {
                var prop = obj.GetType().GetProperty(member);
                if (prop != null)
                    return obj.GetType().GetProperty(member).GetValue(obj, null);
                    
            }
            return null;
        }

        private IList<Object> _collection;
        private IList<Object> GetList()
        {
            if (_collection == null || _collection.Count != collection.List.Count)
                _collection = collection.List.OfType<Object>().ToList();
            return _collection;
        }

        protected virtual void CalcSelectedObjects()
        {
            if (collection.List != null)            
                SelectedObjects = GetList().Where(
                    f => (
                        !AllItemsChecked && MarkedObjects.IndexOf(String.Concat(GetMemberValue(f, collection.ObjectSpace.GetKeyPropertyName(f.GetType())))) > -1) ||
                        (AllItemsChecked && MarkedObjects.IndexOf(String.Concat(GetMemberValue(f, collection.ObjectSpace.GetKeyPropertyName(f.GetType())))) == -1)
                    ).ToList();                
        }

        public virtual void ClearSelectedObjects()
        {
            SelectedObjects.Clear();
            MarkedObjects.Clear();
        }

        public virtual void DoProcessAction(string parameter)
        {
            string[] vals = String.Concat(parameter).Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length == 2)            
                DoProcessPairAction(vals[0], vals[1]);                            
        }

        public object GetObjectByKey(String key)
        {
            return GetList().FirstOrDefault(f => String.Concat(GetMemberValue(f, collection.ObjectSpace.GetKeyPropertyName(f.GetType()))) == key);
        }

        public virtual void DoProcessPairAction(String Action, String Param)
        {
            switch (Action)
            {
                case "View":
                    IObjectSpace os = null;

                    var obj = GetObjectByKey(Param);
                    var svp = new ShowViewParameters();

                    if (collection is PropertyCollectionSource)
                    {
                        os = collection.ObjectSpace.CreateNestedObjectSpace();
                        svp.TargetWindow = TargetWindow.NewModalWindow;
                    }
                    else
                    {
                        os = App.CreateObjectSpace(obj.GetType());
                        svp.TargetWindow = TargetWindow.Default;
                    }
                    
                    obj = os.GetObject(obj);
                    DetailView view = App.CreateDetailView(os, ListView.DetailView, true, obj);
                    view.ViewEditMode = EditMode;
                    svp.CreatedView = view;                    
                    App.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

                    break;
                case "Mark":
                    string[] marks = String.Concat(Param).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var mark in marks)
                    {
                        if (MarkedObjects.IndexOf(mark) == -1)
                            MarkedObjects.Add(mark);
                        else
                            MarkedObjects.Remove(mark);
                    }
                    control.Refresh();
                    CalcSelectedObjects();
                    OnSelectionChanged();
                    break;
            }
        }

        protected internal IObjectSpace ObjectSpace
        {
            get { return collection != null ? collection.ObjectSpace : null; }
        }        

        public void Setup(CollectionSourceBase collectionSource, XafApplication application)
        {   
            collection = collectionSource;
            _collection = null;
            App = application;
        }

        public String GetScript(String parameter, String stringConfirmation = "", bool usePostBack = false)
        {
            return Handler != null ? Handler.GetScript(parameter, stringConfirmation, usePostBack) : "";
        }

        public Boolean ShowSelector = true;

        protected override object CreateControlsCore()
        {            
            control = new XafBootstrapTable();
            control.ObjectTypeInfo = ObjectTypeInfo;
            control.ListView = ListView;
            control.ObjectSpace = ObjectSpace;
            control.ShowNumbers = false;
            
            control.OnGenerateItemClick += new OnGenerateRowEventHandler(delegate(ref String Result, int RowNumber, object Row)
            {
                var key = GetMemberValue((Row as Object), collection.ObjectSpace.GetKeyPropertyName(Row.GetType()));                
                if (!IsLookup)
                    Result = String.Format(Result, GetScript(String.Format("'View={0}'", key)));
                else
                    Result = String.Format(Result, GetScript(String.Format("'Mark={0}'", key)));
            });

            control.Init += control_Init;
            return control;
        }

        void control_Init(object sender, EventArgs e)
        {
            control.CustomColumns.Add(new XafBootstrapTable.HeaderInfo() { ID = "Selector", Align = "left", FieldName = "Selector", Caption = " ", FixedWidth = 20 });
            if (ShowSelector && (EditMode == ViewEditMode.Edit || !(collection is PropertyCollectionSource) || (collection is LookupEditPropertyCollectionSource)))
            {                
                control.OnGenerateCell += new OnGenerateCellHandler(delegate(ref String Format, String FieldName, ref String value, int RowNumber, object data)
                {
                    if (FieldName == "Selector" && control.ClientID != null)
                    {
                        var key = GetMemberValue((data as Object), collection.ObjectSpace.GetKeyPropertyName(data.GetType()));
                        Format = Format.Replace("<td ", String.Format("<td onclick=\"event.cancelBubble = true; {0}\" ", GetScript(String.Format("'Mark={0}'", key))));
                        value = String.Format("<input type=\"checkbox\" {0}></label>",
                            SelectedObjects.IndexOf(ObjectSpace.GetObject(data as Object)) > -1 ? "checked" : ""
                        );
                    }
                });

                control.OnGenerateHeader += new OnGenerateHeaderHandler(delegate(ref String Format, String FieldName, ref String value, object data)
                {
                    if (FieldName == "Selector" && control.ClientID != null)
                    {
                        AllItemsChecked = Helpers.RequestManager.Request.Form[control.ClientID + "_CheckAll"] == "on";
                        Format = Format.Replace("<th ", String.Format("<th onclick=\"event.cancelBubble = true; {0}\" ", GetScript("'Mark=CheckAllChanged'")));
                        value = String.Format("<input runat=\"server\" name=\"{1}_CheckAll\" type=\"checkbox\" {0}></label> ", (AllItemsChecked) ? "checked" : "", control.ClientID) + value;
                    }
                });
            }
            RegisterHandler();
        }

        CallbackHandler Handler;
        public virtual void RegisterHandler()
        {
            Handler = new CallbackHandler(control.ClientID);
            Handler.OnCallback += Handler_OnCallback;
        }        

        protected override void OnControlsCreated()
        {
            base.OnControlsCreated();
        }        

        protected virtual IList GetData(object data)
        {
            if (data == null)
                return null;         

            var List = ListHelper.GetList(data).OfType<Object>().ToList();
            
            if (ListView.Sorting.Count > 0)
                List = Helpers.SortListByViewSorting(Model, List).ToList();

            return List;
        }

        protected override void AssignDataSourceToControl(object dataSource)
        {   
            if (control != null)
            {
                control.DataSource = GetData(dataSource);   
            }
        }

        public override void Refresh()
        {
            if (control != null)
            {
                control.DataSource = GetData(collection.List);
                control.Refresh();
            }
            CalcSelectedObjects();
            OnSelectionChanged();
        }

        public override bool AllowEdit
        {
            get
            {
                return false;
            }
            set
            {
            }            
        }
        
        public override object FocusedObject
        {
            get
            {
                if (SelectedObjects.Count == 1)
                    return SelectedObjects[0];
                return null;
            }            
        }
        
        public override SelectionType SelectionType
        {
            get { return SelectionType.MultipleSelection; }
        }

        public override IList GetSelectedObjects()
        {
            SelectedObjects = SelectedObjects.Select(f => ObjectSpace.GetObject(f)).ToList();
            return SelectedObjects as IList;
        }

        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate
           ContextMenuTemplate
        {
            get { return null; }
        }

        #region Printable

        public IPrintable Printable
        {
            get { aspxGridViewExporter = new ASPxGridViewExporter(); return aspxGridViewExporter; }
        }

        public event EventHandler<PrintableChangedEventArgs> PrintableChanged;

        public void OnExporting()
        {
            var printableEditor = new ASPxGridListEditor(ListView);
            printableEditor.CreateControls();
            printableEditor.Setup(collection, App);
            printableEditor.Grid.Visible = false;
            aspxGridViewExporter.Page = control.Page;
            control.Controls.Add(printableEditor.Grid);
        }
        private ASPxGridViewExporter aspxGridViewExporter;

        public List<ExportTarget> SupportedExportFormats
        {
            get
            {
                if (Printable == null)
                {
                    return new List<ExportTarget>();
                }
                else
                {
                    return new List<ExportTarget>() {
                        ExportTarget.Csv,
                        ExportTarget.Html,
                        ExportTarget.Image,
                        ExportTarget.Mht,
                        ExportTarget.Pdf,
                        ExportTarget.Rtf,
                        ExportTarget.Text,
                        ExportTarget.Xls,
                        ExportTarget.Xlsx
                    };
                }
            }
        }

        #endregion

        public override void SaveModel() { }
    }
}
