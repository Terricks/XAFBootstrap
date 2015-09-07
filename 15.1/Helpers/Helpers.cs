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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using XAF_Bootstrap.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.ExpressApp.SystemModule;
using System.Web.UI.HtmlControls;
using XAF_Bootstrap.ModelExtensions.Attributes;

namespace XAF_Bootstrap.Templates
{
    public delegate Control CreateFunc(Control parent, DevExpress.ExpressApp.View view, Control control = null);

    public class MenuAction
    {
        public XafMenuItem MenuItem;
        public ActionBase Action;
    }

    public class DynamicControl
    {
        public Control Control;
        public Control Parent;
        public String ID;
        public CreateFunc CreateFunc;
        public DevExpress.ExpressApp.View View;
    }

    public static class EditorHelper
    {
        public static String GetPlaceholder(PropertyEditor editor, String defaultValue = "")
        {
            String ret = defaultValue;
            if (editor != null && editor.MemberInfo != null)
            {
                var placeholder = editor.MemberInfo.FindAttribute<PlaceholderAttribute>();
                if (placeholder != null)
                    ret = placeholder.Placeholder;
            }
            return ret;
        }
    }

    public static class Helpers
    {
        public const String JoinString = "_ibn_";
        private static EditorsFactory _EditorsFactory;
        public static EditorsFactory EditorsFactory
        {
            get
            {
                if (_EditorsFactory == null)
                    _EditorsFactory = new EditorsFactory();
                return _EditorsFactory;
            }
        }

        public static System.Web.SessionState.HttpSessionState Session
        {
            get
            {
                return System.Web.HttpContext.Current.Session;
            }
        }

        public static ContentHelperClass ContentHelper
        {            
            get {
                var session = System.Web.HttpContext.Current.Session;
                if (session["ContentHelper"] == null)
                    session["ContentHelper"] = new ContentHelperClass();
                return (ContentHelperClass)session["ContentHelper"];
            }
        }
        public static IList<XafMenuItem> GetMenuActions(XbActionContainerHolder holder)
        {   
            return holder.actionObjects.Where(f => f.Key.Active && f.Key.Enabled).Select(f => f.Value.MenuItem).ToList();
        }

        public static IList<XafMenuItem> GetMenuActions(ASPxMenu menu) {
            IList<XafMenuItem> List = new List<XafMenuItem>();
            if (menu != null) {
                foreach(var item in menu.Items.OfType<XafMenuItem>()) {
                    if (item.ActionProcessor != null && item.ActionProcessor is DevExpress.ExpressApp.Templates.ActionBaseItem) {
                        var action = (item.ActionProcessor as DevExpress.ExpressApp.Templates.ActionBaseItem);
                        if (action.Action != null && action.IsVisible && action.Action.Enabled && action.Action.Active)
                            List.Add(item);
                    }
                }                
            }
            return List;
        }

        public static String GetFormattedActionString(XafMenuItem menuItem, String Callback, String style, String Type = "button", String defaultIcon = "glyphicon-star")
        {
            if (menuItem == null)
                return "";

            String ClickScript = Callback;
            if (menuItem.ActionProcessor != null && menuItem.ActionProcessor is ActionBaseItem)
            {
                var processor = menuItem.ActionProcessor as ActionBaseItem;
                if (processor.Action != null && String.Concat(processor.Action.ConfirmationMessage) != "")
                {
                    ClickScript = String.Format(@"var func = function(){{{1}}}; ShowXafMessage(""{2}"", ""{0}"", func, """", """");"
                        , String.Concat(processor.Action.ConfirmationMessage).Replace("\r\n", "<br>").Replace("\n","<br>")
                        , Callback
                        , XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "ConfirmAction")).Replace("\"","&quot;");
                }
            }
            var action = (menuItem.ActionProcessor as MenuActionItemBase).Action as ActionBase;
            SingleChoiceAction singleChoiceAction = action as SingleChoiceAction;
            if (singleChoiceAction != null)
                if (singleChoiceAction.Items.Count == 1 && singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode)
                    menuItem.Text = (action as SingleChoiceAction).Items[0].Caption;
                else
                    if (singleChoiceAction.Items.Count == 1 && singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsOperation)
                        menuItem.Text = action.Caption;
            String FormatString = "<button type='button' class='{3}' onclick='{2}'>{4}{0}</button>";
            if (Type == "a")
                FormatString = "<a href='javascript:;' role='button' class='{3}' onclick='{2}'>{4}{0}</a>";

