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
using XAF_Bootstrap.Editors;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Globalization;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapDateEdit : XafBootstrapBaseControl
    {
        public String ID = "";
        public String Placeholder = "";
        public String AddonLeft = "";
        public String AddonRight = "";        
        public Boolean TextOnly = false;
        public String DisplayFormat = "dd.MM.yyyy H:mm";
        public string OnClickScript;

        private DateTime _Value;
        public DateTime Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                if (this.Initialized)
                    InnerRender();
            }
        }

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        public XafBootstrapDateEdit()
        {
            Content = new HTMLText();
            Controls.Add(Content);
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EncodeHtml = false;

            ClientSideEvents.Init = "function(s,e) { " + GetInitScript() + " }";
            ClientSideEvents.EndCallback = ClientSideEvents.Init;

            InnerRender();
        }

        public String GetInitScript(String area = "s.GetMainElement()")
        {
            string datePickerFormat = "";
            DisplayFormat = String.Concat(DisplayFormat);
            if (DisplayFormat.IndexOf("{0") == -1)
                DisplayFormat = String.Format("{{0:{0}}}", DisplayFormat);
            if (DisplayFormat != "{0:d}")
            {
                if (DisplayFormat.Length >= 4)
                    datePickerFormat = DisplayFormat.Substring(3, DisplayFormat.Length - 4).Replace("y", "Y").Replace("d", "D");
                else
                    datePickerFormat = DisplayFormat.Replace("y", "Y").Replace("d", "D");
            }
            else
                datePickerFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern.Replace("y", "Y").Replace("d", "D");

            return String.Format(@"
                $({4}).find(""#{0}"").each(function() {{   
                    var item = $(this);                 
                    item.datetimepicker({{                        
                        format: ""{2}"",
                        language: ""{3}""
                    }});
                    item.on(""dp.hide"",function (e) {{                        
                        {1}
                    }});
                }});"
                , PropertyName
                , GetCallbackScript("'NewValue=' + item.val()")
                , datePickerFormat
                , CultureInfo.CurrentCulture.IetfLanguageTag
                , area);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
        }

        public String InnerRender()
        {
            Content.Text = "";            

            if (AddonLeft != "")
                Content.Text += String.Format(@"<span class=""input-group-addon"">{0}</span>", AddonLeft);
            DisplayFormat = String.Concat(DisplayFormat);
            if (DisplayFormat.IndexOf("{0") == -1)
                DisplayFormat = String.Format("{{0:{0}}}", DisplayFormat);
            if (DisplayFormat == "{0:d}")
                DisplayFormat = String.Format("{{0:{0}}}", CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern);
            if (TextOnly)
            {
                if (Value != new DateTime())
                    Content.Text += String.Format(@"<span>{0}</span>", String.Format(DisplayFormat, Value));
            } 
            else
            {
                String changeEvent = String.Format(@" onchange="";window.DataChanged=true; {0} """, GetCallbackScript("'NewValue=' + this.value"));
                Content.Text += String.Format(@"<input  type=""text"" class=""form-control input-sm"" id=""{0}"" value=""{2}"">", PropertyName, Placeholder, (Value == new DateTime()) ? "" : String.Format(DisplayFormat, Value), changeEvent);
            }
            if (AddonRight != "")
                Content.Text += String.Format(@"<span class=""input-group-addon"">{0}</span>", AddonRight);

            return Content.Text;
        }        

        protected override void Render(HtmlTextWriter writer)
        {            
            base.Render(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

        }

        protected override void OnCallback(DevExpress.Web.CallbackEventArgsBase e)
        {
            base.OnCallback(e);
            String[] values = String.Concat(e.Parameter).Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "NewValue":
                        DateTime val;
                        if (DateTime.TryParse(values[1], out val))
                            Value = val;
                        break;
                }
            }            
            if (EditValueChanged != null)
                EditValueChanged(this, EventArgs.Empty);
            InnerRender();

            if (OnClickScript != "")
            {
                ASPxLabel label = new ASPxLabel();
                label.ClientSideEvents.Init = string.Format("function(s,e) {{ {0} }}", OnClickScript);
                Controls.Add(label);
            }
        }
    }
}
