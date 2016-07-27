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

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Xpo;
using XAF_Bootstrap.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Drawing;

namespace XAF_Bootstrap.Controls
{
    public enum SelectionObjectType
    {        
        SingleObject,
        MultipleObject,
        None,
        Browsing
    }

    public enum XafBootstrapTablePosition
    {
        Top,
        Bottom,
        Both
    }

    public delegate void OnGenerateRowEventHandler(ref String Result, int RowNumber, object Row);
    public delegate void OnGenerateRowFinishedHandler(ref String Result, ref Control ResultControl, int RowNumber, object Row);
    public delegate void OnGenerateCellHandler(ref String Format, String FieldName, ref String value, int RowNumber, object data);
    public delegate void OnGenerateHeaderHandler(ref String Format, String FieldName, ref String value, object data);    

    public class XafBootstrapTable : XafBootstrapBaseControl
    {
        private Control Content;

        public XafBootstrapTable()
        {
            SelectionType = SelectionObjectType.None;
            SelectedObjects = new List<object>();
            CustomColumns = new List<HeaderInfo>();

            Content = new Control();
            Controls.Add(Content);

            ClientSideEvents = new StaticEditClientSideEvents();

            PageButtonsPosition = XafBootstrapTablePosition.Bottom;
        }

        public event EventHandler OnBrowse;