            return String.Format(FormatString, menuItem.Text, menuItem.Name, ClickScript, style, (defaultIcon != "" ? String.Format(@"<span class='glyphicon {0}'></span> ", defaultIcon) : ""));
        }

        public static bool ProcessAction(ActionBase action)
        {
            if (action != null && action.Active && action.Enabled)
            {
                if (action is SimpleAction)
                {
                    var closePopup = false;
                    var dialogController = action.Controller.Frame.GetController<DialogController>();
                    if (dialogController != null) {
                        dialogController.ViewClosed += new EventHandler(delegate{
                            closePopup = true;
                        });       
                    }
                    if ((action as SimpleAction).DoExecute())
                    {                        
                        if (closePopup)
                            WebWindow.CurrentRequestWindow.RegisterStartupScript("actionClosePopup", "window.DataChanged=false; if(window.closeThisModal) window.closeThisModal();");
                    }
                    return true;
                }
                else if (action is SingleChoiceAction)
                {
                    var selectedItem = (action as SingleChoiceAction).SelectedItem;
                    if (selectedItem == null)
                        selectedItem = (action as SingleChoiceAction).Items.FirstActiveItem;
                    if (selectedItem != null)
                    {
                        (action as SingleChoiceAction).DoExecute(selectedItem);
                        return true;
                    }

                }
                else if (action is PopupWindowShowAction)
                {
                    var args = (action as PopupWindowShowAction).GetPopupWindowParams();

                    ShowViewParameters svp = new ShowViewParameters();
                    svp.CreatedView = args.View;             
                    svp.Controllers.Add(args.DialogController);

                    svp.Context = TemplateContext.PopupWindow;
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    svp.NewWindowTarget = NewWindowTarget.Separate;

                    (WebApplication.Instance as XafApplication).ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                }
            }
            return false;
        }        

        public static Boolean GenerateParametrizedAction(ref StringBuilder result, XafMenuItem menuItem, Boolean IsLeft, String style, String Callback, String ControlType = "button", String Click = "")
        {
            if (!(menuItem.ActionProcessor is MenuActionItemBase)                
                || !((menuItem.ActionProcessor as MenuActionItemBase).Action is ParametrizedAction))
                return false;
            var parAction = (menuItem.ActionProcessor as MenuActionItemBase).Action as ParametrizedAction;
            bool usePostBack = (parAction as ActionBase).Model.GetValue<bool>("IsPostBackRequired");
            result.AppendFormat(@"
                <div class='input-group parametrized-action'>                  
                    <input type='text' class='form-control input-sm' placeholder='{0}' onkeydown=""if (event.keyCode == 13) $(this).parent().find('button').click();"" value='{3}' style='min-width:75px'>
                    <span class='input-group-btn'>
                        <button class='btn btn-default btn-sm' type='button' onclick='{1}'>{2}</button>
                    </span>
                </div>"
                , parAction.NullValuePrompt
                , Helpers.ContentHelper.GetScript(Callback, String.Format("\"Action={0},\" + $(this).parent().parent().find('input').val()", menuItem.Name), "", usePostBack).Replace("'", "\"")
                , parAction.ShortCaption
                , parAction.Value
                , style);

            return true;
        }

