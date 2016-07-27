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

using DevExpress.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapTagSelectorItem
    {
        public XafBootstrapTagSelectorItem()
        {
            
        }
        public XafBootstrapTagSelectorItem(XafBootstrapTagSelectorItems items)
        {
            this.Items = items;
        }
        
        [Browsable(false)]
        public XafBootstrapTagSelectorItems Items;

        public String Text;
        public String Hint;
        public String Value;
        public String ImageUrl;
        public int Index
        {
            get
            {
                if (Items != null)
                    return Items.List.IndexOf(this);                
                return -1;
            }
        }

        [Browsable(false)]
        public Boolean IsMissed;
    }

    public class XafBootstrapTagSelectorItems : IEnumerable {

        public int ItemImageWidth;
        public int ItemImageHeight;

        public XafBootstrapTagSelectorItems() {
            List = new List<XafBootstrapTagSelectorItem>();
        }
        public IList<XafBootstrapTagSelectorItem> List;

        public XafBootstrapTagSelectorItem Add() {
            var item = new XafBootstrapTagSelectorItem(this);
            List.Add(item);
            return item;
        }

        public void Add(XafBootstrapTagSelectorItem Item)
        {
            if (List.IndexOf(Item) == -1)
            {
                Item.Items = this;
                List.Add(Item);
            }
        }
        
        public int Count {
            get {
                return List.Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)List.GetEnumerator();
        }
    }

    public class XafBootstrapTagSelector : System.Web.UI.WebControls.WebControl
    {
        public String ID = "";
        public string OnClickScript;
        public String PropertyName;
        private Boolean IsInitialized;
        private String _Value;

        public String Delimeter = ",";
        public String DisplayDelimeter = ", ";
        public String Value
        {
            get
            {
                if (ValueRendered && String.Concat(XAF_Bootstrap.Templates.Helpers.RequestManager.Request.Form[ClientID + "_value"]) != "")
                    _Value = HttpUtility.UrlDecode(String.Concat(XAF_Bootstrap.Templates.Helpers.RequestManager.Request.Form[ClientID + "_value"]));

                return _Value;
            }
            set
            {
                ValueRendered = false;
                _Value = value;
                foreach(var item in values)
                    if (Items.List.Where(f => f.Value == item).Count() == 0)
                    {
                        var newItem = Items.Add();
                        newItem.Text = newItem.Value = item;
                        newItem.IsMissed = true;
                    }

                if (IsInitialized)
                    InnerRender();
            }
        }

        private IList<String> values
        {
            get
            {
                return String.Concat(Value).Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public String DisplayValue
        {
            get
            {
                var _DisplayValues = Items.List.Where(f => values.IndexOf(f.Value) > -1);
                if (_DisplayValues.Count() > 0)
                    return String.Join(DisplayDelimeter, _DisplayValues.Select(f => f.Text));
                return "";
            }
        }

        public Boolean TextOnly = false;        
        public XafBootstrapTagSelectorItems Items;
        public String EmptyText;        

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        public XafBootstrapTagSelectorItem SelectedItem;

        public CallbackEventHandler OnCallback;
        public Boolean AllowAddCustomValues;
        public Boolean AllowSelectValues;

        public XafBootstrapTagSelector()
        {
            Items = new XafBootstrapTagSelectorItems();
            Content = new HTMLText();
            AllowAddCustomValues = true;
            AllowSelectValues = true;
            ValueRendered = true;

            EmptyText = XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapDropdownEdit", "EmptyText");
        }

        public Boolean TextEqualValues()
        {
            return Items.List.Where(f => f.Text != f.Value).Count() == 0;
        }

        private Boolean ValueRendered;
        private CallbackHandler Handler;
        public String InnerRender()
        {
            Content.Text = "";

            var values = String.Concat(Value).Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var selectedItems = new List<XafBootstrapTagSelectorItem>();
            foreach(var value in values) {
                var item = Items.List.FirstOrDefault(f => f.Value == value);
                if (item != null && selectedItems.IndexOf(item) == -1)
                    selectedItems.Add(item);
            }   
            
            if (TextOnly)
                Content.Text += String.Format(@"<span>{0}</span>", String.Join(DisplayDelimeter, selectedItems.Select(f => String.Format(@"{0}", f.Text))));
            else
            {
                Content.Text += String.Format(@"<input type='hidden' value='{0}' name='{1}'/>", HttpUtility.UrlEncode(String.Concat(Value)), ClientID + "_value");
                Content.Text += String.Format(@"
                    <div class=""btn-group"">
                        <span class=""form-control"" style=""height: auto; padding: 0px; {5}"">{0}" + (AllowSelectValues && selectedItems.Count < Items.Count ? @" <span class=""btn btn-xs btn-info"" style=""margin: 1px"" onclick=""var elems = $(this).parents('.btn-group').find('.modal.selector').modal();""> ... </span>" : "") + @" {2}</span>"
                        + (AllowAddCustomValues ? @"
                        <div class=""modal editor fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
                            <div class=""modal-dialog"">
                                <div class=""modal-content"">                        
                                    <div class=""modal-body"">
                                        <div class=""input-group input-group-sm"">
                                          <input type=""text"" class=""form-control"" placeholder="""">
                                          <span class=""input-group-btn"">
                                            <button class=""btn btn-default"" type=""button"" onclick=""$(this).parents('.btn-group').find('.modal.editor').modal('hide'); {4}"">{3}</button>
                                          </span>
                                        </div>
                                    </div>        
                                </div>                            
                            </div>
                        </div>" : "") + (AllowSelectValues ? @"
                        <div class=""modal selector fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
                            <div class=""modal-dialog"">
                                <div class=""modal-content"">
                                    <div class=""modal-header"">
                                        <span style='font-size: 18px;'>{1}</span>                                        
                                        <input type=""text"" class=""form-control input-sm pull-right"" placeholder=""{6}"" onkeyup=""var rows = $(this).parents('.modal-content').find('table tr'); var val = $.trim($(this).val()).replace(/ +/g, ' ').toLowerCase(); rows.show().filter(function() {{ var text = $(this).text().replace(/\s+/g, ' ').toLowerCase(); return !~text.indexOf(val); }}).hide();"" value="""" style=""min-width:75px; max-width: 300px;"">
                                    </div>
                                    <div class=""modal-body"">  
                                        <div class=""table-responsive"" style=""max-height: 480px; overflow: auto"">
                                            <table class=""table table-hover"">
                " : ""), Value == null ? EmptyText : String.Join("", selectedItems.Select(f => String.Format(@"<span class=""btn btn-xs btn-default"" style=""margin: 1px"" onclick=""{1}"">{0}</span>", f.Text, Handler.GetScript("'Remove=" + Items.List.IndexOf(f) + "'"))))
                 , EmptyText
                 , TextEqualValues() && AllowAddCustomValues ? @"<span class=""btn btn-xs btn-info"" style=""margin: 1px"" onclick=""var elems = $(this).parents('.btn-group').find('.modal.editor').modal();""> + </span>" : ""
                 , XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "Add")
                 , Handler.GetScript("'AddValue=' + $(this).parents('.input-group').find('input').val()")
                 , Value == null ? "padding-left: 6px;" : ""
                 , XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "FastSearchText")
                 );

                if (AllowSelectValues)
                {
                    foreach (XafBootstrapTagSelectorItem item in Items.List.Where(f => selectedItems.IndexOf(f) == -1).OrderBy(f => f.Text))
                    {
                        String changeEvent = @"onclick=""if ($(this).hasClass(&quot;bg-success&quot;)) {{ $(this).removeClass(&quot;bg-success&quot;).removeClass(&quot;text-primary&quot;); }} else {{ $(this).addClass(&quot;bg-success&quot;).addClass(&quot;text-primary&quot;); }} """;
                        Content.Text += String.Format(@"
                                        <tr><td {1} idx=""{3}"" style='vertical-align: middle'>{2}{0}</td></tr>"
                            , item.Text
                            , changeEvent
                            , String.Concat(item.ImageUrl) != "" ? String.Format("<img class='img-circle' style='max-width: {0}px; max-height: {1}px;' src='{2}'/> "
                                , Items.ItemImageWidth
                                , Items.ItemImageHeight
                                , item.ImageUrl) : ""
                            , Items.List.IndexOf(item));
                    }

                    Content.Text += String.Format(@"
                                                </table>
                                            </div>                                     
                                        </div>
                                        <div class=""modal-footer"">                                        
                                            <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"" onclick=""var values = ''; var items = $(this).parents('.modal').find('.bg-success'); for(i = 0; i < items.length; i++) values += $(items[i]).attr('idx') + '" + Delimeter + @"'; {1}"">{0}</button>
                                        </div>
                                    </div>
                                </div>
                            </div>                    
                    ", XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "OK")
                     , Handler.GetScript("'NewValue=' + values"));
                }

                Content.Text += @"
                    </div>";
            }

            return Content.Text; 
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitHandler(ClientID);
            Content = new HTMLText();
            Controls.Add(Content);
            IsInitialized = true;     
            InnerRender();
        }

        public void InitHandler(String HandlerID)
        {
            Handler = new CallbackHandler(HandlerID);
            Handler.OnCallback += Handler_OnCallback;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
        }

        protected override void Render(HtmlTextWriter writer)
        {            
            base.Render(writer);
            ValueRendered = true;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

        protected void Handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {            
            String[] values = String.Concat(e.Parameter).Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "NewValue":
                        var newValues = values[1].Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries);
                        int id;
                        var oldValues = String.Concat(Value).Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in newValues)
                        {
                            if (int.TryParse(item, out id))
                            {                             
                                var SelectedItem = Items.List[id];
                                if (oldValues.IndexOf(SelectedItem.Value) == -1)
                                    oldValues.Add(SelectedItem.Value);
                            }
                        }
                        Value = String.Join(Delimeter, oldValues);
                        break;
                    case "AddValue":
                        newValues = values[1].Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(var item in newValues)
                            if (Items.List.Where(f => f.Value == item).Count() == 0) {
                                var newItem = Items.Add();
                                newItem.Text = newItem.Value = item;
                            }
                        
                        oldValues = String.Concat(Value).Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var item in newValues)
                            if (oldValues.IndexOf(item) == -1)
                                oldValues.Add(item);
                        Value = String.Join(Delimeter, oldValues);
                        break;
                    case "Remove":
                        oldValues = String.Concat(Value).Split(new String[] { Delimeter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (int.TryParse(values[1], out id))
                        {
                            var item = Items.List[id];
                            if (oldValues.IndexOf(item.Value) > -1)
                                oldValues.Remove(item.Value);
                            if (item.IsMissed)
                                Items.List.Remove(item);
                        }
                        Value = String.Join(Delimeter, oldValues);
                        break;
                }
            }            
            if (EditValueChanged != null)
                EditValueChanged(this, EventArgs.Empty);
            InnerRender();

            if (OnClickScript != "")
            {
                ASPxLabel label = new ASPxLabel();
                label.ClientSideEvents.Init = string.Format(@"function(s,e) {{ {0} }}", OnClickScript);
                Controls.Add(label);
            }
            if (OnCallback != null)
                OnCallback(source, e);
        }
    }
}
