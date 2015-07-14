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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Model;

namespace XafBootstrap.Web
{    
    public partial class MenuItemClickController : ViewController, IXafCallbackHandler
    {
        public MenuItemClickController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public static XafCallbackManager XafCallbackManager
        {
            get
            {
                return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            }
        }

        public IModelNode FindNode(String param)
        {
            IModelNode result = Application.Model.GetNode("NavigationItems");
            try
            {
                foreach (var item in param.Split(new String[] { "->" }, StringSplitOptions.RemoveEmptyEntries))
                    result = result.GetNode("Items").GetNode(item);
            }
            catch
            {
            }

            return result;
        }

        public void ProcessAction(String parameter)
        {
            IModelNode Node = FindNode(parameter);
            if (Node != null && Node is IModelNavigationItem)
            {
                IModelView view = Node.GetValue<IModelView>("View");

                XafApplication App = Application;

                View ResultView = null;
                if (view is IModelListView)
                {
                    IModelListView ModelListView = (view as IModelListView);
                    Type type = (ModelListView.ModelClass.TypeInfo).Type;
                    CollectionSource cs = new CollectionSource(App.CreateObjectSpace(), type);
                    if (ModelListView.Criteria != null)
                        cs.Criteria["Criteria"] = DevExpress.Data.Filtering.CriteriaOperator.Parse(ModelListView.Criteria);
                    ResultView = App.CreateListView(ModelListView, cs, true);
                }
                else if (view is IModelDetailView)
                {
                    IModelDetailView DetailView = (view as IModelDetailView);
                    Type type = (DetailView.ModelClass.TypeInfo).Type;
                    IObjectSpace os = App.CreateObjectSpace();
                    CollectionSource cs = new CollectionSource(os, type);
                    object obj = null;
                    if (cs.List.Count > 0)
                        obj = cs.List[0];                    
                    ResultView = App.CreateDetailView(os, os.GetObject(obj), true);
                }

                if (ResultView != null)
                {
                    ShowViewParameters svp = new ShowViewParameters();
                    svp.CreatedView = ResultView;
                    
                    svp.Context = TemplateContext.ApplicationWindow;
                    svp.TargetWindow = TargetWindow.Current;
                    svp.NewWindowTarget = NewWindowTarget.Separate;

                    App.ShowViewStrategy.ShowView(svp, new ShowViewSource(Application.MainWindow, null));
                }
            }
        }
    }
}