        public event OnGenerateRowEventHandler OnGenerateItemClick;
        public event OnGenerateRowFinishedHandler OnGenerateRowFinished;
        public event OnGenerateCellHandler OnGenerateCell;
        public event OnGenerateHeaderHandler OnGenerateHeader;
        public event EventHandler BeforeGenerateHeaders;
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
        public XafBootstrapTablePosition PageButtonsPosition;
        private object _DataSource;
        public object DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                _DataSource = value;
            }
        }

        public IList<HeaderInfo> HeaderColumns;
        public IList<HeaderInfo> CustomColumns;

        public int PageItemsCount = 20;
        public int PagesCount = 0;
        public Boolean IsFloatThead = true;
        public Boolean ProcessClick = true;

        private bool _ShowNumbers = true;
        [SaveState]
        public bool ShowNumbers
        {
            get { return _ShowNumbers; }
            set { _ShowNumbers = value; }
        }        
        
        private int _PageIndex;
        [SaveState]
        public int PageIndex
        {
            get {
                int.TryParse(String.Concat(Helpers.Session["XafBootstrapTable_" + ID + "_PageIndex"]), out _PageIndex);
                return _PageIndex;
            }
            set {
                _PageIndex = value;
                Helpers.Session["XafBootstrapTable_" + ID + "_PageIndex"] = value;
            }
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

        public class TableSettings
        {
            public int PageItemsCount = 20;
            public int PagesCount = 0;
            public int PageIndex = 0;
            public int PagerButtonsCount = 7;
            public IList<object> SelectedObjects;
        }

        private Boolean IsRendering;
        public void Refresh()
        {
            if (!IsRendering)
                InnerRender();
        }

        public void AddStringContent(String content)
        {
            HTMLText control = new HTMLText();
            control.Text = content;
            Content.Controls.Add(control);
        }

        public void AddStringContent(String content, Control[] controls)
        {
            foreach (var dest in controls)
            {
                HTMLText control = new HTMLText();
                control.Text = content;
                dest.Controls.Add(control);
            }
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
            public String Filter = "";
            public SortDirection Sorting = SortDirection.None;
            public double SortIndex = -1;
            public String CSSClassNames;
        }

        private void ProcessBitmap(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber, ref String Value)
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

        private void ProcessEnum(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber, ref String Value)
        {
            Value = Helpers.GetXafDisplayName((Enum)objValue);
        }

        private void ProcessBoolean(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber, ref String Value)
        {
            if ((Boolean)objValue)
                Value = "<span class=\"glyphicon glyphicon-ok\"></span>";
        }

        private void ProcessColor(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber, ref String Value)
        {
            var color = (Color)objValue;
            if (color != Color.Empty)
            {
                Value = String.Format("<div style='width: 20px; height: 20px; border: 1px solid; border-color: rgba(0,0,0,0.2); background: rgba({0},{1},{2},{3});' class='img-circle'><span style='padding-left: 24px'>{4}</span></div>", color.R, color.G, color.B, color.A / 255, ColorTranslator.ToHtml(color));
            }
        }

        private void ProcessDefault(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber, ref String Value)
        {
            String displayFormat = "{0}";
            if (columnModel != null)
            {
                var valuePath = columnModel.GetValue<String>("FieldName").Split('.');
                if (!(objValue is XPBaseObject) && !(objValue is String) && !(objValue is DateTime) && !objValue.GetType().IsPrimitive)
                    if (valuePath.Length > 1)
                    {
                        IMemberInfo mInfo;
                        var val = ObjectFormatValues.GetValueRecursive(String.Join(".", valuePath.Skip(1).Take(valuePath.Length - 1)), objValue, out mInfo);
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
                        else
                        {
                            var defPropAttr = memberInfo.MemberTypeInfo.FindAttribute<XafDefaultPropertyAttribute>();
                            if (defPropAttr != null)
                            {
                                displayFormat = "{0:" + defPropAttr.Name + "}";
                                Value = String.Format(new ObjectFormatter(), displayFormat, objValue);
                            }
                        }
                    };
                if (Value == "" && valuePath.Length > 1)
                    displayFormat = "{0:" + String.Join(".", valuePath.Skip(1).Take(valuePath.Length - 1)) + "}";
            }

            if (Value == "")
            {
                if (displayFormat != "{0}" && (memberInfo.MemberTypeInfo != null && memberInfo.MemberTypeInfo.IsDomainComponent))                
                    Value = String.Format(new ObjectFormatter(), displayFormat, objValue);
                else
                    Value = String.Format(displayFormat, objValue);
            }

            if (memberInfo.MemberTypeInfo != null && memberInfo.MemberTypeInfo.IsDomainComponent && columnModel != null && ProcessClick)
                Value = String.Format(@"<a href=""javascript:;"" onclick=""event = event || window.event; event.stopPropagation(); {0}"">{1}</a>", Handler.GetScript(String.Format("'BrowseObject|{0}|{1}'", RowNumber, columnModel.PropertyName)), Value);                    
        }

        public String FormatValue(IMemberInfo memberInfo, object objValue, IModelColumn columnModel, int RowNumber)
        {
            String Value = "";
            if (memberInfo != null && objValue != null)
            {
                if (objValue is System.Drawing.Bitmap)                
                    ProcessBitmap(memberInfo, objValue, columnModel, RowNumber, ref Value);                    
                else                
                if (objValue is Enum)
                    ProcessEnum(memberInfo, objValue, columnModel, RowNumber, ref Value);      
                else
                if (objValue is Boolean)
                    ProcessBoolean(memberInfo, objValue, columnModel, RowNumber, ref Value);
                else
                if (objValue is Color)
                    ProcessColor(memberInfo, objValue, columnModel, RowNumber, ref Value);
                else
                    ProcessDefault(memberInfo, objValue, columnModel, RowNumber, ref Value);
            }
            return Value;
        }        

        public Boolean TableHover = true;
        public Boolean TableStriped = true;
        public Boolean ShowHeaders = true;
        public Boolean ShowFilter = true;

        internal class SummaryValue
        {
            public IModelColumn Column;            
            private decimal Max;
            private decimal Min;
            private decimal Sum;
            private decimal Count;
            public decimal Summary
            {
                get
                {                    
                    switch (Column.Summary[0].SummaryType)
                    {
                        case SummaryType.Average:
                            return Sum / Count;                            
                        case SummaryType.Count:
                            return Count;
                        case SummaryType.Max:
                            return Max;
                        case SummaryType.Min:
                            return Min;
                        case SummaryType.Custom:
                        case SummaryType.None:
                        case SummaryType.Sum:
                            return Sum;
                    }
                    return 0;
                }
            }

            public void AddValue(decimal Value) {
                Count++;
                Max = Math.Max(Max, Value);
                Min = Math.Min(Min, Value);
                Sum += Value;
            }
        }

        public String TableDivClass = "panel panel-default";
        public String TableClass = "xaf-bootstrap";

        public void InnerRender(Boolean Force = false)
        {
            try {
                IsRendering = true;
                if (!Initialized && !Force)
                    return;

                if (Handler == null)
                {
                    Handler = new CallbackHandler(ClientID + "_handler");
                    Handler.OnCallback += Handler_OnCallback;
                }

                Content.Controls.Clear();
                InnerLoadControlState();

                var TopRow = new Control();
                Content.Controls.Add(TopRow);

                #region Read headers info

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
                        if (ObjectTypeInfo != null && ObjectSpace != null)
                        {
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
                            if (ObjectTypeInfo != null)
                            {
                                var member = ObjectTypeInfo.FindMember(column.GetValue<String>("FieldName"));
                                switch (Type.GetTypeCode(member.MemberType))
                                {
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

                #region read form values

                foreach (var column in HeaderColumns.Where(f => f.Model != null)) {
                    var sorting = String.Concat(Request.Form[ClientID + "_sort_" + column.FieldName]);
                    var sortingOptions = sorting.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (sortingOptions.Length > 0)
                    {
                        column.Sorting = (SortDirection)(sortingOptions[0] == "1" ? 1 : sortingOptions[0] == "2" ? 2 : 0);
                        if (sortingOptions.Length > 1)
                            column.SortIndex = double.Parse(sortingOptions[1]);
                    }
                    column.Filter = Request.Form[ClientID + "_filter_" + column.FieldName];
                }

                #endregion

                #endregion

                AddStringContent("<div>");

                AddStringContent(String.Format("<div class='container'></div><div class='{0}' style='overflow: auto;'>", TableDivClass));

                #region FilterButton

                if (ShowHeaders)
                {
                    if (ShowFilter)
                    {
                        AddStringContent(String.Format(@"<div class=""table-filter-button {0}"" onclick=""if ($(this).hasClass('btn-default')) {{ $(this).removeClass('btn-default').addClass('btn-primary'); }} else {{ $(this).addClass('btn-default').removeClass('btn-primary'); }}; $(this).parent().find('.filter-row').first().toggle();""><span class=""glyphicon glyphicon-filter""></span></div>"
                            , (HeaderColumns.Count(f => String.Concat(f.Filter) != "") > 0 ? "btn-primary" : "btn-default")));
                    }
                }

                #endregion

                AddStringContent(String.Format(@"<table class='table {1} {2} {3}' id = '{0}_Table'>"
                    , ClientID
                    , TableHover ? "table-hover" : ""
                    , TableStriped ? "table-striped" : ""
                    , TableClass));

                StringBuilder TableScripts = new StringBuilder();

                int ItemsCount = 0;
                IMemberInfo memberInfo;

                #region Build Header Row, Filters                        

                if (ShowHeaders)
                {
                    if (BeforeGenerateHeaders != null)
                        BeforeGenerateHeaders(this, EventArgs.Empty);

                    AddStringContent("<thead>");
                    AddStringContent("<tr class='panel'>");

                    foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                    {
                        String columnVal = (String.Concat(column.Caption) == "" ? column.FieldName : column.Caption);
                        String columnFormat = String.Format(@"<th {{1}} {{4}} style=""text-align: {{2}}; vertical-align: middle; {0} {1} {2}"" class=""{6}"">{{0}} {{3}}<input type=""hidden"" name=""{3}_sort_{4}"" value=""{5}""></th>"
                            , column.MaxWidth > -1 ? String.Format("max-width: {0}px;", column.MaxWidth) : ""
                            , column.MinWidth > -1 ? String.Format("min-width: {0}px;", column.MinWidth) : ""
                            , column.FixedWidth > -1 ? String.Format("width: {0}px;", column.FixedWidth) : ""
                            , ClientID
                            , column.FieldName
                            , (int)column.Sorting
                            , column.CSSClassNames
                        );
                        if (OnGenerateHeader != null)
                            OnGenerateHeader(ref columnFormat, column.FieldName, ref columnVal, null);

                        String sort = "";
                        String sortClick = "";
                        if (column.Model != null)
                        {
                            switch (column.Sorting)
                            {
                                case SortDirection.Ascending:
                                    sort = @"<span class=""glyphicon glyphicon-chevron-up""></span>";
                                    break;
                                case SortDirection.Descending:
                                    sort = @"<span class=""glyphicon glyphicon-chevron-down""></span>";
                                    break;
                            }

                            sortClick = String.Format(@"onclick=""{1}_DoSort(this)""", column.FieldName, ClientID);
                        }

                        AddStringContent(
                            String.Format(columnFormat
                                , columnVal
                                , (column.Model != null && column.Model.ImageEditorFixedHeight > 0) ? String.Format("width={0}", column.Model.ImageEditorFixedHeight) : ""
                                , column.Align
                                , sort
                                , sortClick));
                    }

                    AddStringContent("</tr>");

                    AddStringContent("</thead>");
                }
                #endregion            

                #region Build Data Rows
                IList<Object> objects = null;
                //Make data
                if (DataSource != null)
                {
                    int RowCounter = PageIndex * PageItemsCount;
                    AddStringContent("<tbody>");

                    if (ShowFilter)
                    {
                        AddStringContent(String.Format(@"<tr class=""filter-row"" style=""{0}"">", HeaderColumns.Count(f => String.Concat(f.Filter) != "") == 0 ? "display: none" : ""));

                        foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                        {
                            AddStringContent(@"<td class=""filter-header"">");
                            if (column.Model != null)
                            {
                                AddStringContent(String.Format(@"<input name=""{0}_filter_{1}"" type=""text"" class=""form-control input-sm"" onkeypress=""{0}_DoFilter(this, event)"" onchange=""{0}_DoFilter(this)"" value=""{2}"">", ClientID, column.FieldName, column.Filter));
                            }
                            AddStringContent("</td>");
                        }

                        AddStringContent("</tr>");
                    }

                    if (DataSource is IList)
                    {
                        objects = (DataSource as IList).OfType<Object>().ToList();

                        //Apply filter
                        foreach (var column in HeaderColumns.Where(f => String.Concat(f.Filter) != ""))
                            objects = objects.Where(f => IsObjectFiltered(column, column.Filter, f, RowCounter)).ToList();

                        //Apply sorting
                        int sortCounter = 0;
                        var sortings = HeaderColumns.Where(f => f.Sorting != SortDirection.None);
                        IOrderedEnumerable<object> orderedList = null;
                        foreach (var column in sortings.OrderBy(f => f.SortIndex))
                        {
                            switch (column.Sorting)
                            {
                                case SortDirection.Ascending:
                                    if (sortCounter == 0)
                                        orderedList = objects.OrderBy(f => ObjectFormatValues.GetValueRecursive(column.FieldName, f, out memberInfo));
                                    else
                                        orderedList = orderedList.ThenBy(f => ObjectFormatValues.GetValueRecursive(column.FieldName, f, out memberInfo));
                                    break;
                                case SortDirection.Descending:
                                    if (sortCounter == 0)
                                        orderedList = objects.OrderByDescending(f => ObjectFormatValues.GetValueRecursive(column.FieldName, f, out memberInfo));
                                    else
                                        orderedList = orderedList.ThenByDescending(f => ObjectFormatValues.GetValueRecursive(column.FieldName, f, out memberInfo));
                                    break;
                            }
                            sortCounter++;
                        }
                        if (orderedList != null)
                        {
                            objects = orderedList.ToList();
                            DataSource = objects;
                        }

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
                                        row.AppendFormat(String.Format("<tr onclick=\"{0}\">", (ProcessClick ? Handler.GetScript(String.Format("'BrowseObject|{0}'", RowCounter)) : "")));
                                    else
                                    {
                                        String Click = "<tr onclick=\"{0}\">";
                                        OnGenerateItemClick(ref Click, RowCounter, item);
                                        row.AppendFormat(String.Format(Click, (ProcessClick ? Handler.GetScript(String.Format("'BrowseObject|{0}'", RowCounter)) : "")));
                                    }
                                    break;
                            }

                            foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                            {
                                String FieldName = (column.Model != null ? column.Model.PropertyName : column.FieldName);

                                var objValue = ObjectFormatValues.GetValueRecursive(FieldName, item, out memberInfo);

                                String Value = FormatValue(memberInfo, objValue, column.Model, RowCounter);
                                if (item is System.Data.DataRowView)
                                {
                                    memberInfo = null;
                                    var arow = (item as System.Data.DataRowView);
                                    if (arow.DataView.Table.Columns.Contains(FieldName))
                                        Value = String.Concat((item as System.Data.DataRowView)[FieldName]);
                                }

                                if (column.FieldName == "SystemColumn_RowCounter")
                                    Value = String.Format("{0}", RowCounter + 1);

                                String DataFormat = "<td style=\"text-align: {1}\">{0}</td>";

                                if (OnGenerateCell != null)
                                    OnGenerateCell(ref DataFormat, column.FieldName, ref Value, RowCounter, item);

                                row.AppendFormat(DataFormat, String.Concat(Value), column.Align);
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

                #region Build Footers
                var SummaryColumns = HeaderColumns.Where(f => f.Model != null && f.Model.Summary != null && f.Model.Summary.Count > 0).Select(f => f.Model).ToList();
                var HasFooters = SummaryColumns.Count > 0;
                IList<SummaryValue> SummaryValues = new List<SummaryValue>();
                foreach (var col in SummaryColumns)
                    SummaryValues.Add(new SummaryValue() { Column = col });

                AddStringContent("<tfoot>");
                int colSpan = 0;
                if (HasFooters && objects != null)
                {
                    foreach (var item in objects)
                    {
                        foreach (var summary in SummaryValues)
                        {
                            decimal summaryValue = 0;
                            if (decimal.TryParse(String.Concat(ObjectFormatValues.GetValueRecursive(summary.Column.FieldName, item, out memberInfo)), out summaryValue))
                            {
                                summary.AddValue(summaryValue);
                            }
                        }
                    }

                    AddStringContent("<tr>");
                    foreach (var column in HeaderColumns.OrderBy(f => f.Index))
                    {

                        var summary = SummaryValues.Where(f => f.Column == column.Model).FirstOrDefault();
                        if (summary != null)
                        {
                            if (colSpan > 0)
                                AddStringContent(String.Format("<td colspan=\"{0}\"></td>", colSpan));

                            AddStringContent(String.Format("<td class=\"text-right\">{0:n}</td>", summary.Summary));
                            colSpan = 0;
                        }
                        else
                        {
                            colSpan++;
                        }
                    }
                    if (colSpan > 0)
                        AddStringContent(String.Format("<td colspan=\"{0}\"></td>", colSpan));
                    AddStringContent("</tr>");
                }
                AddStringContent("</tfoot>");
                #endregion

                AddStringContent("</table>");
                AddStringContent("</div>");

                AddStringContent("</div>");

                #region Build Pager
                //Pager
                if (PagesCount > 1)
                {
                    var controls = new List<Control>();
                    if (PageButtonsPosition == XafBootstrapTablePosition.Top || PageButtonsPosition == XafBootstrapTablePosition.Both)
                        controls.Add(TopRow);

                    if (PageButtonsPosition == XafBootstrapTablePosition.Bottom || PageButtonsPosition == XafBootstrapTablePosition.Both)
                        controls.Add(Content);

                    #region PageItemsCount
                    ///PageItemsCount button
                    AddStringContent("<div class='btn-group pull-right'>", controls.ToArray());
                    AddStringContent(String.Format("<select style=\"margin-left: 2px; width: 60px;\" class=\"input input-sm form-control\" onchange=\"{2}\">{1}</select>"
                        , ClientID
                        , String.Join("", (new String[] { "10", "20", "50", "100", Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "AllItems") }).ToList().Select(f => String.Format("<option value=\"{0}\" {1}>{0}</option>", f, String.Concat(PageItemsCount) == f ? "selected" : "")))
                        , Handler.GetScript("'PageItemsCount|' + $(this).val()")), controls.ToArray());
                    AddStringContent("</div>", controls.ToArray());

                    #endregion

                    #region Pages
                    AddStringContent("<div class='btn-group pull-right'>", controls.ToArray());

                    int ButtonsCount = PagerButtonsCount - 2; //Minus two buttons - prev and next
                    int FirstHalf = (int)Math.Ceiling((decimal)ButtonsCount / 2);

                    if (ID == null || String.Concat(ID).IndexOf(Helpers.JoinString) > -1)
                        if (Parent != null)
                            ID = Parent.ID = Helpers.JoinString + "table";
                        else
                            ID = Guid.NewGuid().ToString();

                    AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"{0}\">&laquo;</button>", Handler.GetScript("'PageIndex|-'")), controls.ToArray());

                    int aStartIdx = Math.Min(Math.Max(0, PageIndex - FirstHalf), PagesCount - ButtonsCount - 1);
                    int aFinishIdx = aStartIdx + ButtonsCount - ((aStartIdx > 0) ? 1 : 0);

                    for (int i = 1; i <= PagesCount; i++)
                    {
                        if ((i >= aStartIdx + 1 && i <= aFinishIdx + 1) || i == 1 || i == PagesCount)
                            AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm {2}\" onclick=\"{1}\">{0}</button>", i, Handler.GetScript(String.Format("'PageIndex|{0}'", i)), (i == (PageIndex + 1)) ? @"active" : ""), controls.ToArray());
                    }
                    AddStringContent(String.Format("<button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"{0}\">&raquo;</button>", Handler.GetScript("'PageIndex|+'")), controls.ToArray());
                    AddStringContent("</div>", controls.ToArray());

                    #endregion

                    #region GOTO Page
                    ///GOTO Page button
                    AddStringContent("<div class='btn-group pull-right'>", controls.ToArray());
                    AddStringContent(String.Format("<button type=\"button\" style=\"margin-right: 2px\" class=\"btn btn-default btn-sm\" data-toggle=\"modal\" data-target=\"#{0}_GotoPageindex\">{1}</button>", ClientID, Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "GotoPage")), controls.ToArray());
                    AddStringContent("</div>", controls.ToArray());

                    ///Modal GOTO Page
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
                                    <button type='button' class='btn btn-default btn-sm' data-dismiss='modal'>{5}</button>
                                    <button type='button' class='btn btn-primary btn-sm' data-dismiss='modal' onclick=""{1};""><span class=""glyphicon glyphicon-share-alt""></span> {4}</button>
                                </div>                    
                            </div>
                        </div>
                    </div>                
                    ", ClientID
                     , Handler.GetScript(String.Format("'PageIndex|' + $('#{0}_GotoPageNumber').val()", ClientID))
                     , Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "DialogGotoPageCaption")
                     , Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "DialogGotoPageEnterPageNumber")
                     , Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "GotoPage")
                     , Helpers.GetLocalizedText(@"DialogButtons", "Cancel"));

                    AddStringContent(aGoto);

                    #endregion
                }
                #endregion
                var form = System.Web.HttpContext.Current.Request.Form;
                if (IsFloatThead && (ItemsCount > 1))
                {
                    AddStringContent(String.Format(@"<input type='hidden' name='{0}_scrollTop' id='{0}_scrollTop' value=''><input type='hidden' name='{0}_scrollLeft' id='{0}_scrollLeft' value=''>", ClientID));
                    TableScripts.AppendFormat(@"
                        $('#{0}_Table').parent().scroll(function() {{ $('#{0}_scrollTop').val($('#{0}_Table').parent().scrollTop()); $('#{0}_scrollLeft').val($('#{0}_Table').parent().scrollLeft()); }});                        
                        $('#{0}_Table').floatThead('destroy');
                        $('#{0}_Table').floatThead({{
                            scrollContainer: function($table) {{
                                return $('#{0}_Table').parent();
                            }},
                            zIndex: 100
                        }});
                    
                        var offset = $('.navbar-fixed-top').height();  
                        var footerHeight = $('.FooterCopyright').closest('.container').height();
                        footerHeight = Math.max(footerHeight, $('.FooterCopyright').height());
                    
                        var maxHeight = Math.max(400, $(window).height() - footerHeight - offset - 100) + 'px';
                        $('#{0}_Table').parent().css({{ 'max-height': maxHeight }});

                        $(window).trigger('resize');

                        {1} {2}
                    "
                        , ClientID
                        , String.Concat(form[ClientID + "_scrollTop"]) != "" ? String.Format("$('#{0}_Table').parent().scrollTop('{1}');", ClientID, form[ClientID + "_scrollTop"], form[ClientID + "_scrollLeft"]) : ""
                        , String.Concat(form[ClientID + "_scrollLeft"]) != "" ? String.Format("$('#{0}_Table').parent().scrollLeft('{1}');", ClientID, form[ClientID + "_scrollLeft"]) : ""
                    );
                }



                TableScripts.AppendFormat(@"
                    window['{0}_DoFilter'] = function(filterInput, keyPressEvent) {{
                        var refresh = ((!keyPressEvent) || keyPressEvent.which == 13 || keyPressEvent.keyCode == 13);                    
                        if (refresh == true) {{
                            {1};
                        }};                
                    }};
                    window['{0}_DoSort'] = function(sortProp) {{
                        var values = $(sortProp).find('input').val().split(',');
                        var val = parseInt(values[0], 0) + 1;
                        var timestamp = Date.now();                    
                        if (val > 2)
                            val = 0;
                        else if (values.length > 1)
                            timestamp = values[1];                    
                    
                        $(sortProp).find('input').val(val + ',' + timestamp);
                        {2};
                    }};
                
                ", ClientID, Handler.GetScript("'FiltersUpdated|'"), Handler.GetScript("'DoSort|' + sortProp"));

                #region Scripts

                if (TableScripts.Length > 0)
                    WebWindow.CurrentRequestWindow.RegisterStartupScript(ClientID + "_scripts", TableScripts.ToString(), true);

                Scripts = new ASPxLabel();
                Scripts.ClientSideEvents.Init = ClientSideEvents.Init;
                Scripts.ClientSideEvents.Click = ClientSideEvents.Click;

                AddContent(Scripts);

                #endregion
            }
            finally
            {
                IsRendering = false;
            }
        }

        private Boolean IsObjectFiltered(HeaderInfo column, String filter, object obj, int RowNumber)
        {
            IMemberInfo info;
            String FieldName = (column.Model != null ? column.Model.PropertyName : column.FieldName);
            var objValue = ObjectFormatValues.GetValueRecursive(FieldName, obj, out info);
            var Value = String.Concat(FormatValue(info, objValue, column.Model, RowNumber));
            return Value.ToLower().IndexOf(filter.ToLower()) > -1;
        }

        void Handler_OnCallback(object source, CallbackEventArgs e)
        {            
            String[] values = String.Concat(e.Parameter).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "PageItemsCount":
                        if (values[1] == Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapTable", "AllItems"))
                            PageItemsCount = 10000;
                        else
                        {
                            int count = PageItemsCount;
                            if (int.TryParse(values[1], out count))
                                PageItemsCount = count;
                        }
                        break;
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
                                var obj = objects.Skip(objectNumber).FirstOrDefault();
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
                        InnerRender();
                        if (int.TryParse(values[1], out browseNum))
                        {   
                            IList<Object> objects = (DataSource as IList).OfType<Object>().ToList();
                            var obj = objects.Skip(browseNum).FirstOrDefault();
                            
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
                    case "FiltersUpdated":
                        #region FiltersUpdated

                        #endregion
                        break;
                    case "DoSort":
                        #region FiltersUpdated

                        #endregion
                        break;
                }
                InnerSaveControlState();
                InnerRender();                
            }
        }
    }
}
