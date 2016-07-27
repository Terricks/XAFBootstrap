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
using DevExpress.Persistent.Base.General;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapTableEditor
{
    public interface ITreeNodeGlyphicon
    {
        String Glyphicon { get; }
    }

    [ListEditor(typeof(DevExpress.Persistent.Base.General.ITreeNode), true)]
    public class XafBootstrapTreeEditor : XafBootstrapTableEditor
    {
        public Boolean IsExpanded = false;
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
                    if (GetExpanded(item))                        
                        CalcNestedSelectedObjects((item as ITreeNode), SelectedObjects.Contains(item));
            }
        }

        private IList<ITreeNode> GetChildren(ITreeNode obj)
        {
            IList<ITreeNode> list = new List<ITreeNode>();
            foreach(ITreeNode item in collection.List)
            {
                if (item.Parent == obj)
                    list.Add(item);
            }
            return list;
        }

        public virtual void CalcNestedSelectedObjects(ITreeNode obj, Boolean parentStatus)
        {
            if (obj == null)
                return;
            var children = GetChildren(obj);            
            foreach (ITreeNode child in children)
            {
                var status = false;                
                if (parentStatus == !MarkedObjects.Contains(String.Concat(GetMemberValue((child as Object), collection.ObjectSpace.GetKeyPropertyName(child.GetType())))))
                {
                    SelectedObjects.Add(child as Object);
                    status = true;
                }               
                CalcNestedSelectedObjects(child, status);
            }

        }

        public void DeleteSelectedObject(ITreeNode selectedObject)
        {            
            if (selectedObject != null)
            {
                var children = GetChildren(selectedObject);
                foreach (ITreeNode nestedObject in children)
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
                if (control == null)
                    return new List<String>();
                if (control.ClientID == null)
                    return new List<String>();
                if (Helpers.Session[control.ID + "_ExpandedItems"] == null)
                    Helpers.Session[control.ID + "_ExpandedItems"] = new List<String>();
                return Helpers.Session[control.ID + "_ExpandedItems"] as IList<String>;
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

                        CalcSelectedObjects();
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
                            int lvl = 0;
                            GetLevel(obj, ref lvl);
                            var spacing = (lvl * 15);
                            var glyphicon = "";
                            if (obj is ITreeNodeGlyphicon)
                            {
                                glyphicon = (obj as ITreeNodeGlyphicon).Glyphicon;
                                if (String.Concat(glyphicon) != "")
                                    glyphicon = String.Format("<span class=\"{0}\"></span> ", glyphicon);
                            }

                            var expandScript = "<td onclick=\"if (!e) var e = event || window.event; var posX = $(this).offset().left; if (e.pageX - posX < 30 + " + spacing + ") {{{{ {0}; e.cancelBubble = true; }}}};\"";
                            
                            if ((DataSource as IList).OfType<Object>().Where(f => (f as ITreeNode).Parent != null && (String.Concat(GetMemberValue((f as ITreeNode).Parent as Object, collection.ObjectSpace.GetKeyPropertyName(f.GetType()))) == String.Concat(GetMemberValue((obj as Object), collection.ObjectSpace.GetKeyPropertyName(obj.GetType()))))).Count() > 0)
                            {
                                if (GetExpanded(obj))                                
                                {
                                    Format = Format.Replace("<td ", String.Format(expandScript, XafCallbackManager.GetScript(control.ClientID, String.Format("'Expand={0}'", GetMemberValue((data as Object), collection.ObjectSpace.GetKeyPropertyName(data.GetType()))))));
                                    Value = "</span><span class=\"glyphicon glyphicon-chevron-down glyphicon-sm\" > </span>" + glyphicon + Value;
                                }
                                else
                                {
                                    Format = Format.Replace("<td ", String.Format(expandScript, XafCallbackManager.GetScript(control.ClientID, String.Format("'Expand={0}'", GetMemberValue((data as Object), collection.ObjectSpace.GetKeyPropertyName(data.GetType()))))));
                                    Value = "</span><span class=\"glyphicon glyphicon-chevron-right glyphicon-sm\" > </span>" + glyphicon + Value;
                                }
                            }
                            else
                                Value = "<span class=\"glyphicon glyphicon-sm\"></span> " + glyphicon + Value;
                                                        
                            Value = "<span style=\"margin-left: " + (spacing) + "px\"></span>" + Value;
                        }
                        else
                        {
                            Boolean hasChildren = false;
                            foreach(var listObj in collection.List)
                            {
                                if ((listObj as ITreeNode).Parent == obj)
                                {
                                    hasChildren = true;
                                    break;
                                }
                            }
                            if (hasChildren)
                            {
                                if (HideGroupData && control.CustomColumns.Where(f => f.FieldName == FieldName).Count() == 0)
                                    Value = "";
                            }
                        }
                    }
                });

            control.Init += Control_Init;

            return control;
        }

        private void Control_Init(object sender, EventArgs e)
        {
            AssignDataSourceToControl(collection.List);
        }

        protected override void OnControlsCreated()
        {
            base.OnControlsCreated();            
        }

        private Boolean GetExpanded(object item)
        {
            Boolean condition = ExpandedItems.IndexOf(String.Concat(GetMemberValue(item, collection.ObjectSpace.GetKeyPropertyName(item.GetType())))) > -1;
            return ((condition && !IsExpanded) || (!condition && IsExpanded));
        }

        public void MakeSortedList(Object parent, ref IList<Object> List, IList<Object> source, Boolean ForceBuild = false)
        {
            foreach (var item in source.Where(f => ForceBuild || (f as ITreeNode).Parent == parent))
            {
                List.Add(item);
                if (GetExpanded(item))
                    MakeSortedList(item, ref List, source);
            }
        }

        public void MakeRootList(ref IList<Object> List, IList<Object> root, IList<Object> source)
        {
            foreach (var item in root)
            {
                List.Add(item);
                if (GetExpanded(item))
                    MakeSortedList(item, ref List, source);
            }
        }

        protected override IList GetData(object data)
        {
            var List = base.GetData(data);

            var rootList = new List<Object>();
            var objectsList = List.OfType<Object>();
            var key = "";
            var keys = objectsList.Select(f => String.Concat(GetMemberValue(f, collection.ObjectSpace.GetKeyPropertyName(f.GetType())))).ToList();
            foreach (ITreeNode f in List)
            {
                if (key == "")
                    key = collection.ObjectSpace.GetKeyPropertyName(f.GetType());
                var add = f.Parent == null
                    || (f.Parent != null && keys.IndexOf(String.Concat(GetMemberValue((f.Parent as Object), key))) == -1);
                if (add)
                    rootList.Add(f);
            }            

            IList<Object> sortedList = new List<Object>();
            MakeRootList(ref sortedList, rootList as IList<Object>, List as IList<Object>);
            return sortedList as IList;
        }
    }
}
