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
    [DefaultClassOptions, ObjectCaptionFormat("{0:Theme}"), DefaultProperty("Theme"), XafDisplayName("XAF Bootstrap Configuration"), ImageName("BO_List"), CreatableItem(false), NavigationItem(false)]
    public class XAFBootstrapConfiguration : BaseObject
    {
        public static XAFBootstrapConfiguration Instance
        {
            get
            {
                return XAF_Bootstrap.DatabaseUpdate.Updater.Configuration((WebApplication.Instance as XafApplication).CreateObjectSpace(typeof(XAFBootstrapConfiguration)));
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
