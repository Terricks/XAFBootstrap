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

using XAF_Bootstrap.Editors;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapStringEdit : XafBootstrapBaseControl
    {
        public String ID = "";
        public String Placeholder = "";
        public String AddonLeft = "";
        public String AddonRight = "";
        public String Value = "";
        public Boolean TextOnly = false;
        public Boolean IsPassword = false;
        public int RowCount = 1;       
        public bool EncodeInnerHtml = true;

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        public XafBootstrapStringEdit()
        {
            EncodeHtml = true;
        }

        public void InnerRender()
        {
            Content.Text = "";

            String val = Value;
            if (EncodeInnerHtml)
                val = HttpContext.Current.Server.HtmlEncode(Value);

            if (AddonLeft != "")
                Content.Text += String.Format(@"<span class=""input-group-addon"">{0}</span>", AddonLeft);
            if (TextOnly)
                Content.Text += String.Format(@"<span>{0}</span>", val);
            else
            {
                String changeEvent = String.Format(@"onchange="" window.DataChanged=true; {0} """, GetCallbackScript("'NewValue=' + $(this).val()"));
                if (RowCount <= 1)
                {
                    Content.Text += String.Format(@"<input type=""{3}"" class=""form-control input-sm"" placeholder=""{0}"" value =""{1}"" {2}>", Placeholder, val, changeEvent, IsPassword ? "password" : "text");
                }
                else
                {
                    Content.Text += String.Format(@"<textarea class=""form-control"" rows=""{0}"" {2} placeholder=""{3}"">{1}</textarea>", RowCount, val, changeEvent, Placeholder);
                }
            }
            if (AddonRight != "")
                Content.Text += String.Format(@"<span class=""input-group-addon"">{0}</span>", AddonRight);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Content = new HTMLText();
            Controls.Add(Content);
            EncodeHtml = false;
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

        protected override void OnCallback(DevExpress.Web.CallbackEventArgsBase e)
        {
            base.OnCallback(e);
            String[] values = String.Concat(e.Parameter).Split(new char[] { '=' }, StringSplitOptions.None);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "NewValue":
                        Value = String.Join("=", values.ToList().Skip(1).Take(values.Length-1));
                        break;
                }
            }            
            if (EditValueChanged != null)
                EditValueChanged(this, EventArgs.Empty);
            InnerRender();
        }
    }
}
