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
    public class XafBootstrapNumericEdit : XafBootstrapBaseControl
    {
        public String ID = "";
        public String Placeholder = "";
        public String AddonLeft = "";
        public String AddonRight = "";        
        public Boolean TextOnly = false;
        public Boolean IsPassword = false;
        public int RowCount = 1;       
        public bool EncodeInnerHtml = true;
        public String DisplayFormat = "{0}";
        public string OnClickScript;

        public event EventHandler EditValueChanged;

        private decimal _Value;
        public decimal Value
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

        private HTMLText Content;

        public XafBootstrapNumericEdit()
        {
            EncodeHtml = true;
        }

        public void InnerRender()
        {
            Content.Text = "";

            string val = String.Format(DisplayFormat, Value).Replace(",",".");
            
            if (TextOnly)
                Content.Text += String.Format(@"<span>{0}</span>", val);
            else
            {
                val = val.Replace(" ", "");
                String changeEvent = String.Format(@"onchange=""window.DataChanged=true; {0} """, GetCallbackScript("'NewValue=' + $(this).val()"));                

                Content.Text += String.Format(@"
                <div class=""input-group number-spinner"">
                    <span class=""input-group-btn data-dwn"">
					    <button type=""button"" class=""btn btn-default btn-sm"" data-dir=""dwn"" onclick=""window.DataChanged=true; $('#{0}_input').each(function() {{ $(this).val(parseFloat($(this).val()) - 1); {1} }});""> - </button>
				    </span>", ClientID, GetCallbackScript("'NewValue=' + $(this).val()"));

                Content.Text += String.Format(@"<input type=""{3}"" class=""form-control input-sm"" placeholder=""{0}"" value =""{1}"" {2} id=""{4}_input"">", Placeholder, val, changeEvent, IsPassword ? "password" : "text", ClientID);

                Content.Text += String.Format(@"
                    <span class=""input-group-btn data-up"">
					    <button type=""button"" class=""btn btn-default btn-sm"" data-dir=""up"" onclick=""window.DataChanged=true; $('#{0}_input').each(function() {{ $(this).val(parseFloat($(this).val()) + 1); {1} }});""> + </button>
				    </span> 
                </div>
                ", ClientID, GetCallbackScript("'NewValue=' + $(this).val()"));    
            }
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
            String[] values = String.Concat(e.Parameter).Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "NewValue":
                        decimal val = 0;
                        if (decimal.TryParse(String.Concat(values[1]).Replace(".",","), out val))
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
                label.ClientSideEvents.Init = string.Format(@"function(s,e) {{ {0} }}", OnClickScript);
                Controls.Add(label);
            }
        }
    }
}
