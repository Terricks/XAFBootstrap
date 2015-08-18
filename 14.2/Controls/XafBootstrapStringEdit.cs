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

using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Boolean TextOnly = false;
        public Boolean IsPassword = false;
        public int RowCount = 1;       
        public bool EncodeInnerHtml = true;

        public event EventHandler EditValueChanged;

        private HTMLText Content;

        private String _Value;
        public String Value
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
                String changeEvent = String.Format(@"window.DataChanged=true;  $('#{0}_changed').val('1');", ClientID);
                if (RowCount <= 1)
                {
                    Content.Text += String.Format(@"<input type=""{3}"" name=""{4}"" class=""form-control input-sm"" placeholder=""{0}"" value =""{1}"" onchange=""{2}"" onkeypress=""{2}"">", Placeholder, val, changeEvent, IsPassword ? "password" : "text", ClientID);
                }
                else
                {
                    Content.Text += String.Format(@"<textarea class=""form-control"" name=""{4}"" rows=""{0}""  onchange=""{2}"" onkeypress=""{2}"" placeholder=""{3}"">{1}</textarea>", RowCount, val, changeEvent, Placeholder, ClientID);
                }
            }
            if (AddonRight != "")
                Content.Text += String.Format(@"<span class=""input-group-addon"">{0}</span>", AddonRight);
            Content.Text += String.Format(@"<input name=""{0}_changed"" id=""{0}_changed"" value=""0"" type=""hidden"">", ClientID);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Content = new HTMLText();
            Controls.Add(Content);

            if (Helpers.RequestManager.Request.Form[ClientID + "_changed"] == "1")
            {
                Value = Helpers.RequestManager.Request.Form[ClientID];
                if (EditValueChanged != null)
                    EditValueChanged(this, EventArgs.Empty);
            }            
            
            EncodeHtml = false;
            InnerRender();
        }        
    }
}