        public static Boolean GenerateSingleChoiceAction(ref StringBuilder result, XafMenuItem menuItem, Boolean IsLeft, String style, String Callback, String ControlType = "button", String Click = "")
        {
            if (!(menuItem.ActionProcessor is MenuActionItemBase) 
                || !((menuItem.ActionProcessor as MenuActionItemBase).Action is SingleChoiceAction))
                return GenerateParametrizedAction(ref result, menuItem, IsLeft, style, Callback, ControlType, Click);

            var choiceAction = (menuItem.ActionProcessor as MenuActionItemBase).Action as SingleChoiceAction;            

            if (choiceAction.Items.Count <= 1)
                return false;
            bool usePostBack = (choiceAction as ActionBase).Model.GetValue<bool>("IsPostBackRequired");

            var dropdown = String.Format(@"<div class=""dropdown {4}"">
                <{3} href=""javascript:;"" class=""dropdown-toggle {0}"" type=""button"" id=""dropdownMenu1"" data-toggle=""dropdown"" aria-expanded=""true"">
                    {2}
                    <span class=""caret""></span>
                </{3}>
                <ul class=""dropdown-menu"" role=""menu"" aria-labelledby=""dropdownMenu1"">
                    {1}                    
                </ul>
                </div>"
                , style
                , String.Join("", choiceAction.Items.Select(f =>
                    String.Format(@"<li role=""presentation""><a role=""menuitem"" tabindex=""-1"" href=""javascript:;"" onclick='{2}; {1}'>{0}</a></li>"
                        , f.Caption
                        , Helpers.ContentHelper.GetScript(Callback, String.Format("\"Action={0},{1}\"", menuItem.Name, f.Id), "", usePostBack).Replace("'", "\"")
                        , Click)
                ))
                , choiceAction.SelectedItem != null && choiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode ? choiceAction.SelectedItem.Caption : choiceAction.Caption
                , ControlType
                , IsLeft ? "pull-left" : "pull-right"
            );
            result.Append(dropdown);
            return true;
        }

        public static String BuildActionsMenu(XbActionContainerHolder actionHolder, String callbackName, Boolean IsLeft = true, String style = "btn btn-primary btn-sm", String type = "button", String ClickScript = "", String ClassName = "actions", String Glyphicon = "glyphicon-star")
        {
            StringBuilder result = new StringBuilder();

            if (!IsLeft)
                style += " pull-right";
            else
                style += " pull-left";

            var actions = actionHolder.actionObjects.Where(f => f.Key.Active && f.Value.IsVisible && f.Value.Action.Active && f.Value.Action.Enabled).Select(f => f.Value.MenuItem).OrderBy(f => f.VisibleIndex).ToList();
            if (actions.Count > 0)
            {                   
                result.AppendFormat("<div class='{0}'>", ClassName);

                Helpers.ContentHelper.ObjectActions = new List<MenuAction>();                

                /// Reversing for pull-right correct visibility
                if (!IsLeft)
                    actions.Reverse();

                foreach (XafMenuItem menuItem in actions)
                {
                    var action = (menuItem.ActionProcessor as MenuActionItemBase).Action as ActionBase;
                    bool usePostBack = action.Model.GetValue<bool>("IsPostBackRequired");
                    if (!(Helpers.GenerateSingleChoiceAction(ref result, menuItem, IsLeft, style, callbackName, type, ClickScript)))
                        result.Append(Helpers.GetFormattedActionString(menuItem, ClickScript + ";" + Helpers.ContentHelper.GetScript(callbackName, String.Format("\"Action={0}\"", menuItem.Name), "", usePostBack).Replace("'", "\""), style, type, Glyphicon));
                }
                result.Append("</div>");
            }

            return result.ToString();
        }

        public static String BuildActionsMenu(ActionContainerHolder actions, String callbackName, Boolean IsLeft = true, String style = "btn btn-primary btn-sm", String type = "button", String ClickScript = "", String ClassName = "actions", String Glyphicon = "glyphicon-star")
        {
            StringBuilder result = new StringBuilder();

            if (!IsLeft)
                style += " pull-right";

            if (actions.Menu.Items.Count > 0)
            {
                result.AppendFormat("<div class='{0}'>", ClassName);

                Helpers.ContentHelper.ObjectActions = new List<MenuAction>();
                var menuItems = Helpers.GetMenuActions(actions.Menu);

                /// Reversing for pull-right correct visibility
                if (!IsLeft)
                    menuItems = menuItems.Reverse().ToList();
                
                foreach (XafMenuItem menuItem in menuItems)
                {
                    var action = (menuItem.ActionProcessor as MenuActionItemBase).Action as ActionBase;
                    bool usePostBack = action.Model.GetValue<bool>("IsPostBackRequired");
                    if (!(Helpers.GenerateSingleChoiceAction(ref result, menuItem, IsLeft, style, callbackName, type, ClickScript)))
                        result.Append(Helpers.GetFormattedActionString(menuItem, ClickScript + ";" + Helpers.ContentHelper.GetScript(callbackName, String.Format("\"Action={0}\"", menuItem.Name), menuItem, usePostBack).Replace("'", "\""), style, type, Glyphicon));
                }
                result.Append("</div>");
            }

            return result.ToString();
        }

