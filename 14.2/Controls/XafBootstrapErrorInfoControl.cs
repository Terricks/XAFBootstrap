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

using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XAF_Bootstrap.Controls
{
    [ToolboxItem(false)]
    public class XafBootstrapErrorInfoControl : Panel
    {
        private string errorImageName;
        private Table element;
        private ASPxCheckBox checkIgnore;
        protected override void Render(HtmlTextWriter writer)
        {
            this.Style.Clear();
            element.CssClass = "";
            if (DesignMode)
            {
                TableRow row = new TableRow();
                Rows.Add(row);
                row.Cells.Add(new TableCell());
                row.Cells[0].Text = "Error message";
            }
            else
            {
                ErrorInfo pageErr = ErrorHandling.Instance.GetPageError();
                if (pageErr != null && Visible)
                {
                    var validExc = pageErr.Exception as ValidationException;
                    if (validExc != null)
                    {
                        HandleValidExc(validExc);
                    }
                    else
                    {
                        AddError(element, ErrorImageName, pageErr.Exception.Message);
                    }
                }
                else
                {
                    foreach (var item in Controls)
                        (item as Control).Visible = false;

                }
                ErrorHandling.Instance.ClearPageError();
            }
            base.Render(writer);
        }
        private void HandleValidExc(ValidationException exc)
        {
            var newRow = new TableRow();
            Rows.Add(newRow);
            var newCell = new TableCell()
            {
                Text = exc.MessageHeader,
                ColumnSpan = 2
            };

            newCell.Attributes["id"] = ClientID + "_Header";
            newCell.Attributes[EasyTestTagHelper.TestField] = "ErrorInfo";
            newCell.Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;

            newRow.Cells.Add(newCell);

            foreach (var validRes in new[] { ValidationResultType.Error, ValidationResultType.Warning, ValidationResultType.Information })
            {
                string errorMessage = exc.GetMessages(validRes);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    AddError(element, validRes.ToString(), errorMessage, "Validation" + validRes);
                }
            }
            if (exc.Result.ValidationOutcome == ValidationOutcome.Warning)
            {
                TableRow row2 = new TableRow();
                row2.Cells.Add(new TableCell());
                var cell = new TableCell();
                cell.ColumnSpan = 2;
                row2.Cells.Add(cell);
                Rows.Add(row2);
                cell.Controls.Add(checkIgnore);
                checkIgnore.Visible = true;
                checkIgnore.Text = CaptionHelper.GetLocalizedText("Texts", "IgnoreWarning");
                checkIgnore.Attributes[EasyTestTagHelper.TestField] = checkIgnore.Text;
                checkIgnore.Attributes[EasyTestTagHelper.TestControlClassName] = JSASPxCheckBoxTestControl.ClassName;
            }
        }
        protected void AddError(Table table, string imageName, string message, string testTag = "ErrorInfo")
        {
            TableRow errorRow = new TableRow();
            table.Rows.Add(errorRow);                        
            
            var lblCell = new TableCell();
            if (!string.IsNullOrEmpty(imageName))
            {
                Image image = new Image()
                {
                    ImageUrl = ImageLoader.Instance.GetImageInfo(imageName).ImageUrl
                };
                lblCell.Controls.Add(image);
            }
            lblCell.Attributes["id"] = ClientID + "_" + testTag;
            lblCell.Attributes[EasyTestTagHelper.TestField] = testTag;
            lblCell.Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;            
            Literal label = new Literal();
            lblCell.Controls.Add(label);            

            errorRow.Cells.Add(lblCell);
            string formattedMessage = System.Web.HttpUtility.HtmlEncode(message.Trim());
            if (formattedMessage.Length > 2 && formattedMessage.Substring(0, 2) == "- ")
                formattedMessage = "&nbsp" + formattedMessage.Substring(2, formattedMessage.Length - 2);
            string[] lines = formattedMessage.Split('\n');
            formattedMessage = string.Join("<br>", lines);
            label.Text = formattedMessage;
        }
        public XafBootstrapErrorInfoControl()
            : base()
        {
            Controls.Add(new HTMLText(@"<div class=""row""><div class=""col-sm-12""><div class=""alert alert-danger alert-dismissible""><button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""Close""><span aria-hidden=""true"">&times;</span></button>"));
            element = new Table();
            Controls.Add(element);
            ErrorImageName = "Error";
            checkIgnore = new ASPxCheckBox();
            Controls.Add(checkIgnore);
            checkIgnore.ID = "Ch";
            checkIgnore.CheckedChanged += new EventHandler(checkbox_CheckedChanged);
            checkIgnore.Visible = false;
            Controls.Add(new HTMLText(@"</div></div></div>"));
        }
        void checkbox_CheckedChanged(object sender, EventArgs e)
        {
            ErrorInfo error = ErrorHandling.Instance.GetPageError();
            /*if (error != null)
            {
                error.IgnoreError = ((ASPxCheckBox)sender).Checked;
            }*/
        }
        [DefaultValue("Error")]
        public string ErrorImageName
        {
            get { return errorImageName; }
            set
            {
                errorImageName = value;
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TableRowCollection Rows
        {
            get { return element.Rows; }
        }
    }
}
