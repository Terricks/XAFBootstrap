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
using DevExpress.Persistent.Base.General;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapTableEditor
{
    [ListEditor(typeof(DevExpress.Persistent.Base.General.ITreeNode), true)]
    public class XafBootstrapTreeEditor : XafBootstrapTableEditor
    {
        public bool HideGroupData = true; 
        public XafBootstrapTreeEditor(IModelListView info)
            : base(info)
        {
        }

        protected override void CalcSelectedObjects()
        {
            base.CalcSelectedObjects();
            if (!ListView.Id.ToLower().Contains("lookup"))
            {
                var rootObjects = collection.List.OfType<ITreeNode>().Where(f => f.Parent == null || !collection.List.Contains(f.Parent)).OfType<Object>().ToList();
                SelectedObjects = SelectedObjects.OfType<ITreeNode>().Where(f => f.Parent == null || !collection.List.Contains(f.Parent)).OfType<Object>().ToList();
                foreach (var item in rootObjects)
                    CalcNestedSelectedObjects((item as ITreeNode), SelectedObjects.Contains(item));
            }
        }

        public virtual void CalcNestedSelectedObjects(ITreeNode obj, Boolean parentStatus)
        {
            if (obj == null)
                return;
            foreach (ITreeNode child in obj.Children)
            {
                var status = false;
                if (collection.List.IndexOf(child) > -1)
                {
                    if (parentStatus == !MarkedObjects.Contains(String.Concat(GetMemberValue((child as Object), collection.ObjectSpace.GetKeyPropertyName(child.GetType())))))
                    {
                        SelectedObjects.Add(child as Object);
                        status = true;
                    }
                }
                CalcNestedSelectedObjects(child, status);
            }

        }

        public void DeleteSelectedObject(ITreeNode selectedObject)
        {            
            if (selectedObject != null)
            {
                foreach (ITreeNode nestedObject in selectedObject.Children)
                {
                    var id = String.Concat(GetMemberValue((nestedObject as Object), collection.ObjectSpace.GetKeyPropertyName(nestedObject.GetType())));
                    if (MarkedObjects.Contains(id))
                        MarkedObjects.Remove(id);
                    DeleteSelectedObject(nestedObject);
                }
            }
        }

        public IList<String> ExpandedItems
        {
            get
            {
                if (control.ClientID == null)
                    return new List<String>();
                if (Helpers.Session[control.ClientID + "_ExpandedItems"] == null)
                    Helpers.Session[control.ClientID + "_ExpandedItems"] = new List<String>();
                return Helpers.Session[control.ClientID + "_ExpandedItems"] as IList<String>;
            }
        }

        public override void DoProcessAction(string parameter)
        {
            base.DoProcessAction(parameter);
            string[] vals = String.Concat(parameter).Split(new String[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length == 2)
            {
                switch (vals[0])
                {
                    case "Expand":
                        if (ExpandedItems.IndexOf(vals[1]) == -1)
                            ExpandedItems.Add(vals[1]);
                        else
                            ExpandedItems.Remove(vals[1]);
                        control.DataSource = GetData(collection.List);
                        control.Refresh();
                        break;
                    case "Mark":
                        string[] marks = String.Concat(vals[1]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var mark in marks)
                            DeleteSelectedObject(collection.List.OfType<ITreeNode>().Where(f => String.Concat(GetMemberValue((f as Object), collection.ObjectSpace.GetKeyPropertyName(f.GetType()))) == mark).FirstOrDefault());

                        CalcSelectedObjects();
                        break;
                }
            }
        }

        public void GetLevel(ITreeNode node, ref int Level)
        {
            if (node.Parent != null)
            {
                Level++;
                GetLevel(node.Parent, ref Level);
            }
        }

        protected override object CreateControlsCore()
        {
            control = base.CreateControlsCore() as XafBootstrapTable;
            var FirstModelVisibleColumn = ListView.Columns.Where(f => f.Index >= 0).OrderBy(f => f.Index).FirstOrDefault();
            if (FirstModelVisibleColumn != null)
                control.OnGenerateCell += new OnGenerateCellHandler(delegate(ref String Format, String FieldName, ref String Value, int RowNumber, object data)
                {
                    if (data is ITreeNode)
                    {
                        var obj = (ITreeNode)data;
                        if (obj != null && FieldName == FirstModelVisibleColumn.Id)
                        {

                            if ((DataSource as IList).OfType<Object>().Where(f => (f as ITreeNode).Parent != null && (String.Concat(GetMemberValue((f as ITreeNode).Parent as Object, collection.ObjectSpace.GetKeyPropertyName(f.GetType()))) == String.Concat(GetMemberValue((obj as Object), collection.ObjectSpace.GetKeyPropertyName(obj.GetType()))))).Count() > 0)
                            {
                                if (ExpandedItems.IndexOf(String.Concat(GetMemberValue((obj as Object), collection.ObjectSpace.GetKeyPropertyName(obj.GetType())))) > -1)
                                    Value = "</span><span class=\"glyphicon glyphicon-chevron-down glyphicon-sm\" onclick=\"" + XafCallbackManager.GetScript(control.ClientID, String.Format("'Expand={0}'", GetMemberValue((data as Object), collection.ObjectSpace.GetKeyPropertyName(data.GetType())))) + "event.cancelBubble = true;\"> </span>" + Value;
                                else
                                    Value = "</span><span class=\"glyphicon glyphicon-chevron-right glyphicon-sm\" onclick=\"" + XafCallbackManager.GetScript(control.ClientID, String.Format("'Expand={0}'", GetMemberValue((data as Object), collection.ObjectSpace.GetKeyPropertyName(data.GetType())))) + "event.cancelBubble = true;\"> </span>" + Value;
                            }
                            else
                                Value = "</span><span class=\"glyphicon glyphicon-sm\"> </span><span></span>" + Value;

                            int lvl = 0;
                            GetLevel(obj, ref lvl);
                            Value = "<span style=\"margin-left: " + (lvl * 10) + "px\">" + Value;
                        }
                        else
                        {
                            if (obj.Children.Count > 0)
                            {
                                if (HideGroupData && control.CustomColumns.Where(f => f.FieldName == FieldName).Count() == 0)
                                    Value = "";
                            }
                        }
                    }
                });

            return control;
        }

        protected override void OnControlsCreated()
        {
            base.OnControlsCreated();
        }

        public void MakeSortedList(Object parent, ref IList<Object> List, IList<Object> source, Boolean ForceBuild = false)
        {
            foreach (var item in source.Where(f => ForceBuild || (f as ITreeNode).Parent == parent))
            {
                List.Add(item);
                if (ExpandedItems.IndexOf(String.Concat(GetMemberValue(item, collection.ObjectSpace.GetKeyPropertyName(item.GetType())))) > -1)
                    MakeSortedList(item, ref List, source);
            }
        }

        public void MakeRootList(ref IList<Object> List, IList<Object> root, IList<Object> source)
        {
            foreach (var item in root)
            {
                List.Add(item);
                if (ExpandedItems.IndexOf(String.Concat(GetMemberValue(item, collection.ObjectSpace.GetKeyPropertyName(item.GetType())))) > -1)
                    MakeSortedList(item, ref List, source);
            }
        }

        protected override IList GetData(object data)
        {
            var List = base.GetData(data);
            var rootList = List.OfType<ITreeNode>().Where(
                f => f.Parent == null 
                    || (f.Parent != null && List.OfType<Object>().Where(
                        w => String.Concat(GetMemberValue(w, collection.ObjectSpace.GetKeyPropertyName(w.GetType()))) == String.Concat(GetMemberValue((f.Parent as Object), collection.ObjectSpace.GetKeyPropertyName(f.Parent.GetType())))
                        ).Count() == 0)
                ).OfType<Object>().ToList();

            IList<Object> sortedList = new List<Object>();
            MakeRootList(ref sortedList, rootList as IList<Object>, List as IList<Object>);
            return sortedList as IList;
        }
    }
}
