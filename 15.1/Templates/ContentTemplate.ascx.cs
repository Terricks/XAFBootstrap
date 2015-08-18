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

using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Web;

using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp;
using XAF_Bootstrap.ModelExtensions;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using System.Linq;
using System.IO;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web.Controls;
using XAF_Bootstrap.BusinessObjects;

namespace XAF_Bootstrap.Templates
{
    public partial class ContentTemplate : TemplateContent, IXafPopupWindowControlContainer
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Helpers.AddMeta(Page);

            if (WebWindow.CurrentRequestWindow != null)
            {   
                WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
                if (Helpers.ContentHelper.Manager == null)
                    Helpers.ContentHelper.Manager = (Page as BaseXafPage).CallbackManager;

                var menuHandler = new XAF_Bootstrap.Controls.CallbackHandler();
                menuHandler.Register("MenuItemClickControllerCallback");

                menuHandler.OnCallback += menuHandler_OnCallback;

                var app = (WebApplication.Instance as XafApplication);
                if (app.MainWindow != null)
                    GenerateMenuItems(app.MainWindow.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Items);
            }            
        }        

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);            
            
        }

        protected override void OnUnload(EventArgs e)
        {
            if (WebWindow.CurrentRequestWindow != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
            }
            base.OnUnload(e);
        }

        private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e)
        {   
            VIC.Control.CssClass = "img-circle .img-responsive";
            TB.MenuItemsCreated += TB_MenuItemsCreated;
            VRAH.MenuItemsCreated += VRAH_MenuItemsCreated;
            EMA.MenuItemsCreated += EMA_MenuItemsCreated;            
            EditModeActions2.MenuItemsCreated += EditModeActions2_MenuItemsCreated;
            SAC.MenuItemsCreated += SAC_MenuItemsCreated;
            SHC.MenuItemsCreated += SHC_MenuItemsCreated;
        }

        void SHC_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(5); 
        }

        void VRAH_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(4); 
        }

        void SAC_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(3);            
        }

        void EMA_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(1);
        }

        void EditModeActions2_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(2);
        }

        public void UpdateActions(int Type = -1)
        {   
            
            var app = (WebApplication.Instance as XafApplication);            
            if (app.MainWindow != null)
                GenerateMenuItems(app.MainWindow.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Items); 

            if (Type == 5)
                GenerateActionsLayoutScript();
        }

        public void GenerateActionsLayoutScript()
        {   
            var lbl = new ASPxLabel();
            lbl.ClientSideEvents.Init = "function(s,e) { CalcActionsColumns(); }";
            VRAH.Controls.Add(lbl);
            Scripts.Controls.Add(lbl);            
        }

        void TB_MenuItemsCreated(object sender, EventArgs e)
        {
            UpdateActions(0);
        }        

        private void UpdateTRPVisibility()
        {
            UpdateActions();            
        }
        public override IActionContainer DefaultContainer
        {
            get
            {
                if (TB != null)
                {
                    return TB.FindActionContainerById("View");
                }
                return null;
            }
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
            InfoMessagesPanel.Text = string.Join("<br>", new List<string>(statusMessages).ToArray());
        }
        public override object ViewSiteControl
        {
            get
            {
                return VSC;
            }
        }
        public ThemedImageControl HeaderImageControl { get { return new ThemedImageControl(); } }

        #region Menu

        public String GetNodeID(ChoiceActionItem Node)
        {
            var Parent = Node.ParentItem;
            
            if (Parent != null)
                return GetNodeID(Parent) + "->" + Node.Id;
            return Node.Id;
        }

        public String GetNodeClasses(ChoiceActionItem Node, Boolean AsSelector = false)
        {
            if (Node != null)
                return (Node.ParentItem != null ? GetNodeClasses(Node.ParentItem, AsSelector) : "") + (AsSelector ? "." : " ") + FormatNodeId(Node.Id);
            return "";
        }

        public String GetNodeLevel(ChoiceActionItem Node)
        {
            String ret = "";
            var Parent = Node.ParentItem;

            if (Parent != null)            
                Parent = Parent.ParentItem;

            if (Parent != null)
            {
                ret = "&nbsp ";
                ret += GetNodeLevel(Parent);
            }
            return ret;
        }

        public String GetMenuItem(ChoiceActionItem Node)
        {
            if (Node.ParentItem == null || Node.ParentItem.ParentItem == null)
            {
                if (Node.Items == null || Node.Items.Count == 0)
                {
                    Guid ID = Guid.NewGuid();
                    Helpers.ContentHelper.MenuItems.Add(ID, Node);

                    String NodeID = GetNodeID(Node);
                    return String.Format("<li><a href=\"javascript:;\" onclick=\"if ($('#menuCollapseButton').is(':visible')) $('#menuCollapseButton').click();{1}\">{2}{0}</a></li>"
                        , Node.Caption
                        , Helpers.ContentHelper.GetScript("MenuItemClickControllerCallback", String.Format("'{0}'", NodeID))
                        , Node.Model is IModelGlyphicon && String.Concat((Node.Model as IModelGlyphicon).Glyphicon) != ""
                                     ? String.Format("<span class='glyphicon glyphicon-{0}'></span> ", (Node.Model as IModelGlyphicon).Glyphicon) : ""
                    );
                }
                else
                    return String.Format("<li><a>{1}{0}</a></li>"
                        , Node.Caption
                        , Node.Model is IModelGlyphicon && String.Concat((Node.Model as IModelGlyphicon).Glyphicon) != ""
                                     ? String.Format("<span class='glyphicon glyphicon-{0}'></span> ", (Node.Model as IModelGlyphicon).Glyphicon) : ""
                    );
            }
            else
            {
                if (Node.Items == null || Node.Items.Count == 0)
                {
                    Guid ID = Guid.NewGuid();
                    Helpers.ContentHelper.MenuItems.Add(ID, Node);

                    String NodeID = GetNodeID(Node);
                    return String.Format("<li class='collapse {3}'><a href=\"javascript:;\" onclick=\"if ($('#menuCollapseButton').is(':visible')) $('#menuCollapseButton').click();{1}\">{2}{0}</a></li>"
                        , Node.Caption
                        , Helpers.ContentHelper.GetScript("MenuItemClickControllerCallback", String.Format("'{0}'", NodeID))
                        , GetNodeLevel(Node) + "&nbsp " + String.Format("<span class='glyphicon glyphicon-{0}'></span> ",
                            Node.Model is IModelGlyphicon && String.Concat((Node.Model as IModelGlyphicon).Glyphicon) != ""
                                     ? (Node.Model as IModelGlyphicon).Glyphicon : "")
                        , GetNodeClasses(Node.ParentItem, false)

                    );
                }
                else
                    return String.Format("<li class='collapse {2}'><a>{1}{0}</a></li>"
                        , Node.Caption
                        , GetNodeLevel(Node) + "&nbsp " + String.Format("<span class='glyphicon glyphicon-{0}'></span> ",
                            Node.Model is IModelGlyphicon && String.Concat((Node.Model as IModelGlyphicon).Glyphicon) != ""
                                     ? (Node.Model as IModelGlyphicon).Glyphicon : "")
                        , GetNodeClasses(Node.ParentItem, false)

                    );
            }
        }

        public static String FormatNodeId(string id)
        {
            return String.Concat(id).Replace("@", "at_");
        }

        public String GenerateSubMenu(ChoiceActionItem NavNode, Boolean IsRoot = false)
        {
            if (NavNode != null)
            {
                if (NavNode.Items != null)
                {
                    if (NavNode.Items.Count == 0)
                        return GetMenuItem(NavNode);
                    else
                    {
                        String aRet = "";                        
                        if (NavNode.ParentItem != null && (NavNode.ParentItem.Items.Count > 1 && !IsRoot))
                            aRet = String.Format(@"<li class=""{2}"" onclick="" event.stopPropagation();"" ><a href=""javascript:;"" onclick=""toggleMenuItem(this, '.collapse{1}');"">{3}<span class=""glyphicon glyphicon-chevron-right""></span> {0}</a></li>"
                                , NavNode.Caption
                                , GetNodeClasses(NavNode, true)
                                , NavNode.ParentItem.ParentItem != null ? "collapse " + GetNodeClasses(NavNode.ParentItem, false) : ""
                                , GetNodeLevel(NavNode));
                        for (int i = 0; i < NavNode.Items.Count; i++)
                        {
                            var subNode = NavNode.Items[i];
                            if (!subNode.Active || !subNode.Enabled)
                                continue;
                            aRet += GenerateSubMenu(subNode);
                        }
                        
                        return aRet;
                    }
                }
                return "";
            }
            return "";
        }

        public void GenerateMenuItems(ChoiceActionItemCollection collection)
        {
            Helpers.ContentHelper.ClearMenu();
            var ULText = "";

            XafApplication App = (WebApplication.Instance as XafApplication);
            IModelNode NavigationItems = App.Model.GetNode("NavigationItems").GetNode("Items");

            IList<ChoiceActionItem> LeftItems = new List<ChoiceActionItem>();
            IList<ChoiceActionItem> RightItems = new List<ChoiceActionItem>();

            for (int i = 0; i < collection.Count; i++)
            {
                var RootItem = collection[i];                
                if (!RootItem.Active || !RootItem.Enabled)
                    continue;
                if (RootItem.Model is IModelMenuAlign)
                {
                    if ((RootItem.Model as IModelMenuAlign).MenuAlign == XAFBootstrapMenuAlign.Right)
                        RightItems.Add(RootItem);
                    else
                        LeftItems.Add(RootItem);
                }
                else
                    LeftItems.Add(RootItem);
            }

            if (LeftItems.Count > 0)
            {
                ULText += "<ul class='nav navbar-nav'>";
                foreach (var RootItem in LeftItems)
                {
                    if (RootItem.Items.Count > 0)
                        ULText += String.Format(
                          @"<li class='dropdown'>
                            <a href='javascript:;' class='dropdown-toggle' data-toggle='dropdown'>{2} <span>{0}</span> <span class='caret'></span>
                            </a>
                            <ul class='dropdown-menu' role='menu'>{1}</ul></li>
                           "
                            , RootItem.Caption
                            , GenerateSubMenu(RootItem, true)
                            , RootItem.Model is IModelGlyphicon && String.Concat((RootItem.Model as IModelGlyphicon).Glyphicon) != ""
                                 ? String.Format("<span class='glyphicon glyphicon-{0}'></span> ", (RootItem.Model as IModelGlyphicon).Glyphicon) : ""
                        );
                    else
                        ULText += GetMenuItem(RootItem);
                }
                ULText += "</ul>";
            }

            var Settings = "";
            var SettingsLi = "";
            Settings += @"
                    <li class='dropdown hidden-xs' style='position: absolute; right: 10px;'>
                      <a href='#' class='dropdown-toggle' data-toggle='dropdown'><span class='glyphicon glyphicon-cog settings-button'></span></a>
                      <ul class='dropdown-menu' role='menu'>";

            StringBuilder tempRes = new StringBuilder();
            foreach (XafMenuItem menuItem in Helpers.GetMenuActions(SAC))
            {
                if (!(Helpers.GenerateSingleChoiceAction(ref tempRes, menuItem, true, "", SAC.UniqueID + "_Callback")))
                    SettingsLi += String.Format("<li><a href='javascript:;' onclick='if ($(\"#settingsCollapseButton\").is(\":visible\")) $(\"#settingsCollapseButton\").click();{2}'><span class='glyphicon glyphicon-star'></span> {0}</a></li>"
                        , menuItem.Text
                        , menuItem.Name
                        , Helpers.ContentHelper.GetScript(SAC.UniqueID + "_Callback", String.Format("\"Action={0}\"", menuItem.Name)).Replace("'", "\""), "");
                else
                {
                    var choiceAction = (menuItem.ActionProcessor as MenuActionItemBase).Action as SingleChoiceAction;
                    SettingsLi += String.Format(@"<li class=""dropdown-header"">{0}</li>", choiceAction.Caption);
                    SettingsLi += String.Join("", choiceAction.Items.Select(f =>
                        String.Format("<li><a href=\"javascript:;\" onclick='if ($(\"#settingsCollapseButton\").is(\":visible\")) $(\"#settingsCollapseButton\").click();{1}'><span class=\"glyphicon glyphicon-star\"></span> {0}</a></li>"
                            , f.Caption
                            , Helpers.ContentHelper.GetScript(SAC.UniqueID + "_Callback", String.Format("\"Action={0},{1}\"", menuItem.Name, f.Id)).Replace("'", "\""))
                    ));
                }
            }
            Settings += SettingsLi;
            Settings +=
                  @"</ul>
                  </li>";


            //if (RightItems.Count > 0)
            {
                ULText += "<ul class='nav navbar-nav navbar-right'>";

                ULText += Settings;

                foreach (var RootItem in RightItems)
                {
                    if (RootItem is IModelMenuAlign)
                    {

                    }
                    if (RootItem.Items.Count > 0)
                        ULText += String.Format(
                            @"<li class='dropdown'>
                            <a href='#' class='dropdown-toggle' data-toggle='dropdown'>{0} <span class='caret'></span></a>
                            <ul class='dropdown-menu' role='menu'>{1}</ul></li>"
                            , RootItem.Caption
                            , GenerateSubMenu(RootItem, true)
                            , RootItem.Model is IModelGlyphicon && String.Concat((RootItem.Model as IModelGlyphicon).Glyphicon) != ""
                                 ? String.Format("<span class='glyphicon glyphicon-{0}'></span> ", (RootItem.Model as IModelGlyphicon).Glyphicon) : ""
                        );
                    else
                        ULText += String.Format("<li><a href='#about'>{0}</a></li>", GetMenuItem(RootItem));
                }
                ULText += "</ul>";
            }

            TopMenu.EncodeHtml = false;
            TopMenu.Text = String.Format(@"
                <div class='navbar navbar-default navbar-fixed-top {3}' role='navigation' id='navbar'>
                  <div class='container'>
                    <div class='navbar-header'>                        
                      <button type='button' id='menuCollapseButton' class='navbar-toggle collapsed' data-toggle='collapse' data-target='#mainMenu'>
                        <span class='sr-only'>Toggle navigation</span>
                        <span class='icon-bar'></span>
                        <span class='icon-bar'></span>
                        <span class='icon-bar'></span>
                      </button>
                      <button type='button' id='settingsCollapseButton' class='navbar-toggle collapsed' data-toggle='collapse' data-target='#settingsMenu'>
                        <span class='sr-only'>Toggle navigation</span>
                        <span class='glyphicon glyphicon-cog text-info settings-button'></span>
                      </button>                    
                      <a class='navbar-brand' href='#'>{1}</a>
                    </div>                    
                    <div class='navbar-collapse collapse' id='mainMenu'>
                        {0}
                    </div>
                    <div class='navbar-collapse collapse' id='settingsMenu'>
                        <ul class='nav hidden-sm hidden-md hidden-lg navbar-nav'>
                            {2}
                        </ul>
                    </div>
                  </div>
                    
                  <div class=""progress loading-progress"" style=""display: none"" id=""loadingProgress"">
                    <div class=""progress-bar progress-bar-info progress-bar-striped active"" role=""progressbar"" aria-valuenow=""100"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: 100%""></div>
                  </div>

                </div>"
                    , ULText
                    , (File.Exists(MapPath("~/images/logo.png")) ? "<img style='max-height: 30px; max-width: 160px; margin-top: -5px;' src='images/logo.png'>" : App.Model.Title)
                    , SettingsLi
                    , XAFBootstrapConfiguration.Instance != null && XAFBootstrapConfiguration.Instance.InverseNavBar ? "navbar-inverse" : "");
        }

        public ChoiceActionItem FindNode(ChoiceActionItemCollection items, String param)
        {
            var paramValues = param.Split(new String[] { "->" }, StringSplitOptions.RemoveEmptyEntries);            
            var item = items.Find(paramValues[0], ChoiceActionItemFindType.NonRecursive, ChoiceActionItemFindTarget.Any);
            if (paramValues.Length > 1)
                return FindNode(item.Items, String.Join("->", paramValues.Skip(1).Take(paramValues.Length - 1)));            
            return item;
        }

        void menuHandler_OnCallback(object source, CallbackEventArgs e)
        {
            var app = (WebApplication.Instance as XafApplication);
            var items = app.MainWindow.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Items;
            ChoiceActionItem Node = FindNode(items, String.Concat(e.Parameter));
            (WebApplication.Instance as XafApplication).MainWindow.GetController<ShowNavigationItemController>().ShowNavigationItemAction.DoExecute(Node);            
        }

        #endregion

        #region Object Actions

        public String GetSecurityActions()
        {
            var app = (WebApplication.Instance as XafApplication);
            if (app.MainWindow != null)
                GenerateMenuItems(app.MainWindow.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Items);

            return ""; // Helpers.BuildActionsMenu(SAC, false, "btn btn-default btn-xs");            
        }

        #endregion 

        protected override void OnInit(EventArgs e)
        {   
            base.OnInit(e);
        }
        
        protected void Page_Init(object sender, EventArgs e)
        {
        }

        public ActionContainerHolder MainToolBarActionContainer { get { return null; /* TB; */ } }
        public ActionContainerHolder SecurityActionContainer { get { return null; /* SAC; */ } }
        public ActionContainerHolder TopToolsActionContainer { get { return null; /* SHC; */ } }

        public XafPopupWindowControl XafPopupWindowControl
        {
            get { return PopupWindowControl; }
        }
    }
}
