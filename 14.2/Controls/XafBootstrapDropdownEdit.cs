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

using DevExpress.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapDropdownItem
    {
        public XafBootstrapDropdownItem()
        {
            
        }
        public XafBootstrapDropdownItem(XafBootstrapDropdownItems DropdownControl)
        {
            this.Dropdown = DropdownControl;
        }
        public XafBootstrapDropdownItems Dropdown;

        public String Text;
        public String Hint;
        public object Value;
        public String ImageUrl;
        public int Index
        {
            get
            {
                if (Dropdown != null)
                    return Dropdown.List.IndexOf(this);                
                return -1;
            }
        }
    }

    public class XafBootstrapDropdownItems : IEnumerable {

        public int ItemImageWidth;
        public int ItemImageHeight;

        public XafBootstrapDropdownItems() {
            List = new List<XafBootstrapDropdownItem>();
        }
        public IList<XafBootstrapDropdownItem> List;

        public XafBootstrapDropdownItem Add() {
            var item = new XafBootstrapDropdownItem(this);
            List.Add(item);
            return item;
        }

        public void Add(XafBootstrapDropdownItem Item)
        {
            if (List.IndexOf(Item) == -1)
            {
                Item.Dropdown = this;
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

    public class XafBootstrapDropdownEdit : System.Web.UI.WebControls.WebControl
    {
        public String ID = "";
        public string OnClickScript;
        public String PropertyName;
        private Boolean IsInitialized;
        public object Value
        {
            get
            {
                if (SelectedItem != null)
                    return SelectedItem.Value;
                return null;
            }
            set
            {
                SelectedItem = Items.List.Where(f => f.Value.Equals(value)).FirstOrDefault();
                if (SelectedItem == null)
                    SelectedItem = Items.List.Where(f => String.Concat(f.Value) == String.Concat(value)).FirstOrDefault();
                if (IsInitialized)
                    InnerRender();
            }
        }

        public Boolean TextOnly = false;        
        public XafBootstrapDropdownItems Items;
        public String EmptyText;

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        public XafBootstrapDropdownItem SelectedItem;        

        public XafBootstrapDropdownEdit()
        {
            Items = new XafBootstrapDropdownItems();

            EmptyText = XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapDropdownEdit", "EmptyText");
        }

        private CallbackHandler Handler;
        public void InnerRender()
        {
            Content.Text = "";
            
            if (TextOnly)
                Content.Text += String.Format(@"<span>{0}</span>", (SelectedItem != null) ? SelectedItem.Text : "");
            else
            {
                if (Items.Count > 3)
                {
                    Content.Text += String.Format(@"
                        <div class=""btn-group"">
                            <button type=""button"" class=""btn btn-default btn-sm dropdown-toggle"" onclick=""var elems = $(this).parent().find('.modal').modal();"">
                                <span data-bind=""label"">{0}</span>&nbsp;<span class=""caret""></span>
                            </button>                            
                            <div class=""modal fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
                                <div class=""modal-dialog"">
                                    <div class=""modal-content"">
                                        <div class=""modal-header"">
                                            <h4>{1}</h4>
                                        </div>
                                        <div class=""modal-body"">  
                                            <table class=""table table-hover"">
                    ", Value == null ? EmptyText : SelectedItem == null ? EmptyText : SelectedItem.Text, EmptyText);

                    foreach (XafBootstrapDropdownItem item in Items.List.OrderBy(f => f.Text))
                    {
                        String changeEvent = String.Format(@"onclick=""$(this).parents('.modal').modal('hide'); window.DataChanged=true; {0};""", Handler.GetScript(String.Format("'NewValue={0}'", item.Index)));
                        Content.Text += String.Format(@"
                                        <tr><td {1} style='vertical-align: middle'>{2}{0}</td></tr>", item.Text, changeEvent, String.Concat(item.ImageUrl) != "" ? String.Format("<img class='img-circle' style='max-width: {0}px; max-height: {1}px;' src='{2}'/> ", Items.ItemImageWidth, Items.ItemImageHeight, item.ImageUrl) : "");
                    }

                    Content.Text += String.Format(@"
                                            </table>
                                        </div>
                                        <div class=""modal-footer"">
                                            <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">{0}</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ", XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "Cancel"));
                }
                else
                {
                    Content.Text += String.Format(@"
                        <div class=""btn-group"">
                            <button type=""button"" class=""btn btn-default btn-sm dropdown-toggle"" data-toggle=""dropdown"">
                                <span data-bind=""label"">{0}</span>&nbsp;<span class=""caret""></span>
                            </button>
                            <ul class=""dropdown-menu"" role=""menu"">
                    ", Value == null ? EmptyText : SelectedItem == null ? EmptyText : SelectedItem.Text);
                        

                    foreach (XafBootstrapDropdownItem item in Items.List.OrderBy(f => f.Text))
                    {
                        String changeEvent = String.Format(@"onclick="" window.DataChanged=true;{0};""", Handler.GetScript(String.Format("'NewValue={0}'", item.Index)));
                        Content.Text += String.Format(@"
                            <li role=""presentation""><a role=""menuitem"" tabindex=""-1"" {1}><span class=""text-success"">{0}</span>{2}</a></li>"
                                , item.Text
                                , changeEvent
                                , (String.Concat(item.Hint) != "" ? "<br>" + item.Hint : ""));
                    }

                    Content.Text += @"
                        </ul>
                    </div>
                    ";
                }                
            }    
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Handler = new CallbackHandler(ClientID);
            Handler.OnCallback += Handler_OnCallback;
            Content = new HTMLText();
            Controls.Add(Content);
            IsInitialized = true;     
            InnerRender();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
        }

        protected override void Render(HtmlTextWriter writer)
        {            
            base.Render(writer);
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
                        int id;
                        if (int.TryParse(values[1], out id))
                            SelectedItem = Items.List.Where(f => f.Index == id).FirstOrDefault();
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
            
        }
    }
}
