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
                try
                {
                    if (!copyOnlyIfNotExists || (copyOnlyIfNotExists && !File.Exists(file)))
                        using (Stream output = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
                            resource.CopyTo(output);
                }
                catch (Exception exc)
                {
                }
            }
        }

        public static void CheckResource(string location, string resourceName, Boolean copyOnlyIfNotExists = true, String Path = "XAF_Bootstrap.Content")
        {
            CopyResource(String.Format("{2}.{0}.{1}", location, resourceName, Path), AssemblyDirectory + location.Replace(".", "\\") + "\\" + resourceName, copyOnlyIfNotExists);
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                var path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path)).Split(new String[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                return String.Join("\\", path.Take(path.Length - 1)) + "\\";
            }
        }

        public static void CheckResources()
        {
            Boolean versionIsNewer = false;
            using (var resource = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("XAF_Bootstrap.Content.xaf_bootstrap_version.txt"))
            {
                using (var reader = new StreamReader(resource))
                {
                    var version = int.Parse(reader.ReadToEnd());
                    if (!File.Exists(AssemblyDirectory + "xaf_bootstrap_version.txt"))
                        versionIsNewer = true;
                    else
                    {
                        using (var fs = new FileStream(AssemblyDirectory + "xaf_bootstrap_version.txt", FileMode.Open))
                        {
                            using (var data = new StreamReader(fs))
                            {
                                var current = 0;
                                if (int.TryParse(data.ReadToEnd(), out current))
                                    versionIsNewer = version > current;
                            }
                        }                        
                    }
                    using (Stream fs = new FileStream(AssemblyDirectory + "xaf_bootstrap_version.txt", FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (var res = Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("XAF_Bootstrap.Content.xaf_bootstrap_version.txt"))
                        {
                            res.CopyTo(fs);
                        }                        
                    }
                }
            }

            if (versionIsNewer)
            {
                CheckResource("bootstrap_js", "jquery-1.11.2.min.js");
                CheckResource("bootstrap_js", "bootstrap.min.js", false);
                CheckResource("bootstrap_js", "bootstrap-datetimepicker.min.js");
                CheckResource("bootstrap_js", "bootstrap-select.min.js");
                CheckResource("bootstrap_js", "moment-with-locales.min.js");
                CheckResource("bootstrap_js", "bootstrap-dx.js", false);

                CheckResource("bootstrap_css", "bootstrap.min.css");
                CheckResource("bootstrap_css", "bootstrap-datetimepicker.css", false);
                CheckResource("bootstrap_css", "bootstrap-select.min.css", false);
                CheckResource("bootstrap_css", "bootstrap-dx.css", false);
                CheckResource("bootstrap_css", "bootstrap-custom.css");

                CheckResource("fonts", "glyphicons-halflings-regular.eot", false);
                CheckResource("fonts", "glyphicons-halflings-regular.svg", false);
                CheckResource("fonts", "glyphicons-halflings-regular.ttf", false);
                CheckResource("fonts", "glyphicons-halflings-regular.woff", false);
                CheckResource("fonts", "glyphicons-halflings-regular.woff2", false);

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
        }

        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
            if (ObjectSpace.CanInstantiate(typeof(XAFBootstrapConfiguration)))
            {
                Configuration(ObjectSpace);
            }
            
            if (HttpContext.Current == null || HttpContext.Current.Server == null)
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
