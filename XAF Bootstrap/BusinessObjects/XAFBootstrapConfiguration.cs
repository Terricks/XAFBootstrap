#region Copyright (c) 2014-2016 DevCloud Solutions
/*
{********************************************************************************}
{                                                                                }
{   Copyright (c) 2014-2016 DevCloud Solutions                                   }
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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XAF_Bootstrap.BusinessObjects
{
    public enum MenuType
    {
        [XafDisplayName("Default")]
        Default,
        [XafDisplayName("xbMegaMenu")]
        xbMegaMenu,
        [XafDisplayName("Bootstrap NavBar")]
        Bootstrap,
    }

    [DefaultClassOptions, ObjectCaptionFormat("{0:Theme}"), DefaultProperty("Theme"), XafDisplayName("XAF Bootstrap Configuration"), ImageName("BO_List"), CreatableItem(false), NavigationItem(false)]
    public class XAFBootstrapConfiguration : BaseObject
    {
        public static XAFBootstrapConfiguration _Instance;
        public static XAFBootstrapConfiguration Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = XAF_Bootstrap.DatabaseUpdate.Updater.Configuration((WebApplication.Instance as XafApplication).CreateObjectSpace(typeof(XAFBootstrapConfiguration)));
                return _Instance;
            }
        }

        public XAFBootstrapConfiguration(Session session) : base(session) { }

        private Boolean ThemeChanged;

        public void SetThemeChanged()
        {
            ThemeChanged = true;
        }
                
        private String _Theme;
        [Size(255), EditorAlias("XafBootstrapThemeConfigurationEditor")]
        public String Theme
        {
            get { return _Theme; }
            set { SetPropertyValue("Theme", ref _Theme, value); }
        }
                
        private Boolean _InverseNavBar;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Inverse navbar")]
        public Boolean InverseNavBar
        {
            get { return _InverseNavBar; }
            set { SetPropertyValue("InverseNavBar", ref _InverseNavBar, value); }
        }

        // LogonPageBackgroundUrl //
        private String _LogonPageBackgroundUrl;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Logon page background url"), Size(1000)]
        public String LogonPageBackgroundUrl
        {
            get { return _LogonPageBackgroundUrl; }
            set { SetPropertyValue("LogonPageBackgroundUrl", ref _LogonPageBackgroundUrl, value); }
        }

        // LogoImageUrl //
        private String _LogoImageUrl;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Logo image url"), Size(1000)]
        public String LogoImageUrl
        {
            get { return _LogoImageUrl; }
            set { SetPropertyValue("LogoImageUrl", ref _LogoImageUrl, value); }
        }

        // Menu //
        private MenuType _Menu;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Menu type")]
        public MenuType Menu
        {
            get { return _Menu; }
            set { SetPropertyValue("Menu", ref _Menu, value); }
        }

        // MenuBackgroundColor //
        private String _MenuBackgroundColor;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Menu background color")]
        public String MenuBackgroundColor
        {
            get { return _MenuBackgroundColor; }
            set { SetPropertyValue("MenuBackgroundColor", ref _MenuBackgroundColor, value); }
        }

        // MenuTextColor //
        private String _MenuTextColor;
        [DevExpress.ExpressApp.DC.XafDisplayNameAttribute("Menu text color")]
        public String MenuTextColor
        {
            get { return _MenuTextColor; }
            set { SetPropertyValue("MenuTextColor", ref _MenuTextColor, value); }
        }

        public String GetMenuBackgroundColor()
        {
            return String.Concat(MenuBackgroundColor) == "" ? "#0a5c96" : MenuBackgroundColor;
        }

        public String GetMenuTextColor()
        {
            return String.Concat(MenuTextColor) == "" ? "#d9f1fd" : MenuTextColor;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading)
                switch (propertyName)
                {
                    case "Theme":
                        ThemeChanged = true;
                        break;
                }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (ThemeChanged)
                XAF_BootstrapModule.ApplyBootstrapCSS(this);
        }
    }
}
