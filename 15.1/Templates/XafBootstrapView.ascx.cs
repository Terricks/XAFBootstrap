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

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Layout;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using XAF_Bootstrap.Editors.XafBootstrapPropertyEditors;

namespace XafBootstrap.Web
{
    public enum ViewType
    {
        DetailView,
        ListView
    }

    public class HTMLViewItem : Control
    {
        public ViewItem ViewItem = null;
        public IObjectSpace ObjectSpace;
        public XafApplication Application;
        public XafBootstrapView BootstrapView;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (ViewItem != null)
            {
                String ModelID = "";
                if (ViewItem is PropertyEditor)
                {
                    ModelID = (ViewItem as PropertyEditor).Model.Id;
                }
                else
                {
                    if (ViewItem.GetType().GetProperty("Model") != null)
                    {
                        var Model = (IModelViewItem)ViewItem.GetType().GetProperty("Model").GetValue(ViewItem);
                        ModelID = Model.Id;
                    }
                }

                Func<String> controlsCreatedFunc = () =>
                {
                    var innerView = (ViewItem as DashboardViewItem).InnerView;
                    if (innerView != null && innerView is DetailView && ViewItem.View is DetailView)
                    {
                        (innerView as DetailView).ViewEditMode = (ViewItem.View as DetailView).ViewEditMode;
                        if (innerView.CurrentObject == null)
                            innerView.CurrentObject = innerView.ObjectSpace.FindObject((innerView as DetailView).Model.ModelClass.TypeInfo.Type, CriteriaOperator.Parse((ViewItem as DashboardViewItem).Model.Criteria), true);

                        ViewItem.View.ObjectSpace.Committed += new EventHandler(delegate
                        {
                            innerView.ObjectSpace.CommitChanges();
                        });

                        ViewItem.View.ObjectSpace.RollingBack += new EventHandler<System.ComponentModel.CancelEventArgs>(delegate
                        {
                            innerView.ObjectSpace.Rollback();
                        });

                        ViewItem.View.ObjectSpace.Refreshing += new EventHandler<System.ComponentModel.CancelEventArgs>(delegate
                        {
                            innerView.ObjectSpace.Refresh();
                        });
                    }
                    return "";
                };

                if (ViewItem is WebPropertyEditor)
                {
                    (ViewItem as WebPropertyEditor).AllowEdit["XafBootstrapAllowEdit"] = !(ModelID != "" && BootstrapView != null && BootstrapView.DisabledItems.IndexOf(ModelID) > -1);
                    if ((ViewItem as WebPropertyEditor).CurrentObject == null && ViewItem.View is DetailView)
                    {                        
                        ViewItem.CurrentObject = (ViewItem.View as DetailView).CurrentObject;
                    }
                }
                else
                    if (ViewItem is DashboardViewItem)
                    {

                        (ViewItem as DashboardViewItem).ControlCreated += new EventHandler<EventArgs>(delegate
                        {
                            controlsCreatedFunc();
                        });
                    }

                if (ViewItem.Control == null)
                    ViewItem.CreateControl();
                else
                    if (ViewItem is DashboardViewItem)
                        controlsCreatedFunc();

                if (ViewItem.Control != null && ViewItem.Control is Control)
                {
                    Controls.Clear();
                    var Control = ViewItem.Control as Control;                    
                    if (ViewItem is IXafBootstrapEditor
                       || (ViewItem is ListPropertyEditor && (ViewItem as ListPropertyEditor).ListView != null && (ViewItem as ListPropertyEditor).ListView.Editor is IXafBootstrapListEditor))
                    {
                        Controls.Add(Control);
                    }
                    else if (ViewItem is DashboardViewItem && (ViewItem as DashboardViewItem).View is DetailView) {
                            Controls.Add(Control);
                        }
                        else
                        {
                            Controls.Add(new HTMLText() { Text = "<div class='table-responsive'>" });
                            Controls.Add(Control);
                            Controls.Add(new HTMLText() { Text = "</div>" });
                        }                    
                }                
            }   
        }
    }

    public enum ElemType
    {
        FirstObject,
        LastObject,
        SingleObject
    }

    public partial class XafBootstrapView : System.Web.UI.UserControl
    {
        public XafBootstrapView()
        {   
            DetailViewContent = new Control();
            Controls.Add(DetailViewContent);
            DisabledItems = new List<String>();
        }

        public Boolean IsRootView = false;

        public IList<String> DisabledItems;

        public IList<XafBootstrapBaseControl> XafBootstrapControls = new List<XafBootstrapBaseControl>();
        public DevExpress.ExpressApp.View View;
        public Control ControlToRender;
        public String ModelListViewID;
        public object DataSource;
        public ViewType ViewType;
        public String PropertyName;
        public SelectionObjectType SelectionType;
                
        private Control DetailViewContent;
        public XafBootstrapTable ListTable;

        #region Elements rendering

        public Control Content
        {
            get
            {
                return DetailViewContent;
            }
        }

        Boolean RebuildAfterBuilt;
        Boolean IsBuildingContent;
        public void InnerRender()
        {            
            if (!IsBuildingContent)
            {
                try
                {
                    IsBuildingContent = true;
                    BuildContent();
                    if (RebuildAfterBuilt)
                        BuildContent();
                    RebuildAfterBuilt = false;
                }
                finally
                {
                    IsBuildingContent = false;
                }
            }
            else
            {
                RebuildAfterBuilt = true;
            }
        }

        private void AddContent(Control control, object DataContainer = null)        
        {
            if (DataContainer == null)
                DataContainer = Content;
            if (DataContainer is Control)
            {
                var Container = (DataContainer as Control);                
                if (control is HTMLViewItem)
                {
                    if (Container.Controls.OfType<HTMLViewItem>().Where(f => f.ViewItem.Id == (control as HTMLViewItem).ViewItem.Id).Count() == 0)
                        Container.Controls.Add(control);
                }
                else
                    if (Container.Controls.IndexOf(control) == -1)
                    {

                        Container.Controls.Add(control);
                    }
            }
            else
            {
                if (DataContainer is List<Control>)
                {
                    var List = (DataContainer as List<Control>);
                    if (control is HTMLViewItem)
                    {
                        if (List.OfType<HTMLViewItem>().Where(f => f.ViewItem.Id == (control as HTMLViewItem).ViewItem.Id).Count() == 0)
                            List.Add(control);
                    }
                    else
                        if (List.IndexOf(control) == -1)
                        {
                            List.Add(control);
                        }
                }
            }
        }

        private Boolean BuildRecursiveTag(Control control, Control Parent, Boolean Start, ElemType type, object Container)
        {
            if (control is CustomPanel)
                AddContent(new HTMLText() 
                {                
                    Text =
                        Start ?
                        "<div>"
                        :
                        "</div>"
                }, Container);
            else if (control is LayoutGroupTemplateContainer)
            {
                var layoutGroup = (LayoutGroupTemplateContainer)control;

                if (Parent is TabbedGroupTemplateContainer)
                {
                    var manager = Helpers.RequestManager;
                    String activeTabId = String.Concat(manager.Request.Form[(Parent as TabbedGroupTemplateContainer).Model.Id + "_state"]);
                    Boolean isActive = ((activeTabId != "" && layoutGroup.Model.Id == activeTabId) || (activeTabId == "" && type == ElemType.FirstObject));
                    AddContent(new HTMLText()
                    {
                        Text =
                            Start ?
                            String.Format("<li role=\"presentation\"{2}><a href=\"#{0}\" role=\"tab\" data-toggle=\"tab\" onclick=\"$('#{3}_state').val('{0}'); {4} \">{1}</a>".Replace("'","&quot;")
                                , layoutGroup.Model.Id
                                , layoutGroup.Caption
                                , isActive ? " class='active'" : ""
                                , (Parent as TabbedGroupTemplateContainer).Model.Id
                                , manager.Request.Form[layoutGroup.Model.Id + "_activated"] != "1" && !isActive ? String.Format("$('#{0}_activated').val(1); {1}".Replace("'", "&quot;"), layoutGroup.Model.Id, handler.GetScript("")) : ""
                                )
                            :
                            "</li>"
                    }, Container);
                }
                else
                {
                    if (Parent is LayoutGroupTemplateContainer &&
                        (Parent as LayoutGroupTemplateContainer).Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal &&
                        (Parent as LayoutGroupTemplateContainer).Items.Count > 0)
                    {
                        var ParentGroup = (Parent as LayoutGroupTemplateContainer);
                        int Col = 12;
                        if (ParentGroup.Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal)
                            Col = (int)(12 / ParentGroup.Items.Count);
                        if (Col > 12)
                            Col = 12;
                        if (Col < 1)
                            Col = 1;

                        AddContent(new HTMLText()
                        {
                            Text =
                                Start ?
                                String.Format(@"<div class=""col-sm-{0}"">", Col)
                                :
                                "</div>"
                        }, Container);

                    }
                    else
                    {
                        if (Parent != null && layoutGroup.Model.ShowCaption == true && Start)
                        {
                            AddContent(new HTMLText()
                            {
                                Text =                                    
                                    String.Format(@"
                                            <div class=""panel panel-default"">
                                              <div class=""panel-heading"">{0}</div>
                                              <div class=""panel-body"">
                                        ", layoutGroup.Model.Caption)

                            }, Container);
                        }

                        if (layoutGroup.Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal &&
                            layoutGroup.Items.Count > 0)
                        {

                            AddContent(new HTMLText()
                            {
                                Text =
                                    Start ?
                                    @"<div class=""row"">"
                                    :
                                    "</div>"
                            }, Container);
                        }
                        else
                        {
                            AddContent(new HTMLText()
                            {
                                Text =
                                    Start ?
                                    @"<div>"
                                    :
                                    "</div>"
                            }, Container);
                        }

                        if (Parent != null && layoutGroup.Model.ShowCaption == true && !Start)
                        {
                            AddContent(new HTMLText()
                            {
                                Text =
                                           @"</div>
                                        </div><br>"
                            }, Container);
                        }
                    }
                }
            }            
            else if (control is TabbedGroupTemplateContainer)
            {
                var tabbedGroup = (control as TabbedGroupTemplateContainer);
                if (tabbedGroup.Items.Count == 0)
                    return true;
                var manager = Helpers.RequestManager;
                String activeTabId = String.Concat(manager.Request.Form[tabbedGroup.Model.Id + "_state"]);
                AddContent(new HTMLText()
                {                    
                    Text =
                        Start ?
                        String.Format("<div style=\"padding:2px\"><ul class='nav nav-tabs' role='tablist'><input type=\"hidden\" name = \"{0}_state\" runat=\"server\" id = \"{0}_state\" value=\"{1}\">", tabbedGroup.Model.Id, activeTabId)
                        :
                        "</ul></div>"
                }, Container);
            }
            else if (control is LayoutItemTemplateContainer)
            {                
              
                if ((control as LayoutItemTemplateContainer).ViewItem == null)
                    return true;

                if (Start && Parent is LayoutGroupTemplateContainer &&
                    (Parent as LayoutGroupTemplateContainer).Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal &&
                    (Parent as LayoutGroupTemplateContainer).Items.Count > 0)
                {
                    var ParentGroup = (Parent as LayoutGroupTemplateContainer);
                    int Col = 12;
                    if (ParentGroup.Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal)
                        Col = (int)(12 / ParentGroup.Items.Count);
                    if (Col > 12)
                        Col = 12;
                    if (Col < 1)
                        Col = 1;

                    AddContent(new HTMLText()
                    {
                        Text =                            
                            String.Format(@"<div class=""col-sm-{0}"">", Col)                            
                    }, Container);
                }

                if ((control as LayoutItemTemplateContainer).ViewItem is PropertyEditor)
                {                    
                    var editor = (control as LayoutItemTemplateContainer).ViewItem as PropertyEditor;
                    if (editor is WebPropertyEditor)
                        if (View is DetailView)
                            (editor as WebPropertyEditor).ViewEditMode = (View as DetailView).ViewEditMode;
                        else if (View is DashboardView)
                            (editor as WebPropertyEditor).ViewEditMode = ViewEditMode.Edit;                        

                    if (editor != null)
                    {
                        if (Start)
                        {
                            if (editor is ListPropertyEditor)
                                AddContent(new HTMLText()
                                {
                                    Text = String.Format(@"
                                        <div class='form-group'>                                            
                                    ", editor.Caption)
                                }, Container);
                            else
                                if ((control as LayoutItemTemplateContainer).ShowCaption && !(editor is XafBootstrapBooleanPropertyEditor)) 
                                {
                                    var delimeter = "";
                                    var location = "";
                                    switch((control as LayoutItemTemplateContainer).Model.CaptionLocation) 
                                    {
                                        case DevExpress.Utils.Locations.Left: 
                                        case DevExpress.Utils.Locations.Right:
                                            location = "form-inline form-group";
                                            delimeter = ":";
                                            break;
                                        default:
                                            location = "";
                                            break;
                                    }
                                    
                                    AddContent(new HTMLText()
                                    {
                                        Text = String.Format(@"
                                            <div class='{1}'>
                                                <label><b>{0}{2}</b></label>
                                                <div class='form-group'>
                                        ", editor.Caption
                                            , location
                                            , delimeter)
                                    }, Container);                                    
                                } else
                                    AddContent(new HTMLText()
                                    {
                                        Text = @"<div class='form-group'>"
                                    }, Container);

                            if (editor is ListPropertyEditor)
                            {
                                AddContent(new HTMLViewItem() { BootstrapView = this, ViewItem = editor, ObjectSpace = View.ObjectSpace, Application = (WebApplication.Instance as XafApplication) }, Container);
                            }
                            else
                            {
                                if (!(editor is IXafBootstrapEditor))
                                {
                                }
                                AddContent(new HTMLViewItem() { BootstrapView = this, ViewItem = editor, ObjectSpace = View.ObjectSpace, Application = (WebApplication.Instance as XafApplication) }, Container);
                            }
                        }
                        else
                        {
                            if (editor is ListPropertyEditor)
                            {
                                AddContent(new HTMLText()
                                {
                                    Text = @"</div>"
                                }, Container);
                            }
                            else if ((control as LayoutItemTemplateContainer).ShowCaption && !(editor is XafBootstrapBooleanPropertyEditor))
                            {
                                AddContent(new HTMLText()
                                {
                                    Text = @"</div></div>"
                                }, Container);
                            }
                            else
                            {
                                AddContent(new HTMLText()
                                {
                                    Text = @"</div>"
                                }, Container);
                            }                                
                        }
                    }
                }
                else
                {
                    if (Start)
                    {
                        AddContent(new HTMLViewItem() { BootstrapView = this, ViewItem = (control as LayoutItemTemplateContainer).ViewItem, ObjectSpace = View.ObjectSpace, Application = (WebApplication.Instance as XafApplication) }, Container);
                    }
                }

                if (!Start && Parent is LayoutGroupTemplateContainer &&
                    (Parent as LayoutGroupTemplateContainer).Model.Direction == DevExpress.ExpressApp.Layout.FlowDirection.Horizontal &&
                    (Parent as LayoutGroupTemplateContainer).Items.Count > 0)
                {
                    
                    AddContent(new HTMLText()
                    {
                        Text =                            
                            "</div>"
                    }, Container);
                }
            }
            else
            {
                if (Start)
                {
                    AddContent(new HTMLText() { Text = "<div class='table-responsive'>" }, Container);
                    AddContent(control, Container);
                    return false;
                }
                else
                {
                    AddContent(new HTMLText() { Text = "</div>" }, Container);
                }
            }
            return true;
        }

        internal class InnerHTMLText : HTMLText
        {
            public InnerHTMLText()
            {
            }

            public InnerHTMLText(String Text)
                : base(Text)
            {
            }
        }
        
        private Boolean BuildRecursiveElement(Control control, Control Parent, ElemType type, object Container)
        {
            if (control is LayoutItemTemplateContainerBase)
            {
                bool Visible = true;
                bool Enabled = true;

                var baseControl = (control as LayoutItemTemplateContainerBase);

                #region CALCULATE ELEMENT APPEARANCE
                if (View.ObjectTypeInfo != null)
                {
                    var member = View.ObjectTypeInfo.FindMember(baseControl.Model.Id);
                    if (member != null)
                        foreach (var appearanceItem in member.FindAttributes<AppearanceAttribute>().Where(f =>
                            (String.Concat(f.Context) == ""
                            || String.Concat(f.Context) == "Any"
                            || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("DetailView") > -1)
                        ))
                        {
                            if (Convert.ToBoolean(View.ObjectSpace.GetExpressionEvaluator(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(View.CurrentObject)))
                            {
                                var aItem = (appearanceItem as DevExpress.ExpressApp.ConditionalAppearance.IAppearance);
                                switch (aItem.Visibility)
                                {
                                    case ViewItemVisibility.ShowEmptySpace:
                                    case ViewItemVisibility.Hide:
                                        Visible = false;
                                        break;
                                    case ViewItemVisibility.Show:
                                        Visible = true;
                                        break;
                                }
                                switch (aItem.Enabled)
                                {
                                    case true:
                                    case false:
                                        Enabled = (Boolean)aItem.Enabled;
                                        break;
                                }
                            }
                        }

                    foreach (var appearanceItem in View.ObjectTypeInfo.FindAttributes<AppearanceAttribute>().Where(f =>
                        (String.Concat(f.TargetItems).IndexOf("*") > -1
                        || String.Concat(f.TargetItems).Split(',').Where(s => s.Trim() == baseControl.Model.Id).Count() > 0)
                        && (
                            String.Concat(f.Context) == ""
                            || String.Concat(f.Context) == "Any"
                            || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("DetailView") > -1
                        )
                    ))
                    {
                        if (Convert.ToBoolean(View.ObjectSpace.GetExpressionEvaluator(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(View.CurrentObject)))
                        {
                            var aItem = (appearanceItem as DevExpress.ExpressApp.ConditionalAppearance.IAppearance);
                            switch (aItem.Visibility)
                            {
                                case ViewItemVisibility.ShowEmptySpace:
                                case ViewItemVisibility.Hide:
                                    Visible = false;
                                    break;
                                case ViewItemVisibility.Show:
                                    Visible = true;
                                    break;
                            }
                            switch (aItem.Enabled)
                            {
                                case true:
                                case false:
                                    Enabled = (Boolean)aItem.Enabled;
                                    break;
                            }
                        }
                    }
                }
                #endregion

                if (!Visible)
                    return false;
                else
                {
                    if (!Enabled)
                        DisabledItems.Add((control as LayoutItemTemplateContainerBase).Model.Id);
                }
            }

            if (control is TabbedGroupTemplateContainer)
            {                
                var container = (control as TabbedGroupTemplateContainer);
                var i = 0;
                var minVisibleIndex = -1;
                var maxVisibleIndex = -1;

                IList<int> VisibleIndexes = new List<int>();

                IList<KeyValuePair<string, LayoutItemTemplateContainerBase>> AccessableItems = new List<KeyValuePair<string, LayoutItemTemplateContainerBase>>();
                foreach (KeyValuePair<string, LayoutItemTemplateContainerBase> item in container.Items)
                {
                    bool Visible = true;
                    bool Enabled = true;

                    #region CALCULATE ELEMENT APPEARANCE
                    if (View.ObjectTypeInfo != null)
                    {
                        var member = View.ObjectTypeInfo.FindMember(item.Value.Model.Id);
                        if (member != null)
                            foreach (var appearanceItem in member.FindAttributes<AppearanceAttribute>().Where(f => (String.Concat(f.Context) == "" || String.Concat(f.Context) == "Any" || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("DetailView") > -1)))
                            {
                                if (Convert.ToBoolean(View.ObjectSpace.GetExpressionEvaluator(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(View.CurrentObject)))
                                {
                                    var aItem = (appearanceItem as DevExpress.ExpressApp.ConditionalAppearance.IAppearance);
                                    switch (aItem.Visibility)
                                    {
                                        case ViewItemVisibility.ShowEmptySpace:
                                        case ViewItemVisibility.Hide:
                                            Visible = false;
                                            break;
                                        case ViewItemVisibility.Show:
                                            Visible = true;
                                            break;
                                    }
                                    switch (aItem.Enabled)
                                    {
                                        case true:
                                        case false:
                                            Enabled = (Boolean)aItem.Enabled;
                                            break;
                                    }
                                }
                            }

                        foreach (var appearanceItem in View.ObjectTypeInfo.FindAttributes<AppearanceAttribute>().Where(f => String.Concat(f.TargetItems).Split(',').Where(s => s.Trim() == item.Value.Model.Id).Count() > 0 && (String.Concat(f.Context) == "" || String.Concat(f.Context) == "Any" || f.Context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().IndexOf("DetailView") > -1)))
                        {
                            if (Convert.ToBoolean(View.ObjectSpace.GetExpressionEvaluator(View.ObjectTypeInfo.Type, CriteriaOperator.Parse(appearanceItem.Criteria)).Evaluate(View.CurrentObject)))
                            {
                                var aItem = (appearanceItem as DevExpress.ExpressApp.ConditionalAppearance.IAppearance);
                                switch (aItem.Visibility)
                                {
                                    case ViewItemVisibility.ShowEmptySpace:
                                    case ViewItemVisibility.Hide:
                                        Visible = false;
                                        break;
                                    case ViewItemVisibility.Show:
                                        Visible = true;
                                        break;
                                }
                                switch (aItem.Enabled)
                                {
                                    case true:
                                    case false:
                                        Enabled = (Boolean)aItem.Enabled;
                                        break;
                                }
                            }
                        }
                    }
                    #endregion

                    if (Visible)
                    {
                        if (minVisibleIndex == -1)
                            minVisibleIndex = i;
                        maxVisibleIndex = i;

                        VisibleIndexes.Add(i);
                        AccessableItems.Add(item);
                    }
                    i++;
                }

                if (VisibleIndexes.Count > 0)
                {
                    //BUILD TAB DATA
                    IDictionary<int, List<Control>> checkTabs = new Dictionary<int, List<Control>>();                    
                    i = 0;
                    Boolean ActiveWasSet = false;
                    foreach (KeyValuePair<string, LayoutItemTemplateContainerBase> item in AccessableItems)
                    {
                        var checkTab = new List<Control>();
                        
                        var manager = Helpers.RequestManager;
                        String activeTabId = String.Concat(manager.Request.Form[container.Model.Id + "_state"]);
                        String classActive = " activeState";
                        if (activeTabId != "")
                        {
                            ActiveWasSet = true;
                            classActive = (item.Value.Model.Id == activeTabId) ? " active" : "";
                        }

                        AddContent(new InnerHTMLText() { Text = String.Format("<div role='tabpanel' class='tab-pane fade in{1}' id='{0}'><input type='hidden' id='{0}_activated' value='{2}' name = '{0}_activated'/>", item.Value.Model.Id, classActive, manager.Request.Form[item.Value.Model.Id + "_activated"] == "1" || (i == 0) ? "1" : "0") }, checkTab);
                        AddContent(new InnerHTMLText() { Text = String.Format(@"<div class=""row no-margin""><div class=""col-sm-12"">") }, checkTab);

                        var checkTabCounter = new List<Control>();
                        BuildRecursiveElement(item.Value, null, (i == 0) ? ElemType.FirstObject : (i == 0) ? ElemType.LastObject : ElemType.SingleObject, checkTabCounter);                        
                        if (manager.Request.Form[item.Value.Model.Id + "_activated"] == "1" || (i == 0))
                            checkTab.AddRange(checkTabCounter);
                        else
                            if (checkTabCounter.Count(f => !(f is InnerHTMLText)) > 0)
                            {                                
                                AddContent(new HTMLText()
                                {
                                    Text = @"<div class=""progress loading-progress"">
  <div class=""progress-bar progress-bar-info progress-bar-striped active"" role=""progressbar"" aria-valuenow=""100"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: 100%"">
    <span class=""sr-only"">40% Complete (success)</span>
  </div>
</div>" }, checkTab);
                                }

                        AddContent(new InnerHTMLText() { Text = String.Format(@"</div></div>") }, checkTab);
                        AddContent(new InnerHTMLText() { Text = "</div>" }, checkTab);
                        
                        checkTabs.Add(i, checkTab);
                        i++;
                    }

                    AddContent(new InnerHTMLText() { Text = "<div class='panel panel-default'>" }, Container);

                    //BUILD HEADERS
                    i = 0;
                    var k = 0;
                    BuildRecursiveTag(control, Parent, true, type, Container);
                    var visibleCount = (checkTabs.Where(f => f.Value.Count > 4).Count());
                    foreach (KeyValuePair<string, LayoutItemTemplateContainerBase> item in AccessableItems)
                    {
                        if (checkTabs[i].Count(f => !(f is InnerHTMLText)) > 0)
                        {
                            BuildRecursiveTag(item.Value, control, true, (k == 0) ? ElemType.FirstObject : (k == visibleCount) ? ElemType.LastObject : ElemType.SingleObject, Container);
                            BuildRecursiveTag(item.Value, control, false, (k == 0) ? ElemType.FirstObject : (k == visibleCount) ? ElemType.LastObject : ElemType.SingleObject, Container);
                            k++;
                        }
                        i++;
                    }
                    BuildRecursiveTag(control, Parent, false, type, Container);

                    //DISPLAY TAB DATA
                    AddContent(new InnerHTMLText() { Text = "<div class='tab-content'>" }, Container);
                    foreach (var tab in checkTabs.Where(f => f.Value.Count > 2))
                    {
                        foreach (Control c in tab.Value)
                        {
                            if (!ActiveWasSet && c is HTMLText && (c as HTMLText).Text.IndexOf("activeState") > -1)
                            {
                                ActiveWasSet = true;
                                (c as HTMLText).Text = (c as HTMLText).Text.Replace("activeState", "active");
                            }

                            AddContent(c, Container);
                        }
                    }
                    AddContent(new InnerHTMLText() { Text = "</div>" }, Container);

                    AddContent(new InnerHTMLText() { Text = "</div><br>" }, Container);
                }
            }
            else
            {
                var checkContainer = new List<Control>();
                if (BuildRecursiveTag(control, Parent, true, type, checkContainer))
                {
                    if (control is LayoutGroupTemplateContainer)
                    {
                        var container = (control as LayoutGroupTemplateContainer);

                        var i = 0;
                        foreach (KeyValuePair<string, LayoutItemTemplateContainerBase> item in container.Items)
                        {
                            BuildRecursiveElement(item.Value, control, (i == 0) ? ElemType.FirstObject : (i == container.Items.Count) ? ElemType.LastObject : ElemType.SingleObject, checkContainer);
                            i++;
                        }

                    }
                    else
                        if (control.Controls.Count > 0)
                        {
                            var i = 0;
                            foreach (Control item in control.Controls.OfType<Control>().ToList())
                            {
                                BuildRecursiveElement(item, control, (i == 0) ? ElemType.FirstObject : (i == control.Controls.Count) ? ElemType.LastObject : ElemType.SingleObject, checkContainer);
                                i++;
                            }
                        }                    
                }
                BuildRecursiveTag(control, Parent, false, type, checkContainer);

                if (checkContainer.OfType<InnerHTMLText>().Count() < checkContainer.Count)
                    foreach (Control c in checkContainer)
                        AddContent(c, Container);
            }
            return true;
        }        

        #endregion
        
        private void BuildContent()
        {   
            Content.Controls.Clear();
            switch(ViewType) {
                case Web.ViewType.DetailView:
                    if (View == null)
                        return;

                    if (ControlToRender == null)                    
                        if (View is DetailView && !(View as DetailView).IsControlCreated)                            
                            View.CreateControls();                        
                    

                    AddContent(new HTMLText() { Text = @"<div>" }, Content);
                    if (ControlToRender != null)
                        BuildRecursiveElement(ControlToRender, null, ElemType.SingleObject, Content);
                    else
                        BuildRecursiveElement((Control)View.Control, null, ElemType.SingleObject, Content);
                    AddContent(new HTMLText() { Text = @"</div>" }, Content);

                    break;
            
                case Web.ViewType.ListView:                    
                    ListTable = new XafBootstrapTable();                    

                    ListTable.ListView = (IModelListView)(WebApplication.Instance as XafApplication).Model.Views[ModelListViewID];
                    ListTable.DataSource = DataSource;
                    ListTable.PropertyName = PropertyName;
                    ListTable.SelectionType = SelectionType;
                    Content.Controls.Clear();
                    Content.Controls.Add(ListTable);
                    break;
            }
        }

        CallbackHandler handler;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            handler = new CallbackHandler(ClientID);
            handler.OnCallback += handler_OnCallback;
            DetailViewContent = new Control();
            Controls.Add(DetailViewContent);
            InnerRender();            
        }

        void handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {            
            InnerRender();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);                                     
        }
    }
}