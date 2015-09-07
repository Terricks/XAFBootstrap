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

using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Web;
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XAF_Bootstrap.Controls
{    
    [ParseChildren(ChildrenAsProperties = true)]
    [PersistChildrenAttribute(false)]
    public class XbActionContainerHolder : Panel, INamingContainer
    {
        private string categories;
        public Dictionary<ActionBase, MenuActionItemBase> actionObjects;
        private List<IActionContainer> actionContainers;

        private String _ContentBefore;
        public String ContentBefore
        {
            get
            {
                return _ContentBefore;
            }
            set
            {
                _ContentBefore = value;
            }
        }

        private String _ContentAfter;
        public String ContentAfter
        {
            get
            {
                return _ContentAfter;
            }
            set
            {
                _ContentAfter = value;
            }
        }

        private List<WebActionContainer> CreateContainers(string[] containerIds)
        {
            List<WebActionContainer> containers = new List<WebActionContainer>();
            foreach (string containerId in containerIds)
            {
                WebActionContainer webActionContainer = new WebActionContainer();
                webActionContainer.ContainerId = containerId;
                containers.Add(webActionContainer);
            }
            return containers;
        }
        private MenuActionItemBase GenerateactionObject(ActionBase action)
        {
            CreateCustomMenuActionItemEventArgs args = new CreateCustomMenuActionItemEventArgs(action);
            OnCreateCustomMenuactionObject(args);
            if (args.ActionItem != null)
            {
                return args.ActionItem;
            }
            return (MenuActionItemBase)GenerateActionObjectCore(action);
        }
        private void Action_Changed(object sender, ActionChangedEventArgs e)
        {
            if (e.ChangedPropertyType == ActionChangedType.Active)
            {
                ClearChildControls(false);
            }
        }
        private ICallbackManagerHolder CallbackManagerHolder
        {
            get
            {
                Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), Page.GetType(), "Page");
                return (ICallbackManagerHolder)Page;
            }
        }
        internal static List<XafMenuItem> GetAllMenuItems(DevExpress.Web.MenuItemCollection items)
        {
            List<XafMenuItem> result = new List<XafMenuItem>();
            foreach (DevExpress.Web.MenuItem item in items)
            {
                if (item is XafMenuItem)
                {
                    result.Add((XafMenuItem)item);
                }
                result.AddRange(GetAllMenuItems(item.Items));
            }
            return result;
        }

        internal static Dictionary<string, string> GetClickHandlers(XafCallbackManager callbackManager, ASPxMenu menu, string uniqueID)
        {
            List<XafMenuItem> allMenuItems = GetAllMenuItems(menu.RootItem.Items);
            Dictionary<string, string> clickHandlers = new Dictionary<string, string>();
            foreach (XafMenuItem item in allMenuItems)
            {
                MenuActionItemBase MenuActionItemBase = item.ActionProcessor as MenuActionItemBase;
                if (MenuActionItemBase != null)                
                    MenuActionItemBase.SetClientClickHandler(callbackManager, uniqueID);
            }
            return clickHandlers;
        }
        protected virtual void OnCreateCustomMenuactionObject(CreateCustomMenuActionItemEventArgs args)
        {
            if (CreateCustomMenuactionObject != null)
                CreateCustomMenuactionObject(this, args);
        }
        protected virtual void OnMenuactionObjectCreated(MenuActionItemCreatedEventArgs args)
        {
            if (MenuactionObjectCreated != null)
                MenuactionObjectCreated(this, args);
        }
        private WebActionBaseItem GenerateActionObjectCore(ActionBase action)
        {
            MenuActionItemBase actionObject = null;
            if (action is SimpleAction)
            {
                actionObject = CreateSimpleactionObject((SimpleAction)action);
            }
            else if (action is SingleChoiceAction)
            {
                actionObject = CreateSingleChoiceactionObject((SingleChoiceAction)action);
            }
            else if (action is ParametrizedAction)
            {
                actionObject = CreateParametrizedactionObject((ParametrizedAction)action);
            }
            else if (action is PopupWindowShowAction)
            {
                actionObject = CreatePopupWindowShowactionObject((PopupWindowShowAction)action);
            }
            else if (action is ActionUrl && action.SelectionDependencyType == SelectionDependencyType.Independent)
            {
                actionObject = GenerateActionUrlItem((ActionUrl)action);
            }
            else
            {
                actionObject = CreateDefaultactionObject(action);
            }
            return actionObject;
        }

        private CallbackHandler Callback;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            WebActionContainerHelper.TryRegisterActionContainer(this, ActionContainers);
            if (!DesignMode)
            {
                Callback = new CallbackHandler(UniqueID + "_Callback");
                Callback.OnCallback += Callback_OnCallback;
            }
        }
        protected override void OnUnload(EventArgs e)
        {
            WebActionContainerHelper.UnregisterActionContainer(this, ActionContainers);
            base.OnUnload(e);
        }        
        
        protected virtual MenuActionItemBase CreateDefaultactionObject(ActionBase action)
        {
            return new DefaultMenuActionItem(action);
        }
        protected virtual MenuActionItemBase GenerateActionUrlItem(ActionUrl actionUrl)
        {
            return new ActionUrlItem(actionUrl);
        }
        protected virtual MenuActionItemBase CreatePopupWindowShowactionObject(PopupWindowShowAction popupWindowShowAction)
        {
            return new PopupWindowActionMenuActionItem(popupWindowShowAction);
        }
        protected virtual MenuActionItemBase CreateParametrizedactionObject(ParametrizedAction parametrizedAction)
        {
            ParametrizedActionMenuActionItem actionObject = new ParametrizedActionMenuActionItem(parametrizedAction);
            return actionObject;
        }
        protected virtual MenuActionItemBase CreateSingleChoiceactionObject(SingleChoiceAction singleChoiceAction)
        {
            if (singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode)
            {
                if (singleChoiceAction.IsHierarchical())
                {
                    return new SingleChoiceActionItemAsHierarchicalModeActionMenuItem(singleChoiceAction);
                }
                else
                {
                    SingleChoiceActionAsModeMenuActionItem actionObject = new SingleChoiceActionAsModeMenuActionItem(singleChoiceAction);
                    return actionObject;
                }
            }
            else
            {
                SingleChoiceActionItemAsOperationActionMenuItem menuItem = new SingleChoiceActionItemAsOperationActionMenuItem(singleChoiceAction);
                return menuItem;
            }
        }
        protected virtual MenuActionItemBase CreateSimpleactionObject(SimpleAction simpleAction)
        {
            return new SimpleActionMenuActionItem(simpleAction);
        }

        public bool HasActiveActions()
        {
            bool isVisible = false;
            foreach (IActionContainer container in ActionContainers)
            {
                if (((WebActionContainer)container).HasActiveActions)
                {
                    isVisible = true;
                    break;
                }
            }
            return isVisible;
        }

        public string Categories
        {
            get { return categories; }
            set
            {
                if (categories != value)
                {
                    categories = value;
                    SetActionContainers(categories);
                }
            }
        }

        public Boolean LeftDirection { get; set; }
        public String ItemClass { get; set; }
        public String Tag { get; set; }
        public String CallbackName { get; set; }
        public String ClickScript { get; set; }
        public String ContainerClass { get; set; }
        public String DefaultIcon { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<IActionContainer> ActionContainers
        {
            get { return actionContainers; }
        }

        public IActionContainer FindActionContainerById(string containerId)
        {
            foreach (IActionContainer container in actionContainers)
            {
                if (container.ContainerId == containerId)
                {
                    return container;
                }
            }
            return null;
        }

        public XbActionContainerHolder()
        {
            Initlialize();
        }

        public HTMLText Control;
        public void Initlialize()
        {
            actionContainers = new List<IActionContainer>();
            actionObjects = new Dictionary<ActionBase, MenuActionItemBase>();
            Control = new HTMLText();
            Controls.Add(Control);

            LeftDirection = true;
            ItemClass = "btn btn-primary btn-sm";
            Tag = "button";
            CallbackName = "ObjectActionControllerCallback";
            ClickScript = "";
            ContainerClass = "actions";
            DefaultIcon = "glyphicon-star";
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MenuActionItemBase FindactionObject(ActionBase action)
        {
            MenuActionItemBase result;
            actionObjects.TryGetValue(action, out result);
            return result;
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.Write(GetDesignTimeHtml());
            }
            else
            {
                EnsureChildControls();
                base.Render(writer);
            }
        }
        private void OnMenuItemsCreated()
        {
            if (MenuItemsCreated != null)
            {
                MenuItemsCreated(this, EventArgs.Empty);
            }
        }

        private void BuildMenuActions()
        {
            Control.Text = "";
            if (Visible && actionObjects.Count > 0)
            {
                Control.Text += ContentBefore;
                Control.Text += Helpers.BuildActionsMenu(this, UniqueID + "_Callback", LeftDirection, ItemClass, Tag, ClickScript, ContainerClass, DefaultIcon);
                Control.Text += ContentAfter;
            }
            else
            {
                CssClass = "";
            }
        }

        void Callback_OnCallback(object source, CallbackEventArgs e)
        {
            if (actionObjects.Count == 0)
                CreateMenuItems();

            var param = String.Concat(e.Parameter);
            var data = "";

            if (param.Length > 7 && param.Substring(0, 7) == "Action=")
            {
                param = param.Substring(7, param.Length - 7);
                if (param.IndexOf(",") > -1)
                {
                    data = param.Substring(param.IndexOf(",") + 1, param.Length - param.IndexOf(",") - 1);
                    param = param.Substring(0, param.IndexOf(","));
                }

                var items = actionObjects.Where(a => a.Key.Id == param);
                if (items.Count() > 0)
                {
                    ActionBase action = items.FirstOrDefault().Key;
                    if (action is SingleChoiceAction && data != "")
                    {
                        var singleChoice = (action as SingleChoiceAction);
                        singleChoice.SelectedItem = singleChoice.Items.Where(f => f.Id == data).FirstOrDefault();
                    }
                    if (action is ParametrizedAction)
                    {
                        var parametrizedAction = (action as ParametrizedAction);
                        parametrizedAction.DoExecute(data);
                    }
                    else
                        Helpers.ProcessAction(action);
                }

            }
            EnsureChildControls();
        }

        public Boolean IsMenuItemsCreated = false;
        public void CreateMenuItems()
        {
            if (Control == null) return;

            if (!Controls.Contains(Control))
            {
                Controls.Add(Control);
            }
            foreach (IActionContainer actionContainer in actionContainers)
            {
                RegisterContainerActions(actionContainer);
            }

            BuildMenuActions();
            UpdateMenuVisibility();
            OnMenuItemsCreated();

            IsMenuItemsCreated = true;
        }
        public void UpdateMenuVisibility()
        {
            BuildMenuActions();            
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateMenuItems();
        }
        protected internal new void EnsureChildControls()
        {
            base.EnsureChildControls();
            BuildMenuActions();
        }
        private void container_ActionRegistered(object sender, ActionEventArgs e)
        {
            ClearChildControls(false);
        }

        private void RegisterContainerActions(IActionContainer container)
        {
            bool isFirstMenuItem = true;            
            foreach (ActionBase action in container.Actions)
            {
                action.Changed -= new EventHandler<ActionChangedEventArgs>(Action_Changed);
                action.Changed += new EventHandler<ActionChangedEventArgs>(Action_Changed);
                if (action.Active && action.Enabled)
                {
                    CustomCreateActionControlEventArgs customCreateActionControlEventArgs = new CustomCreateActionControlEventArgs(action, container);
                    if (CustomGenerateActionControl != null)
                    {
                        CustomGenerateActionControl(this, customCreateActionControlEventArgs);
                    }
                    if (!customCreateActionControlEventArgs.Handled)
                    {
                        MenuActionItemBase actionObject = GenerateactionObject(action);
                        actionObjects[action] = actionObject;
                        if (isFirstMenuItem)
                        {
                            actionObject.MenuItem.SlidingBeginGroup = true;
                            isFirstMenuItem = false;
                        }

                        MenuActionItemCreatedEventArgs args = new MenuActionItemCreatedEventArgs(actionObject);
                        OnMenuactionObjectCreated(args);
                    }
                }
            }
        }
        public event EventHandler<CustomCreateActionControlEventArgs> CustomGenerateActionControl;
        internal void UpdateGroupsAndIndexes()
        {
            bool isFirstItem = true;
            foreach (var item in actionObjects)
            {
                if (!isFirstItem)                
                    item.Value.MenuItem.BeginGroup = true;
                isFirstItem = false;
            }
        }
        private void container_ActionsClearing(object sender, EventArgs e)
        {
            UnsubsribeActionEvents((IActionContainer)sender);
        }
        private void UnsubsribeActionEvents(IActionContainer container)
        {
            if (container != null)
            {
                foreach (ActionBase action in container.Actions)
                {
                    action.Changed -= new EventHandler<ActionChangedEventArgs>(Action_Changed);
                }
            }
        }
        private void ClearChildControls(bool disposing)
        {
            if (disposing)
            {
                if (actionContainers != null)
                {
                    foreach (IActionContainer container in actionContainers)
                    {
                        UnsubsribeActionEvents(container);
                        ((WebActionContainer)container).ActionRegistered -= new EventHandler<ActionEventArgs>(container_ActionRegistered);
                        ((SimpleActionContainer)container).ActionsClearing -= new EventHandler<EventArgs>(container_ActionsClearing);
                        IDisposable disposable = container as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
                    actionContainers.Clear();
                }
            }

            ChildControlsCreated = false;
        }
        public override void Dispose()
        {
            ClearChildControls(true);
            base.Dispose();
        }
        public void ClearContiners()
        {
            foreach (WebActionContainer container in actionContainers)
            {
                container.Dispose();
            }
            actionContainers.Clear();
        }
        public void SetActionContainers(string categories)
        {
            SetActionContainers(CreateContainers(categories.Split(';')));
        }
        public void SetActionContainers(IList<WebActionContainer> containers)
        {
            ClearContiners();
            foreach (WebActionContainer container in containers)
            {
                AddActionContainer(container);
            }
        }
        public void AddActionContainer(WebActionContainer container)
        {
            container.Owner = null;
            container.ActionRegistered += new EventHandler<ActionEventArgs>(container_ActionRegistered);
            container.ActionsClearing += new EventHandler<EventArgs>(container_ActionsClearing);
            actionContainers.Add(container);
            ClearChildControls(false);
        }
        public void Clear()
        {
            ClearChildControls(false);
        }
        public event EventHandler<MenuActionItemCreatedEventArgs> MenuactionObjectCreated;
        public event EventHandler<CreateCustomMenuActionItemEventArgs> CreateCustomMenuactionObject;
        public event EventHandler MenuItemsCreated;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty)]


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string GetDesignTimeHtml()
        {
            string result = @"<table cellspacing=""0"" cellpadding=""0"" border=""0""><tr>";
            foreach (WebActionContainer container in ActionContainers)
            {
                WebActionContainerDesignerHelper helper = new WebActionContainerDesignerHelper(container);
                result += "<td>" + helper.GetDesignTimeHtml() + "</td>" + @"</tr><tr>";
            }
            result += "</tr></table>";
            return result;
        }        
    }
}
