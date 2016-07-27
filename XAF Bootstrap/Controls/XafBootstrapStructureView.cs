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
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using XAF_Bootstrap.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web;
using XAF_Bootstrap.Templates;
using DevExpress.ExpressApp.SystemModule;
using System.Drawing;
using DevExpress.ExpressApp.DC;
using System.Text;
using Newtonsoft.Json.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security.Strategy;
using XAF_Bootstrap.Editors.XafBootstrapTableEditor;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapStructureView : Control, IXafCallbackHandler
    {
        private Control Root;
        public object DataSource;
        public CollectionSourceBase Collection;
        public IObjectSpace ObjectSpace = null;
        public ViewEditMode EditMode;        
        public XafBootstrapStructureListEditor Editor;
        
        public XafBootstrapStructureView()
        {
            Root = new Control();
            Controls.Add(Root);
        }

        #region IXafCallbackHandler
        public static XafCallbackManager XafCallbackManager
        {
            get
            {
                return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            }
        }

        public void ProcessAction(string parameter)
        {
            var paramsList = String.Concat(parameter).Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (paramsList.Length > 0)
            {                
            }
        }        
        #endregion
        
        public void Refresh()
        {
            BuildData();            
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        CallbackHandler handler;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            handler = new CallbackHandler(ClientID);
            handler.OnCallback += Handler_OnCallback;            
            BuildData();
        }

        public object GetMemberValue(object obj, String member)
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

        private String GetKey(object f)
        {            
            return String.Concat(GetMemberValue(f, String.Concat(Collection.ObjectSpace.GetKeyPropertyName(f.GetType()))));
        }

        private ITreeNode FindSelectable(String oid, IEnumerable<ITreeNode> list = null)
        {
            if (list == null)
                list = Collection.List.OfType<ITreeNode>();
            var result = list.FirstOrDefault(f => GetKey(f) == oid);
            if (result == null)
            {
                foreach(var item in list.OfType<ITreeNode>())
                {
                    if (item.Children.Count > 0) {
                        var inner = FindSelectable(oid, item.Children as IEnumerable<ITreeNode>);
                        if (inner != null)
                            return inner;
                    }
                }
            }
            return result;
        }

        private void Handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            var paramsList = String.Concat(e.Parameter).Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (paramsList.Length > 1) 
                switch(paramsList[0])
                {
                    case "navigate":
                        if (String.Concat(Helpers.Session["XafBootstrapStructureView"]) == String.Concat(paramsList[1]))
                        {
                            if (currentStart != null)
                                Helpers.Session["XafBootstrapStructureView"] = (currentStart.Parent != null ? GetKey(currentStart.Parent) : "");
                            else
                                Helpers.Session["XafBootstrapStructureView"] = "";
                        }
                        else
                            Helpers.Session["XafBootstrapStructureView"] = paramsList[1];
                        BuildData();
                        break;
                    case "select":
                        /*var frame = Helpers.Session["XafBootstrapStructureViewFrame"] as Frame;
                        if (frame != null)
                        {
                            var controller = frame.GetController<DialogController>();
                            if (controller != null)
                            {
                                Selected = FindSelectable(paramsList[1]);
                                if (Editor != null)
                                    Editor.InvokeSelectionChanged();                                
                                controller.AcceptAction.DoExecute();
                                WebWindow.CurrentRequestWindow.RegisterStartupScript("CloseThisModal", "closeThisModal()");
                            }
                        }*/
                        var app = (WebApplication.Instance as XafApplication);
                        if (app != null)
                        {
                            Selected = FindSelectable(paramsList[1]);
                            var os = app.CreateObjectSpace();
                            var item = os.GetObject(Selected);
                            var view = app.CreateDetailView(os, item);
                            var SVP = new ShowViewParameters(view);
                            app.ShowViewStrategy.ShowView(SVP, new ShowViewSource(null, null));
                        }
                        break;
                }
        }

        public ITreeNode Selected
        {
            get; set;
        }

        private String GetHandlerScript(ITreeNode structure)
        {
            return (structure.Children.Count == 0 ? handler.GetScript(String.Format("'select|{0}'", GetKey(structure))) : handler.GetScript(String.Format("'navigate|{0}'", GetKey(structure))));
        }

        private String GetFormattedStructure(ITreeNode structure)
        {
            var glyph = (structure as ITreeNodeGlyphicon);
            var ret = String.Format(@"<div class='pull-left'>
                        <button type='button' style='font-size: 75%' class='btn btn-xs' onclick=""{4}""><span class=""glyphicon glyphicon-pencil"" style=""opacity: 0.75""></span> <span class=""{0}"" style=""opacity: 0.75""></span></button>
                    </div><a href=""javascript:;"" onclick=""{3}""> {1}</a>
                    
                    "
                , glyph != null ? glyph.Glyphicon : ""                
                , structure.Name
                , structure.Children.Count > 0 ? "<button type='button' class='btn btn-xs'><span class='glyphicon glyphicon-hand-right'></span></button>" : ""
                , GetHandlerScript(structure)
                , handler.GetScript(String.Format("'select|{0}'", GetKey(structure)))
            );
            return ret;
        }

        private String GetPathStructure(ITreeNode structure)
        {
            var ret = "";
            var curStruct = structure;

            var glyph = (structure as ITreeNodeGlyphicon);

            while (curStruct != null) {
                ret = String.Format(@"                    
                    <a href=""javascript:;"" onclick=""{1}"" class=""label {3}"" style=""margin-top: 2px""><span class=""{2}"" style=""opacity: 0.5""></span> {0}</a> 
                    <button type='button' style='font-size: 75%' class='btn btn-xs' onclick=""{4}""><span class=""glyphicon glyphicon-pencil"" style=""opacity: 0.75""></span></span></button>
                    "

                    , curStruct.Name
                    , GetHandlerScript(curStruct)
                    , glyph != null ? glyph.Glyphicon : ""
                    , (currentStart != null && GetKey(curStruct) == GetKey(currentStart) ? "label-info" : "label-primary")
                    , handler.GetScript(String.Format("'select|{0}'", GetKey(curStruct)))
                ) + ret;
                curStruct = curStruct.Parent;
            }
            
            return ret;
        }

        private ITreeNode currentStart
        {
            get
            {
                var current = String.Concat(Helpers.Session["XafBootstrapStructureView"]);
                if (current != "")
                    return Collection.List.OfType<ITreeNode>().FirstOrDefault(f => GetKey(f) == current);
                return null;
            }
        }                

        public void BuildData()
        {
            Root.Controls.Clear();
            
            var collection = Collection.List.OfType<ITreeNode>().ToList();
            if (collection != null)
            {
                var form = Helpers.RequestManager.Request.Form;                

                var builder = new StringBuilder();                

                var structuresList = (currentStart != null ? (new ITreeNode[] { currentStart }).ToList() : collection.Where(f => f.Parent == null || collection.IndexOf(f.Parent) == -1).ToList());                

                var structures = new StringBuilder();

                foreach (var structure in structuresList)
                {   
                    var nested = structure.Children.OfType<ITreeNode>();                

                    structures.AppendFormat(@"<br>{0}<hr/>", GetPathStructure(structure));                        
                    structures.AppendFormat(@"<ul class=""list-inline row nomargins"">");

                    var liStyle = @"<li style='vertical-align: top;' class='col-sm-4'>";                    

                    if (nested.Count() > 0)
                    {
                        foreach (var sub1 in nested)
                        {
                            structures.AppendFormat(liStyle);
                            var subNested = sub1.Children.OfType<ITreeNode>().ToList();
                            if (subNested.Count > 0)
                            {                                
                                structures.AppendFormat(@"<h6><b>{0}</b></h6><hr class='nopaddings nomargins'/>", GetFormattedStructure(sub1));
                                if (currentStart != null)
                                    foreach (var sub2 in subNested)
                                        structures.AppendFormat(@"<h6>{0}</h6>", GetFormattedStructure(sub2));
                            }
                            else
                            {
                                structures.AppendFormat(@"<h6>{0}</h6>", GetFormattedStructure(sub1));                                
                            }

                            structures.AppendFormat(@"</li>");
                        }
                    };

                    structures.AppendFormat(@"</ul>");
                }                

                builder.AppendFormat(@"{0}", structures.ToString());

                Root.Controls.Add(new HTMLText() { Text = builder.ToString() });
                
            }
        }
    }
}

