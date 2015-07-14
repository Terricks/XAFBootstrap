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
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using XAF_Bootstrap.BusinessObjects;
using System.IO;
using System.Reflection;
using System.Web;

namespace XAF_Bootstrap.DatabaseUpdate
{    
    public class Updater : ModuleUpdater
    {
        public static XAFBootstrapConfiguration Configuration(IObjectSpace ObjectSpace)
        {
            var configuration = ObjectSpace.FindObject<XAFBootstrapConfiguration>(null, true);
            if (configuration == null)
            {
                configuration = ObjectSpace.CreateObject<XAFBootstrapConfiguration>();
                configuration.Theme = "Paper";

                XAF_BootstrapModule.ApplyBootstrapCSS(configuration);
            }
            return configuration;
        }

        private static void CopyResource(string resourceName, string file, Boolean copyOnlyIfNotExists = true)
        {
            using (Stream resource = Assembly.GetExecutingAssembly() 
                .GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", resourceName);
                }
                var dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                
                if (!copyOnlyIfNotExists || (copyOnlyIfNotExists && !File.Exists(file)))
                    using (Stream output = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        resource.CopyTo(output);
            }
        }

        private static void CheckResource(string location, string resourceName, Boolean copyOnlyIfNotExists = true)
        {
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                var path = HttpContext.Current.Server.MapPath("/");
                CopyResource(String.Format("XAF_Bootstrap.Content.{0}.{1}", location, resourceName), path + location.Replace(".", "\\") + "\\" + resourceName, copyOnlyIfNotExists);
            }
        }

        public static void CheckResources()
        {
            CheckResource("bootstrap_js", "jquery-1.11.2.min.js");
            CheckResource("bootstrap_js", "bootstrap.min.js");                        
            CheckResource("bootstrap_js", "bootstrap-datetimepicker.min.js");
            CheckResource("bootstrap_js", "bootstrap-select.min.js");
            CheckResource("bootstrap_js", "moment-with-locales.min.js");

            CheckResource("bootstrap_css", "bootstrap.min.css");
            CheckResource("bootstrap_css", "bootstrap-datetimepicker.css", false);
            CheckResource("bootstrap_css", "bootstrap-select.min.css", false);
            CheckResource("bootstrap_css", "bootstrap-dx.css", false);
            CheckResource("bootstrap_css", "bootstrap-custom.css");

            CheckResource("fonts", "glyphicons-halflings-regular.eot");
            CheckResource("fonts", "glyphicons-halflings-regular.svg");
            CheckResource("fonts", "glyphicons-halflings-regular.ttf");
            CheckResource("fonts", "glyphicons-halflings-regular.woff");

            CheckResource("bootstrap_themes.Cerulean.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Cerulean", "preview.jpg");
            CheckResource("bootstrap_themes.Cosmo.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Cosmo", "preview.jpg");
            CheckResource("bootstrap_themes.Custom.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Custom", "preview.jpg");
            CheckResource("bootstrap_themes.Cyborg.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Cyborg", "preview.jpg");
            CheckResource("bootstrap_themes.Darkly.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Darkly", "preview.jpg");
            CheckResource("bootstrap_themes.Flatly.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Flatly", "preview.jpg");
            CheckResource("bootstrap_themes.Lumen.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Lumen", "preview.jpg");
            CheckResource("bootstrap_themes.Paper.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Paper.bootstrap_css", "bootstrap-custom.css", false);
            CheckResource("bootstrap_themes.Paper", "preview.jpg");
            CheckResource("bootstrap_themes.Readable.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Readable", "preview.jpg");
            CheckResource("bootstrap_themes.Simpex.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Simpex", "preview.jpg");
            CheckResource("bootstrap_themes.Slate.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Slate", "preview.jpg");
            CheckResource("bootstrap_themes.Spacelab.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Spacelab", "preview.jpg");
            CheckResource("bootstrap_themes.Sandstone.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Sandstone", "preview.jpg");
            CheckResource("bootstrap_themes.Sandstone.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Sandstone", "preview.jpg");
            CheckResource("bootstrap_themes.United.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.United", "preview.jpg");
            CheckResource("bootstrap_themes.Yeti.bootstrap_css", "bootstrap.min.css", false);
            CheckResource("bootstrap_themes.Yeti", "preview.jpg");
        }

        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
            Configuration(ObjectSpace);

            CheckResources();
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();            
        }
        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();            
        }
    }
}
