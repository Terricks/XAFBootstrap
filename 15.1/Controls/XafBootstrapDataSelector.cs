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

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using XAF_Bootstrap.Editors.XafBootstrapPropertyEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapDataSelectorEdit : System.Web.UI.WebControls.WebControl
    {
        public string OnClickScript;

        public XafBootstrapDataSelectorEdit(ASPxPropertyEditor Editor, XafApplication Application, IObjectSpace ObjectSpace, LookupEditorHelper helper)
        {
            this.Editor = Editor;
            ContentStart = new HTMLText();
            ContentFinish = new HTMLText();
            Controls.Add(ContentStart);
            Controls.Add(ContentFinish);
            Helper = helper;

            EmptyText = XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Controls\XafBootstrapDataSelectorEdit", "EmptyText");
        }

        public Boolean TextOnly = false;                
        public String EmptyText;
        public String Caption;
        public IModelListView ListView;
        public String DisplayFormat = "{0}";        
        public LookupEditorHelper Helper;
        public String PropertyName;
        public ASPxPropertyEditor Editor;

        private Boolean Initialized;
        private object _value;
        public object Value
        {            
            get
            {   
                return _value;
            }
            set
            {
                _value = value;
                if (Initialized)
                    InnerRender();
            }
        }               

        public event EventHandler EditValueChanged;

        private HTMLText ContentStart;
        private HTMLText ContentFinish;

        private CallbackHandler Handler;

        void Handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            String[] values = String.Concat(e.Parameter).Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Count() > 1)
            {
                switch (values[0])
                {
                    case "Show":
                        if (Helper != null)                            
                        {
                            var view = Helper.CreateListView(Helper.ObjectSpace.GetObject(Editor.CurrentObject));
                            var SVP = new ShowViewParameters(view);
                            SVP.TargetWindow = TargetWindow.NewModalWindow;

                            DialogController dc = Helper.Application.CreateController<DialogController>();
                            dc.Accepting += new EventHandler<DialogControllerAcceptingEventArgs>(delegate(object sender, DialogControllerAcceptingEventArgs ev)
                            {
                                var os = (Editor as XafBootstrapLookupPropertyEditor).ObjectSpace;
                                foreach (var item in view.SelectedObjects)
                                {   
                                    Editor.PropertyValue = os.GetObject(item);
                                }   
                            });
                            SVP.Controllers.Add(dc);                            

                            Helper.Application.ShowViewStrategy.ShowView(SVP, new ShowViewSource(null, null));
                        }
                        break;
                    case "Link":
                        if (Value != null)
                        {
                            DetailView detailView = Helper.Application.CreateDetailView(Helper.ObjectSpace, Helper.ObjectSpace.GetObject(Value));

                            var dSVP = new ShowViewParameters(detailView);
                            dSVP.TargetWindow = TargetWindow.NewModalWindow;

                            DialogController dc = Helper.Application.CreateController<DialogController>();
                            dc.CancelAction.Active["Visible"] = false;                            
                            dSVP.Controllers.Add(dc);

                            Helper.Application.ShowViewStrategy.ShowView(dSVP, new ShowViewSource(null, null));
                        }
                        break;
                }
            }

            if (OnClickScript != "")
            {
                ASPxLabel label = new ASPxLabel();
                label.ClientSideEvents.Init = string.Format("function(s,e) {{ {0} }}", OnClickScript);
                Controls.Add(label);
            }
        }

        public void InnerRender()
        {
            ContentStart.Text = "";
            ContentFinish.Text = "";            

            String displayText = String.Concat(Value);
            try
            {
                displayText = Value == null ? EmptyText : String.Format(new ObjectFormatter(), String.Concat(DisplayFormat) == "" ? "{0}" : DisplayFormat, Value);
            }
            catch
            {
            }
            if (TextOnly)
            {
                ContentStart.Text += String.Format(@"<span><a href=""javascript:;"" onclick=""{1}"">{0}</a></span>", displayText, Handler.GetScript("'Link=true'"));
            }
            else
            {
                ContentStart.Text += String.Format(@"                    
                    <div class=""btn-group"" role=""group"" aria-label=""..."">                
                        <button type=""button"" class=""btn btn-default btn-sm"" onclick=""{0}"">
                            <span data-bind=""label"">{1}</span> 
                            <span class=""caret""></span>
                        </button>                        
                    </div>", Handler.GetScript("'Show=true'"), displayText);
            }    
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Handler = new CallbackHandler(ClientID);
            Handler.OnCallback += Handler_OnCallback;
            InnerRender();
            Initialized = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            
        }

        protected override void Render(HtmlTextWriter writer)
        {            
            base.Render(writer);
        }
    }
}
