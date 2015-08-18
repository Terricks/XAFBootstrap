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

using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Text;
using System.ComponentModel;
using XAF_Bootstrap.BusinessObjects;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.String), "XafBootstrapThemeConfigurationEditor", false), Browsable(false)]
    public class XafBootstrapThemeConfigurationEditor : ASPxPropertyEditor, IXafBootstrapEditor, IComplexViewItem
    {
        internal class CollectorControl : System.Web.UI.WebControls.WebControl
        {
        }

        public XafBootstrapThemeConfigurationEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {             
        }

        #region IComplexViewItem Members

        public IObjectSpace ObjectSpace;
        public XafApplication Application;

        void IComplexViewItem.Setup(DevExpress.ExpressApp.IObjectSpace objectSpace, DevExpress.ExpressApp.XafApplication application)
        {
            this.ObjectSpace = objectSpace;
            this.Application = application;
        }

        #endregion

        internal CollectorControl Collector;
        public XafBootstrapDropdownEdit Edit;
        private void InitEdit()
        {            
            Edit = new XafBootstrapDropdownEdit();
            Edit.Items.ItemImageHeight = 100;
            Edit.Items.ItemImageWidth = 100;
            
            var path = HttpContext.Current.Server.MapPath("bootstrap_themes");
            if (Directory.Exists(path))
            {
                foreach (var item in Directory.GetDirectories(path).Select(f => new DirectoryInfo(f)))
                {
                    var editItem = Edit.Items.Add();
                    editItem.Text = item.Name;
                    editItem.Value = item.Name;
                    if (File.Exists(item.FullName + "\\preview.jpg"))
                        editItem.ImageUrl = "bootstrap_themes\\" + item.Name + "\\preview.jpg";
                }
            }            
        }

        private void InitCollector(Boolean isEditing)
        {            
            Collector = new CollectorControl();

            CallbackHandler handler = new CallbackHandler("ThemeConfigurationCallbackHandler");
            handler.OnCallback += handler_OnCallback;

            var popup = new HTMLText(String.Format(@"
                <div class=""modal fade"" id=""popupThemeLoader"">
                    <div class=""modal-dialog modal-lg"">
                        <div class=""modal-content"">
                            <div class=""modal-header"">
                                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button>
                                <h4 class=""modal-title"">Theme CSS uploading</h4>
                            </div>
                            <div class=""modal-body"">
                                <div class=""form-group"">
                                    <label for=""themeName"">Theme name</label>
                                    <input name=""themeName"" type=""text"" class=""form-control"" id=""themeName"" placeholder=""Name of destination theme folder"">                        
                                </div>

                                <div class=""form-group"">
                                    <label for=""bootstrapCSS"">Bootstrap CSS</label>
                                    <textarea name=""bootstrapCSS"" class=""form-control"" rows=""20"" id=""bootstrapCSS"" placeholder=""Main Bootstrap CSS content""></textarea>                        
                                </div>
                            </div>
                            <div class=""modal-footer"">
                                <button type=""button"" class=""btn btn-default pull-right"" data-dismiss=""modal"">Cancel</button>
                                <button type=""button"" class=""btn btn-default pull-right"" data-dismiss=""modal"" onclick="" startProgress(); setTimeout(function() {{ {0} }}, 1000);"">OK</button>
                            </div>
                        </div>
                    </div>
                </div>
            ", handler.GetScript("'save'")));

            var popupButton = new HTMLText(@"<button class=""btn btn-default btn-sm"" type=""button"" onclick=""$('#popupThemeLoader').modal();""><span class=""glyphicon glyphicon-upload""></span> Upload</button>");            

            Collector.Controls.Add(Edit);            
            if (isEditing)
            {
                Collector.Controls.Add(popup);
                Collector.Controls.Add(popupButton);
            }
        }

        void handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            if (e.Parameter == "save")
            {
                var themeName = String.Concat(HttpContext.Current.Request.Form["themeName"]).Replace("/","").Replace("\\","");
                if (themeName != "")
                {
                    var bootstrapCSS = String.Concat(HttpContext.Current.Request.Form["bootstrapCSS"]);                    

                    var path = HttpContext.Current.Server.MapPath("bootstrap_themes/" + themeName + "/css/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if (bootstrapCSS != "")
                    {
                        using(FileStream fs = new FileStream(path + "bootstrap.min.css", FileMode.OpenOrCreate)) {                            
                            using (StreamWriter outfile = new StreamWriter(fs))
                            {
                                outfile.Write(bootstrapCSS);
                                outfile.Close();
                            }
                            fs.Close();
                        }
                    }

                    if (CurrentObject is XAFBootstrapConfiguration)
                    {
                        (CurrentObject as XAFBootstrapConfiguration).SetThemeChanged();
                        (CurrentObject as XAFBootstrapConfiguration).Session.CommitTransaction();
                    }

                    InitEdit();
                }
            }
        }

        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = !AllowEdit;
            Edit.EditValueChanged += new EventHandler(EditValueChangedHandler);

            InitCollector(true);            
            return Collector;
        }

        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore()
        {
            InitEdit();
            Edit.TextOnly = true;

            InitCollector(false);
            return Collector;
        }

        protected override void SetImmediatePostDataCompanionScript(string script)
        {        
        }
        
        protected override object GetControlValueCore()
        {
            return Edit.Value;
        }
        
        protected override void ReadEditModeValueCore()
        {
            Edit.Value = String.Concat(PropertyValue);
        }
        
        protected override void ReadViewModeValueCore()
        {
            Edit.Value = String.Concat(PropertyValue);
        }
    }
}
