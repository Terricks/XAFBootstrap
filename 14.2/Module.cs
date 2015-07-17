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
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.SystemModule;
using XAF_Bootstrap.ModelExtensions;
using DevExpress.ExpressApp.Web;
using System.IO;
using System.Reflection;
using XAF_Bootstrap.BusinessObjects;
using System.Web;

namespace XAF_Bootstrap
{    
    public sealed partial class XAF_BootstrapModule : ModuleBase
    {
        public XAF_BootstrapModule()
        {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }

        private static Boolean BundlesPrepared;
        public static void PrepareBundles()
        {
            if (!BundlesPrepared)
            {
                System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.
                    Optimization.ScriptBundle("~/bootstrap_js/js.aspx")
                    .Include("~/bootstrap_js/jquery-1.11.2.min.js")
                    .Include("~/bootstrap_js/bootstrap.min.js")
                    .Include("~/bootstrap_js/moment-with-locales.min.js")
                    .Include("~/bootstrap_js/bootstrap-datetimepicker.min.js")
                    .Include("~/bootstrap_js/bootstrap-select.min.js")
					.Include("~/bootstrap_js/bootstrap-dx.js")
                    );

                System.Web.Optimization.BundleTable.Bundles.Add(new System.Web.
                    Optimization.StyleBundle("~/bootstrap_css/css.aspx")
                    .Include("~/bootstrap_css/bootstrap.min.css")
                    .Include("~/bootstrap_css/bootstrap-datetimepicker.css")
                    .Include("~/bootstrap_css/bootstrap-select.min.css")
                    .Include("~/bootstrap_css/bootstrap-dx.css")
                    .Include("~/bootstrap_css/bootstrap-custom.css")
                    );
                BundlesPrepared = true;
            }
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);

            if (application is WebApplication) {
                if (System.Diagnostics.Debugger.IsAttached)
                    XAF_Bootstrap.DatabaseUpdate.Updater.CheckResources();

                PrepareBundles();                

                ProcessTemplateFile("LogonTemplate.ascx");                  
                (application as WebApplication).Settings.LogonTemplateContentPath =
                    "Templates/LogonTemplate.ascx";

                ProcessTemplateFile("ContentTemplate.ascx");
                (application as WebApplication).Settings.DefaultVerticalTemplateContentPath =
                    "Templates/ContentTemplate.ascx";

                ProcessTemplateFile("DialogTemplate.ascx");
                (application as WebApplication).Settings.DialogTemplateContentPath =
                    "Templates/DialogTemplate.ascx";

                ProcessTemplateFile("NestedFrameControl.ascx");
                (application as WebApplication).Settings.NestedFrameControlPath =
                    "Templates/NestedFrameControl.ascx";

                ProcessTemplateFile("XafBootstrapView.ascx");
            }            
        }

        public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        void DirectoryCopy(string sourceDir, string targetDir, string TemplateName)
        {
            Directory.CreateDirectory(targetDir);
            File.Copy(sourceDir + TemplateName, Path.Combine(targetDir, Path.GetFileName(sourceDir + TemplateName)), true);            
        }

        private void ProcessTemplateFile(String FileName)
        {
            try
            {
                string dir = AssemblyDirectory;

                IList<String> path = dir.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                path.RemoveAt(path.Count - 1);

                String filePathDest = String.Join("\\", path.ToArray()) + "\\Templates\\";
                String filePathSource = AssemblyDirectory + "\\Templates\\";

                if (!System.IO.File.Exists(filePathDest + FileName)
                    || (new FileInfo(filePathDest + FileName).LastWriteTimeUtc < new FileInfo(filePathSource + FileName).LastWriteTimeUtc)
                )
                    DirectoryCopy(filePathSource, filePathDest, FileName);
            }
            catch
            {
            }
            
        }

        public static void ApplyBootstrapCSS(XAFBootstrapConfiguration config)
        {
            if (config == null || HttpContext.Current == null || HttpContext.Current.Server == null)
                return;
            
            if (config != null && config.Theme != "Default")
            {
                var info = new DirectoryInfo(HttpContext.Current.Server.MapPath("bootstrap_themes/" + config.Theme));
                if (Directory.Exists(info.FullName))
                {
                    foreach (var sub in Directory.GetDirectories(info.FullName).Select(f => new DirectoryInfo(f)))
                    {
                        foreach (var file in Directory.GetFiles(sub.FullName).Select(f => new FileInfo(f)))
                        {
                            File.Copy(file.FullName, HttpContext.Current.Server.MapPath(sub.Name + "/" + file.Name), true);
                        }
                    }
                }
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            
            //Extenders
            extenders.Add<IModelNavigationItem, IModelMenuAlign>();
            extenders.Add<IModelNavigationItem, IModelGlyphicon>();
        }
    }   
}
