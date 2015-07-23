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

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Xpo;
using XafBootstrap.Web;
using XAF_Bootstrap.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XAF_Bootstrap.Controls
{
    public enum SelectionObjectType
    {        
        SingleObject,
        MultipleObject,
        None,
        Browsing
    }

    public delegate void OnGenerateRowEventHandler(ref String Result, int RowNumber, object Row);
    public delegate void OnGenerateRowFinishedHandler(ref String Result, ref Control ResultControl, int RowNumber, object Row);
    public delegate void OnGenerateCellHandler(ref String Format, String FieldName, ref String value, int RowNumber, object data);
    public delegate void OnGenerateHeaderHandler(ref String Format, String FieldName, ref String value, object data);

    public class XafBootstrapTable : XafBootstrapBaseControl
    {
        public event EventHandler OnBrowse;

        public event OnGenerateRowEventHandler OnGenerateItemClick;
        public event OnGenerateRowFinishedHandler OnGenerateRowFinished;
        public event OnGenerateCellHandler OnGenerateCell;
        public event OnGenerateHeaderHandler OnGenerateHeader;
        public ViewEditMode EditMode;

        public CallbackHandler Handler;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Handler = new CallbackHandler(ClientID + "_handler");
            Handler.OnCallback += Handler_OnCallback;
            InnerRender();
        }        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            
        }

        public IModelListView ListView;
        public ITypeInfo ObjectTypeInfo;
        public IObjectSpace ObjectSpace;
        public object DataSource;

        public IList<HeaderInfo> HeaderColumns;
        public IList<HeaderInfo> CustomColumns;

        public int PageItemsCount = 20;
        public int PagesCount = 0;
        public Boolean IsFloatThead = true;

        private bool _ShowNumbers = true;
        [SaveState]
        public bool ShowNumbers
        {
            get { return _ShowNumbers; }
            set { _ShowNumbers = value; }
        }
        
        private int _PageIndex = 0;
        [SaveState]
        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }

        public int PagerButtonsCount = 7;

        private ASPxLabel Scripts;
        public StaticEditClientSideEvents ClientSideEvents;

        #region Selection
        
        public SelectionObjectType SelectionType;
        public event EventHandler SelectedObjectChanged;
        
        private IList<object> _SelectedObjects;
        [SaveState]
        public IList<object> SelectedObjects
        {
            get { return _SelectedObjects; }
            set { _SelectedObjects = value; }
        }

        #endregion

        private Control Content;

        public XafBootstrapTable()
        {
            SelectionType = SelectionObjectType.None;
            SelectedObjects = new List<object>();
            CustomColumns = new List<HeaderInfo>();

            Content = new Control();
            Controls.Add(Content);

            ClientSideEvents = new StaticEditClientSideEvents();
        }

        public class TableSettings
        {
            public int PageItemsCount = 20;
            public int PagesCount = 0;
            public int PageIndex = 0;
            public int PagerButtonsCount = 7;
            public IList<object> SelectedObjects;
        }

        public void Refresh()
        {
            InnerRender();
        }

        public void AddStringContent(String content)
        {
            HTMLText control = new HTMLText();
            control.Text = content;
            Content.Controls.Add(control);
        }

        public void AddContent(Control control)
        {
            Content.Controls.Add(control);
        }

        public class HeaderInfo
        {
            public String FieldName;
            public String ID;
            public String Align = "left";
            public IModelColumn Model;
            public String Caption;
            public int Index;
            public int MaxWidth = -1;
            public int FixedWidth = -1;
            public int MinWidth = -1;
        }

        public String FormatValue(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber)
        {
            String Value = "";
            if (memberInfo != null && objValue != null)
            {
                if (objValue is System.Drawing.Bitmap)
                {
                    ASPxImage img = new ASPxImage();
                    if (columnModel != null)
                    {
                        if (columnModel.ImageEditorCustomHeight > 0)
                            img.Height = columnModel.ImageEditorCustomHeight;
                        if (columnModel.ImageEditorFixedHeight > 0)
                            img.Height = columnModel.ImageEditorFixedHeight;
                    }
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    (objValue as System.Drawing.Bitmap).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] imageBytes = stream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    img.ImageUrl = "data:image/png;base64," + base64String;
                    img.CssClass = "img-responsive";
                    Value = Helpers.ContentHelper.RenderControl(img);
                }
                else                
                if (objValue is Enum)
                {
                    Value = Helpers.GetXafDisplayName((Enum)objValue);
                }
                else
                {
                    if (objValue is Boolean)
                    {
                        if ((Boolean)objValue)
                            Value = "<span class=\"glyphicon glyphicon-ok\"></span>";
                    }
                    else
                    {
                        String displayFormat = "{0}";
                        if (columnModel != null)
                        {
                            var valuePath = columnModel.GetValue<String>("FieldName").Split('.');
                            if (!(objValue is XPBaseObject) && !(objValue is String) && !objValue.GetType().IsPrimitive)
                                if (valuePath.Length > 1)
                                {
                                    IMemberInfo mInfo;
                                    var val = ObjectFormatValues.GetValueRecursive(String.Join(".", valuePath.Skip(1).Take(valuePath.Length -1)), objValue, out mInfo);
                                    if (val != new object())
                                        objValue = String.Concat(val);
                                }

                            if (String.Concat(columnModel.DisplayFormat) != "")
                                displayFormat = columnModel.DisplayFormat;
                            else
                                if (memberInfo.MemberTypeInfo != null)
                                {
                                    var attr = memberInfo.MemberTypeInfo.FindAttribute<ObjectCaptionFormatAttribute>();
                                    if (attr != null)
                                    {
                                        displayFormat = attr.FormatString;
                                        Value = String.Format(new ObjectFormatter(), displayFormat, objValue);
                                    }
                                };                            
                        }
                            
                        if (Value == "")                            
                            Value = String.Format(displayFormat, objValue);

                        if (memberInfo.MemberTypeInfo != null && memberInfo.MemberTypeInfo.IsDomainComponent && columnModel != null)
                            Value = String.Format(@"<a href=""javascript:;"" onclick=""event = event || window.event; event.stopPropagation(); {0}"">{1}</a>", Handler.GetScript(String.Format("'BrowseObject|{0}|{1}'", RowNumber, columnModel.PropertyName)), Value);
                    }
                }
            }
            return Value;
        }        

        public Boolean TableHover = true;
        public Boolean TableStriped = true;
        public Boolean ShowHeaders = true;

        public void InnerRender()
        {
            if (Handler == null)
            {
                Handler = new CallbackHandler(ClientID + "_handler");
                Handler.OnCallback += Handler_OnCallback;
            }

            Content.Controls.Clear();
            InnerLoadControlState();            

            AddStringContent("<div class='table-responsive panel panel-default'>");
            AddStringContent(String.Format(@"<table class='table {1} {2} xaf-bootstrap' id = '{0}_Table'>"
                , ClientID
                , TableHover ? "table-hover" : ""
                , TableStriped ? "table-striped" : ""));

            ASPxLabel script = null;
            if (IsFloatThead)
            {
                script = new ASPxLabel() { };
                script.EncodeHtml = false;
                AddContent(script);
            }

            int ItemsCount = 0;

            #region Build Header Row
            //Make headers
            HeaderColumns = new List<HeaderInfo>();            

            //Counter column
            if (ShowNumbers)
                HeaderColumns.Add(new HeaderInfo() { FixedWidth = 20, Align = "right", Caption = "#", FieldName = "SystemColumn_RowCounter", ID = "SystemColumn_RowCounter", Index = 0 });            

            foreach (var item in CustomColumns)            
                HeaderColumns.Add(item);            

            int ColumnIdx = 0;
            if (ListView != null)
                foreach (var column in ListView.Columns)
                {
                    var columnIsVisible = true;
                    #region CALCULATE ELEMENT APPEARANCE                
                    if (ObjectTypeInfo != null && ObjectSpace != null) {
                        var member = ObjectTypeInfo.FindMember(column.Id);
                        if (member != null)
                            foreach (var appearanceItem in member.FindAttributes<AppearanceAttribute>().Where(f => (String.Concat(f.Context) == "" || String.Concat(f.Context) == "Any" || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("ListView") > -1)))
                            {
                                if (Convert.ToBoolean(ObjectSpace.GetExpressionEvaluator(ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(null)))
                                {
                                    switch (appearanceItem.Visibility)
                                    {
                                        case ViewItemVisibility.ShowEmptySpace:
                                        case ViewItemVisibility.Hide:
                                            columnIsVisible = false;
                                            break;
                                        case ViewItemVisibility.Show:
                                            columnIsVisible = true;
                                            break;
                                    }
                                }
                            }

                        foreach (var appearanceItem in ObjectTypeInfo.FindAttributes<AppearanceAttribute>().Where(f => String.Concat(f.TargetItems).Split(',').Where(s => s.Trim() == column.Id).Count() > 0 && (String.Concat(f.Context) == "" || String.Concat(f.Context) == "Any" || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("ListView") > -1)))
                        {
                            if (Convert.ToBoolean(ObjectSpace.GetExpressionEvaluator(ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(null)))
                            {
                                switch (appearanceItem.Visibility)
                                {
                                    case ViewItemVisibility.ShowEmptySpace:
                                    case ViewItemVisibility.Hide:
                                        columnIsVisible = false;
                                        break;
                                    case ViewItemVisibility.Show:
                                        columnIsVisible = true;
                                        break;
                                }
                            }
                        }
                    }
                    #endregion

                    if ((column.Index == null || column.Index > -1) && columnIsVisible)
                    {
                        var align = "left";
                        if (ObjectTypeInfo != null) {
                            var member = ObjectTypeInfo.FindMember(column.GetValue<String>("FieldName"));
                            switch(Type.GetTypeCode(member.MemberType)) {
                                case TypeCode.Boolean:
                                case TypeCode.Decimal:                            
                                case TypeCode.Int16:
                                case TypeCode.Int32:
                                case TypeCode.Int64:
                                case TypeCode.UInt16:
                                case TypeCode.UInt32:
                                case TypeCode.UInt64:
                                case TypeCode.Double:
                                    align = "right";                                
                                    break;
                                default:
                                    align = "left";                                
                                    break;
                            }
                        }
                    
                        HeaderColumns.Add(new HeaderInfo() { Index = ColumnIdx, Caption = column.Caption, FieldName = column.GetValue<String>("FieldName"), ID = column.Id, Align = align, Model = column, MinWidth = column.Width });
                        ColumnIdx++;
                    }
                }

            if (ShowHeaders)
            {
                AddStringContent("<thead>");
                AddStringContent("<tr>");

                foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                {
                    String columnVal = column.Caption;
                    String columnFormat = String.Format("<th {{1}} style=\"text-align: {{2}}; {0} {1} {2}\">{{0}}</th>"
                        , column.MaxWidth > -1 ? String.Format("max-width: {0}px;", column.MaxWidth) : ""
                        , column.MinWidth > -1 ? String.Format("min-width: {0}px;", column.MinWidth) : ""
                        , column.FixedWidth > -1 ? String.Format("width: {0}px;", column.FixedWidth) : ""
                    );
                    if (OnGenerateHeader != null)
                        OnGenerateHeader(ref columnFormat, column.FieldName, ref columnVal, null);

                    AddStringContent(String.Format(columnFormat, columnVal, (column.Model != null && column.Model.ImageEditorFixedHeight > 0) ? String.Format("width={0}", column.Model.ImageEditorFixedHeight) : "", column.Align));
                }

                AddStringContent("</tr>");
                AddStringContent("</thead>");
            }
            #endregion

            #region Build Data Rows
            //Make data
            if (DataSource != null)
            {
                int RowCounter = PageIndex * PageItemsCount;
                AddStringContent("<tbody>");

                if (DataSource is IList)
                {
                    IList<Object> objects = (DataSource as IList).OfType<Object>().ToList();
                    ItemsCount = objects.Count;

                    PagesCount = (int)Math.Ceiling(((float)objects.Count / PageItemsCount));

                    if (PagesCount - 1 < PageIndex)
                        PageIndex = Math.Max(PagesCount - 1, 0);

                    foreach (var item in objects.Skip(PageIndex * PageItemsCount).Take(PageItemsCount))
                    {                        
                        StringBuilder row = new StringBuilder();
                        switch (SelectionType)
                        {
                            case SelectionObjectType.SingleObject:
                                if (OnGenerateItemClick == null)
                                    row.AppendFormat(String.Format("<tr onclick=\"{0}\">", Handler.GetScript(String.Format("'SelectedObject|{0}'", RowCounter))));
                                else
                                {
                                    String Click = "<tr onclick=\"{0}\">";
                                    OnGenerateItemClick(ref Click, RowCounter, item);
                                    row.AppendFormat(String.Format(Click, Handler.GetScript(String.Format("'SelectedObject|{0}'", RowCounter))));
                                }
                                break;                            
                            case SelectionObjectType.MultipleObject:                                
                            default:
                                if (OnGenerateItemClick == null)
                                    row.AppendFormat(String.Format("<tr onclick=\"{0}\">", Handler.GetScript(String.Format("'BrowseObject|{0}'", RowCounter))));
                                else
                                {
                                    String Click = "<tr onclick=\"{0}\">";
                                    OnGenerateItemClick(ref Click, RowCounter, item);
                                    row.AppendFormat(String.Format(Click, Handler.GetScript(String.Format("'BrowseObject|{0}'", RowCounter))));
                                }
                                break;
                        }                                                

                        foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                        {
                            String FieldName = (column.Model != null ? column.Model.PropertyName : column.FieldName);

                            IMemberInfo memberInfo;
                            var objValue = ObjectFormatValues.GetValueRecursive(FieldName, item, out memberInfo);

                            String Value = FormatValue(memberInfo, objValue, column.Model, RowCounter);

                            if (column.FieldName == "SystemColumn_RowCounter")
                                Value = String.Format("{0}", RowCounter);

                            String DataFormat = "<td style=\"text-align: {1}\">{0}</td>";

                            if (OnGenerateCell != null)
                                OnGenerateCell(ref DataFormat, column.ID, ref Value, RowCounter, item);                            

                            row.AppendFormat(String.Format(DataFormat, String.Concat(Value).Replace("{","{{").Replace("}","}}"), column.Align));                            
                        }

                        row.AppendFormat("</tr>");
                        var RowString = row.ToString();
                        Control Control = null;

                        if (OnGenerateRowFinished != null)
                            OnGenerateRowFinished(ref RowString, ref Control, RowCounter, item);

                        AddStringContent(RowString);
                        if (Control != null)
                            AddContent(Control);

                        RowCounter++;
                    }
                }

                AddStringContent("</tbody>");
            }            
            #endregion

            AddStringContent("</table>");
            AddStringContent("</div>");

            #region Build Pager
            //Pager
            if (PagesCount > 1)
            {
                #region Pages
                AddStringContent("<div class='btn-group'>");

                int ButtonsCount = PagerButtonsCount - 2; //Minus two buttons - prev and next
                int FirstHalf = (int)Math.Ceiling((decimal)ButtonsCount / 2);

                if (ID == null || String.Concat(ID).IndexOf(Helpers.JoinString) > -1)
                    if (Parent != null)
                        ID = Parent.ID = Helpers.JoinString + "table";
                    else
                        ID = Guid.NewGuid().ToString();

                AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"{0}\">&laquo;</button>", Handler.GetScript( "'PageIndex|-'")));

                int aStartIdx = Math.Min(Math.Max(0, PageIndex - FirstHalf), PagesCount - ButtonsCount - 1);
                int aFinishIdx = aStartIdx + ButtonsCount - ((aStartIdx > 0) ? 1 : 0);

                for (int i = 1; i <= PagesCount; i++)
                {
                    if ((i >= aStartIdx + 1 && i <= aFinishIdx + 1) || i == 1 || i == PagesCount)
                        AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm {2}\" onclick=\"{1}\">{0}</button>", i, Handler.GetScript(String.Format("'PageIndex|{0}'", i)), (i == (PageIndex + 1)) ? @"active" : ""));
                }
                AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"{0}\">&raquo;</button>", Handler.GetScript("'PageIndex|+'")));
                AddStringContent("</div>");

                #endregion

                #region GOTO Page
                ///Modal GOTO Page
                AddStringContent("<div class='btn-group'>");
                AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm\" data-toggle=\"modal\" data-target=\"#{0}_GotoPageindex\">{1}</button>", ClientID, Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "GotoPage")));
                AddStringContent("</div>");

                String aGoto = String.Format(@"
                <!-- Modal -->          
                <div class='modal fade' id='{0}_GotoPageindex' tabindex='-1' style='display: none;' data-focus-on='input:first'>                      
                    <div class='modal-dialog modal-lg'>
                        <div class='modal-content'>
                            <div class='modal-header'>
                                <button type='button' class='close' data-dismiss='modal'><span aria-hidden='true'>&times;</span><span class='sr-only'>Закрыть</span></button>
                                <h4 class='modal-title' id='{0}_GotoPageindexLabel'>{2}</h4>
                            </div>
                            <div class='modal-body'>
                                <div class='input-group'>
                                    <span class='input-group-addon'>#</span>
                                    <input id='{0}_GotoPageNumber' type='text' class='form-control' placeholder='{3}'>
                                </div>
                            </div>
                            <div class='modal-footer'>
                                <button type='button' class='btn btn-default btn-sm' data-dismiss='modal'>Отмена</button>
                                <button type='button' class='btn btn-primary btn-sm' data-dismiss='modal' onclick=""{1};"">Перейти</button>
                            </div>                    
                        </div>
                    </div>
                </div>                
                ", ClientID
                 , Handler.GetScript(String.Format("'PageIndex|' + $('#{0}_GotoPageNumber').val()", ClientID))
                 , Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "DialogGotoPageCaption")
                 , Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "DialogGotoPageEnterPageNumber"));

                AddStringContent(aGoto);

                #endregion
            }
            #endregion                        
            
            if (IsFloatThead && (ItemsCount > 1) && 0==1)
                script.ClientSideEvents.Init = String.Format(@"function(s,e) {{
                    var offset = $('.navbar-fixed-top').height();                
                    $('#{0}_Table').floatThead({{ scrollingTop: offset }});
                }}", ClientID);

            #region Scripts

            Scripts = new ASPxLabel();
            Scripts.ClientSideEvents.Init = ClientSideEvents.Init;
            Scripts.ClientSideEvents.Click = ClientSideEvents.Click;

            AddContent(Scripts);

            #endregion
        }

        void Handler_OnCallback(object source, CallbackEventArgs e)
        {            
            String[] values = String.Concat(e.Parameter).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "PageIndex":
                        #region PageIndex
                        switch (values[1])
                        {
                            case "+":
                                PageIndex++;
                                PageIndex = Math.Min(PagesCount - 1, PageIndex);
                                break;
                            case "-":
                                PageIndex--;
                                PageIndex = Math.Max(0, PageIndex);
                                break;
                            default:
                                int newVal;
                                if (int.TryParse(values[1], out newVal))                                
                                    PageIndex = newVal - 1;                                
                                break;
                        }                        
                        break;
                        #endregion
                    case "SelectedObject":
                        #region SelectedObject
                        if (DataSource is IList)
                        {
                            int objectNumber = -1;
                            if (int.TryParse(values[1], out objectNumber)) {
                                IList<Object> objects = (DataSource as IList).OfType<Object>().ToList();
                                var obj = objects.Skip(objectNumber-1).FirstOrDefault();
                                if (SelectionType == SelectionObjectType.SingleObject)
                                    SelectedObjects.Clear();
                                SelectedObjects.Add(obj);
                                if (SelectedObjectChanged != null)
                                    SelectedObjectChanged(this, EventArgs.Empty);

                                if (SelectedObjects.Count > 0 && SelectionType == SelectionObjectType.SingleObject)
                                {
                                    
                                }
                            }
                        }
                        break;
                        #endregion
                    case "BrowseObject":
                        #region BrowseObject
                        int browseNum = -1;
                        if (int.TryParse(values[1], out browseNum))
                        {
                            IList<Object> objects = (DataSource as IList).OfType<Object>().ToList();
                            var obj = objects.Skip(browseNum - 1).FirstOrDefault();
                            
                            if (values.Count() > 2 && obj != null) {
                                IMemberInfo memberInfo;
                                obj = ObjectFormatValues.GetValueRecursive(values[2], obj, out memberInfo);
                            }

                            if (obj != null)
                            {
                                var App = (WebApplication.Instance as XafApplication);
                                var os = ObjectSpace.CreateNestedObjectSpace();
                                obj = os.GetObject(obj);
                                DetailView view = App.CreateDetailView(os, App.FindDetailViewId(obj.GetType()), true, obj);
                                view.ViewEditMode = EditMode;
                                var svp = new ShowViewParameters(view);
                                svp.TargetWindow = TargetWindow.NewModalWindow;
                                App.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                            }
                        }
                        break;
                        #endregion
                }
                InnerSaveControlState();
                InnerRender();                
            }
        }
    }
}
