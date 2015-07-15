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
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.Editors.XafBootstrapPropertyEditors
{
    [PropertyEditor(typeof(System.Drawing.Image), "XafBootstrapImagePropertyEditor", true)]
    public class XafBootstrapImagePropertyEditor : ASPxImagePropertyEditor, IXafBootstrapEditor
    {
        public XafBootstrapImagePropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {             
        }

        private void MakeResponsive(System.Web.UI.WebControls.WebControl control, bool force = false, int options = 2)
        {   
            if (control != null)
            {
                if (control is System.Web.UI.WebControls.Image && (options == 0 || options == 2))
                    if (force)
                        control_LoadImage(control, EventArgs.Empty);
                    else
                        control.Load += control_LoadImage;

                if (control is System.Web.UI.WebControls.Panel && (options == 1 || options == 2))
                    if (force)
                        control_LoadDiv(control, EventArgs.Empty);
                    else
                        control.Load += control_LoadDiv;  
                                       
                            
                foreach (var c in control.Controls.OfType<System.Web.UI.WebControls.WebControl>())
                    MakeResponsive(c as System.Web.UI.WebControls.WebControl, force, options);
            }
        }

        void control_LoadDiv(object sender, EventArgs e)
        {
            try
            {
                var panel = (sender as System.Web.UI.WebControls.Panel);
                if (panel != null)
                    panel.Style["display"] = "";
                panel.Load -= control_LoadDiv;
            }
            catch
            {
            }
        }

        void control_LoadImage(object sender, EventArgs e)
        {
            try
            {
                var panel = (sender as System.Web.UI.WebControls.Image);
                if (panel != null)
                {
                    panel.CssClass = "img-responsive col-xs-12";
                    if (GetFormattedValue() == "" && !(WebWindow.CurrentRequestPage != null && !WebWindow.CurrentRequestPage.IsCallback && WebWindow.CurrentRequestPage.IsPostBack))
                        panel.Style["Display"] = "none";
                    panel.Load -= control_LoadImage;
                }                
            }
            catch
            {
            }
        }
        
        protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore()
        {
            var control = base.CreateEditModeControlCore();

            if (GetFormattedValue() != "" || (WebWindow.CurrentRequestPage != null && !WebWindow.CurrentRequestPage.IsCallback && WebWindow.CurrentRequestPage.IsPostBack))
                MakeResponsive(control, true);
            else 
                control.Load += control_PreRender;

            return control;
        }

        void control_PreRender(object sender, EventArgs e)
        {
            MakeResponsive(sender as System.Web.UI.WebControls.WebControl);
        }
        
        
        protected override System.Web.UI.WebControls.WebControl CreateViewModeControlCore()
        {
            var control = base.CreateViewModeControlCore();
            control.Load += control_PreRender;
            return control;
        }        
    }
}