        public static void ProcessMenuAction(String parameter, Frame frame)
        {
            var param = String.Concat(parameter);
            var data = "";

            if (param.Length > 7 && param.Substring(0, 7) == "Action=")
            {
                param = param.Substring(7, param.Length - 7);                
                if (param.IndexOf(",") > -1)
                {
                    data = param.Substring(param.IndexOf(",") + 1, param.Length - param.IndexOf(",") - 1);
                    param = param.Substring(0, param.IndexOf(","));
                }   
                    
                foreach (var controller in frame.Controllers.Where<Controller>(f => f.Actions.Where(a => a.Id == param).Count() > 0))
                {   
                    ActionBase action = (ActionBase)controller.Actions.Where(a => a.Id == param).FirstOrDefault();
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
                    {
                        Helpers.ProcessAction(action);                            
                    }                    
                }
            }
        }

        public static DefaultHttpRequestManager RequestManager
        {        
            get {
                return ((DefaultHttpRequestManager)(WebApplication.Instance.RequestManager));
            }
        }

        public static String GetXafDisplayName(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(XafDisplayNameAttribute), false);

                if (attributes.Count() > 0 && attributes[0] is XafDisplayNameAttribute)
                    return ((XafDisplayNameAttribute)attributes[0]).DisplayName;
            }
            return String.Concat(enumVal);
        }

        public static String GetXafDisplayName(this Type type)
        {
            foreach (XafDisplayNameAttribute item in type.GetCustomAttributes(typeof(XafDisplayNameAttribute), true))
                return item.DisplayName;
            return "";
        }

        public static String GetXafImageName(this Enum enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());

            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(ImageNameAttribute), false);

                if (attributes.OfType<ImageNameAttribute>().Count() > 0)
                    return ((ImageNameAttribute)attributes[0]).ImageName;
            }
            return "";
        }    

        public static String GetImmediatePostDataScript(IMemberInfo memberInfo)
        {
            ImmediatePostDataAttribute immediatePostDataAttribute = memberInfo.FindAttribute<ImmediatePostDataAttribute>();
            if (immediatePostDataAttribute != null)
                if (immediatePostDataAttribute.Value != null)
                    return "RaiseXafCallback(globalCallbackControl, '', 'XafParentWindowRefresh', '', false);";
            return "";
        }

        public static void AddMeta(Page page) {
            var meta = page.Header.Controls.OfType<HtmlMeta>().Where(f => f.Name == "viewport").FirstOrDefault();
            if (meta == null)
            {
                meta = new HtmlMeta();
                page.Header.Controls.Add(meta);                
            }
            meta.Name = "viewport";
            meta.Content = "width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no";

            if (page.Header.Controls.OfType<HTMLText>().Count() == 0)
            {
                page.Header.Controls.Add(new HTMLText(System.Web.Optimization.Styles.Render("~/bootstrap_css/css.aspx").ToString()));
                page.Header.Controls.Add(new HTMLText(System.Web.Optimization.Scripts.Render("~/bootstrap_js/js.aspx").ToString()));
            }
        }

        public static String GetLocalizedText(string groupPath, string itemName)
        {
            return String.Concat(CaptionHelper.GetLocalizedText(groupPath, itemName)).Replace("\r\n","<br>").Replace("\n", "<br>");
        }
    }

    public static class ObjectFormatValues
    {
        private const string uniqStrOBrace = "BBBD1F2C-3E69-44D7-96B6-A303A423EDFE";
        private const string uniqStrEBrace = "4759C22A-02A2-419C-9DDA-997BF192F38E";

        private static object nullObject = new object();

        public static object GetValueRecursive(string format, object arg, out IMemberInfo memberInfo)
        {
            memberInfo = null;
            object propertyValue = nullObject;
            object probableParameterValue = CriteriaWrapper.TryGetReadOnlyParameterValue(format);
            if (!(probableParameterValue is string) || (format != (string)probableParameterValue))
            {
                propertyValue = probableParameterValue;
            }
            else
            {
                object valueByPath;
                if (TryGetValueByPathDynamic(format, arg, out valueByPath, out memberInfo))
                {
                    propertyValue = valueByPath;
                }
            }
            if (propertyValue is string && format != (string)propertyValue)
            {
                if (((string)propertyValue).StartsWith(CriteriaWrapper.ParameterPrefix))
                {
                    object value = GetValueRecursive((string)propertyValue, arg, out memberInfo);
                    if (value != null)
                    {
                        propertyValue = value;
                    }
                }
                else
                {
                    if (((string)propertyValue).Contains("{@"))
                    {
                        propertyValue = Format((string)propertyValue, arg);
                    }
                }
            }
            return propertyValue;
        }

        public static bool TryGetValueByPathDynamic(string prop, object source, out object result, out IMemberInfo itemInfo)
        {
            Guard.ArgumentNotNull(source, "from");
            Guard.ArgumentNotNull(prop, "propertyPath");
            result = null;
            string[] items = prop.Split('.');
            object currObj = source;
            itemInfo = null;
            foreach (string item in items)
            {
                if (currObj == null)
                {
                    return true;
                }
                ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(currObj.GetType());
                IMemberInfo curritem = objType.FindMember(item);
                if (curritem == null)
                {
                    return false;
                }
                currObj = curritem.GetValue(currObj);
                itemInfo = curritem;
            }
            result = currObj;
            return true;
        }

        public static string Format(string format, object obj)
        {
            return Format(format, obj, EmptyEntriesMode.Default);
        }
        public static string Format(string format, object obj, EmptyEntriesMode mode)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }
            try
            {
                format = format.Replace("{{", uniqStrOBrace).Replace("}}", uniqStrEBrace);
                char[] chArray = format.ToCharArray(0, format.Length);
                int length = chArray.Length;
                StringBuilder pieceBuilder = new StringBuilder();
                List<string> pieces = new List<string>();
                bool isBrStart = false;
                for (int i = 0; i < length; i++)
                {
                    char ch = chArray[i];
                    if (ch == '{')
                    {
                        if (isBrStart)
                        {
                            throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
                        }
                        isBrStart = true;
                        if (pieceBuilder.Length > 0)
                        {
                            pieces.Add(pieceBuilder.ToString());
                        }
                        pieceBuilder = new StringBuilder();
                        pieceBuilder.Append(ch);
                    }
                    else if (ch == '}')
                    {
                        if (!isBrStart)
                        {
                            throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
                        }
                        if (pieceBuilder.Length <= 1)
                        {
                            throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
                        }
                        isBrStart = false;
                        pieceBuilder.Append(ch);
                        pieces.Add(pieceBuilder.ToString());
                        pieceBuilder = new StringBuilder();
                    }
                    else
                    {
                        pieceBuilder.Append(ch);
                    }
                }
                if (pieceBuilder.Length > 0)
                {
                    pieces.Add(pieceBuilder.ToString());
                }
                if (isBrStart)
                {
                    throw new FormatException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ObjectFormatterFormatStringIsInvalid, format));
                }
                StringBuilder result = new StringBuilder();
                string lastDelimiter = null;
                bool isLastFormattedpieceEmpty = false;
                bool hasNonEmptyFormattedpiece = false;
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i].StartsWith("{"))
                    {
                        string piece = pieces[i];
                        if (!piece.StartsWith("{0:"))
                        {
                            piece = "{0:" + piece.Substring(1, piece.Length - 1);
                        }
                        string formattedpiece = string.Format(new ObjectFormatter(), piece, obj);
                        if (!string.IsNullOrEmpty(formattedpiece))
                        {
                            if (!string.IsNullOrEmpty(lastDelimiter) &&
                                ((mode == EmptyEntriesMode.Default) || hasNonEmptyFormattedpiece || !isLastFormattedpieceEmpty))
                            {
                                result.Append(lastDelimiter);
                            }
                            hasNonEmptyFormattedpiece = true;
                            result.Append(formattedpiece);
                        }
                        else
                        {
                            isLastFormattedpieceEmpty = true;
                            if (!string.IsNullOrEmpty(lastDelimiter) && (mode == EmptyEntriesMode.Default))
                            {
                                result.Append(lastDelimiter);
                            }
                        }
                        lastDelimiter = null;
                    }
                    else
                    {
                        lastDelimiter = pieces[i];
                    }
                }
                if (!string.IsNullOrEmpty(lastDelimiter) && ((mode == EmptyEntriesMode.Default) || !isLastFormattedpieceEmpty))
                {
                    result.Append(lastDelimiter);
                }
                return result.ToString().Replace(uniqStrOBrace, "{").Replace(uniqStrEBrace, "}");
            }
            catch (Exception e)
            {
                Tracing.Tracer.LogValue("format", format);
                if (e is FormatException)
                {
                    Tracing.Tracer.LogValue("obj", e.Message);
                }
                Tracing.Tracer.LogValue("mode", mode);
                throw;
            }
        }
    }

    public class ContentHelperClass
    {       
        #region Menu        
        public IDictionary<Guid, ChoiceActionItem> MenuItems
        {
            get
            {
                if (WebWindow.CurrentRequestPage != null && WebWindow.CurrentRequestPage.Session != null)
                {
                    if (WebWindow.CurrentRequestPage.Session["XafBootstrapMenuItems"] == null)
                        WebWindow.CurrentRequestPage.Session["XafBootstrapMenuItems"] = new Dictionary<Guid, ChoiceActionItem>();
                    return WebWindow.CurrentRequestPage.Session["XafBootstrapMenuItems"] as IDictionary<Guid, ChoiceActionItem>;
                }                
                return new Dictionary<Guid, ChoiceActionItem>();
            }
        }

        public void ClearMenu()
        {
            if (MenuItems != null)
                MenuItems.Clear();
        }
        #endregion

        public XafCallbackManager Manager;
        public IList<MenuAction> ObjectActions;
        public XafMenuItem SelectedObjetItem;

        public HtmlTextWriter GetControlWriter(Control control)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter writer = new HtmlTextWriter(sw);

            control.RenderControl(writer);
            return writer;
        }

        public string RenderControl(Control control)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            HtmlTextWriter writer = new HtmlTextWriter(sw);

            control.RenderControl(writer);
            return sb.ToString();
        }

        public string GetScript(String callbackName, String parameter, XafMenuItem menuItem, Boolean usePostBack)
        {            
            if (Manager == null)
                Manager = new XafCallbackManager();
            return Manager.GetScript(callbackName, String.Format("\"Action={0}\"", menuItem.Name), "", usePostBack).Replace("'", "\"");
        }

        public string GetScript(String callbackName, String parameter, String confirmation, bool usePostBack)
        {            
            if (Manager == null)
                Manager = new XafCallbackManager();
            return Manager.GetScript(callbackName, parameter, confirmation, usePostBack);
        }

        public string GetScript(String callbackName, String parameter, String confirmation)
        {
            if (Manager == null)
                Manager = new XafCallbackManager();
            return Manager.GetScript(callbackName, parameter, confirmation);
        }

        public string GetScript(String callbackName, String parameter)
        {
            if (Manager == null)
                Manager = new XafCallbackManager();
            return Manager.GetScript(callbackName, parameter);
        }

        public string GetCallbackScript(String ClientID, String CallbackString)
        {
            return String.Format("{0}.PerformCallback({1});", ClientID, CallbackString, Helpers.JoinString);
        }

        public IDictionary<String, IDictionary<String, object>> DynamicControlStates = new Dictionary<String, IDictionary<String, object>>();
    }    
}
