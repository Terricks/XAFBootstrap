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
using DevExpress.Web;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapCheckboxEdit : XafBootstrapBaseControl
    {
        public String ID = "";
        
        public Boolean Value;
        public Boolean TextOnly = false;
        public Boolean IsPassword = false;
        public String Text;
        public String OnChangeScript;

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        public XafBootstrapCheckboxEdit()
        {
            EncodeHtml = true;
        }

        public void InnerRender()
        {
            Content.Text = "";

            String changeEvent = String.Format(@"onchange="" window.DataChanged=true; {0} """, GetCallbackScript("'NewValue=' + $(this).is(':checked')"));
            Content.Text += String.Format(@"
                <div class=""checkbox"">
                    <label>
                        <input type=""checkbox"" {1} {2} {3}> {0}
                    </label>
                </div>", Text, TextOnly ? "" : changeEvent, Value ? "checked" : "", TextOnly ? "disabled" : "");
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
                        Value = Boolean.Parse(values[1]);
                        break;
                }
            }            
            if (EditValueChanged != null)
                EditValueChanged(this, EventArgs.Empty);
            InnerRender();

            if (OnChangeScript != "")
            {
                ASPxLabel label = new ASPxLabel();
                label.ClientSideEvents.Init = string.Format("function(s,e) {{ {0} }}", OnChangeScript);
                Controls.Add(label);
            }
        }
    }
}
