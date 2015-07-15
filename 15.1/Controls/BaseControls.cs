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
using XAF_Bootstrap.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web;

namespace XAF_Bootstrap.Controls
{
    public class HTMLText : WebControl
    {
        public HTMLText()
        {
        }
        public HTMLText(String text)
        {
            Text = text;
        }
        public string Text;
        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(String.Concat(Text));
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class SaveState : Attribute
    {
        private readonly Boolean propertyValue;
        public SaveState(Boolean Save = true)
        {
            this.propertyValue = Save;
        }
        public Boolean PropertyValue
        {
            get { return propertyValue; }
        }
    }

    public class XafBootstrapBaseControl : ASPxCallbackPanel
    {
        public String PropertyName;

        public ContentHelperClass ContentHelper
        {
            get
            {
                return Helpers.ContentHelper;
            }
        }
      
        public String CallbackClientID {
            get {                
                return ClientID;                
            }
        }

        public String GetCallbackScript(String CallbackString)
        {
            return ContentHelper.GetCallbackScript(ClientID, String.Format("{0}", CallbackString));
        }

        public static void DetermineCallbackParams(String param, ref String Property, ref String result)
        {
            if (String.Concat(param).IndexOf(Helpers.JoinString) > -1)
            {
                string[] splParam = param.Split(new string[] { Helpers.JoinString }, StringSplitOptions.RemoveEmptyEntries);
                if (splParam.Length > 1)
                {
                    Property = splParam[splParam.Length - 2];
                    result = splParam[splParam.Length - 1];
                }
            }
        }

        public void ProcessCallback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            String param = String.Concat(e.Parameter);
            String propertyName = PropertyName;            
            DetermineCallbackParams(e.Parameter, ref propertyName, ref param);                
            
            if (PropertyName == propertyName)            
                OnCallback(new CallbackEventArgsBase(param));
        }

        protected override void OnCallback(CallbackEventArgsBase e)
        {
            base.OnCallback(e);
        }

        private String UniqName
        {
            get {
                return (String.Concat(PropertyName) != "" ? String.Format("{0}_{1}", CallbackClientID, PropertyName) : ClientID);
            }
        }

        public void InnerSaveControlState()
        {            
            IDictionary<String, object> values = new Dictionary<String, object>();
            if (ContentHelper.DynamicControlStates.Where(f => f.Key == ClientID).Count() > 0)
                values = ContentHelper.DynamicControlStates[UniqName];

            foreach (var member in this.GetType().GetProperties())
            {
                
                SaveState attr = (SaveState)member.GetCustomAttributes(typeof(SaveState), true).FirstOrDefault();
                if (attr != null && attr.PropertyValue)
                    values[member.Name] = this.GetType().GetProperty(member.Name).GetValue(this, null);
            }

            if (values.Count > 0 && UniqName != null)
                ContentHelper.DynamicControlStates[UniqName] = values;
        }

        public void InnerLoadControlState()
        {
            IDictionary<String, object> values = null;
            if (ContentHelper.DynamicControlStates.Where(f => f.Key == UniqName).Count() > 0)
                values = ContentHelper.DynamicControlStates[UniqName];

            if (values != null)
            {                
                foreach (var member in this.GetType().GetProperties())
                {
                    String uniqName = (String.Concat(PropertyName) != "" ? String.Format("{0}_{1}", CallbackClientID, PropertyName) : ClientID);
                    SaveState attr = (SaveState)member.GetCustomAttributes(typeof(SaveState), true).FirstOrDefault();
                    if (attr != null && attr.PropertyValue && values.Where(f => f.Key == member.Name).Count() > 0)
                        this.GetType().GetProperty(member.Name).SetValue(this, values[member.Name]);
                }
            }
        }

        protected override void BeforeRender()
        {
            base.BeforeRender();
            InnerLoadControlState();
        }

        protected override void AfterRender()
        {
            base.AfterRender();
            InnerSaveControlState();
        }
    }

    public interface IXafBootstrapListEditor
    {
    
    }

    public interface IXafBootstrapEditor
    {

    }

    public class CallbackHandler : IXafCallbackHandler, IProcessCallbackComplete
    {
        private String CallbackID = "";

        public CallbackHandler()
        {
        }

        public CallbackHandler(String ID)
        {
            Register(ID);
        }

        public CallbackHandler(String ID, object Data)
        {   
            Register(ID);
            this.Data = Data;
        }

        public void Register(String ID)
        {
            XafCallbackManager.RegisterHandler(ID, this);
            CallbackID = ID;
        }

        public String GetScript(String parameter, String stringConfirmation = "", bool usePostBack = false)
        {
            return XafCallbackManager.GetScript(CallbackID, parameter, stringConfirmation, usePostBack);
        }

        public event CallbackEventHandler OnCallback;
        public event CallbackEventHandler OnCallbackComplete;

        #region IXafCallbackHandler
        public static XafCallbackManager XafCallbackManager
        {
            get
            {
                return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            }
        }

        private String param;
        public void ProcessAction(string parameter)
        {
            param = parameter;
            if (OnCallback != null)
                OnCallback(this, new CallbackEventArgs(parameter));
        }

        void IProcessCallbackComplete.ProcessCallbackComplete()
        {
            if (OnCallbackComplete != null)
                OnCallbackComplete(this, new CallbackEventArgs(param));
        }

        #endregion

        public object Data;
    } 
}
