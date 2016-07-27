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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using XAF_Bootstrap.Controls;
using XAF_Bootstrap.Editors.XafBootstrapTableEditor;

namespace XAF_Bootstrap.Editors.XafBootstrapTableEditor
{
    [ListEditor(typeof(ITreeNode), false)]
    public class XafBootstrapStructureListEditor : XafBootstrapTableEditor, IComplexListEditor, IProcessCallbackComplete, IXafBootstrapListEditor
    {
        public XafBootstrapStructureListEditor(IModelListView info) : base(info)
        {
        }

        XafBootstrapStructureView structure;
        XafBootstrapTable table;       

        protected override object CreateControlsCore()
        {
            structure = new XafBootstrapStructureView();
            structure.Editor = this;
            structure.Collection = collection;
            structure.ObjectSpace = ObjectSpace;
            structure.EditMode = EditMode;            
            ShowSelector = false;
            
            return structure;
        }

        public override void Refresh()
        {
            if (control != null)
            {
                structure.Collection = collection;
                structure.Refresh();
            }
            CalcSelectedObjects();
            OnSelectionChanged();
        }

        public void InvokeSelectionChanged()
        {
            SelectedObjects.Clear();
            if (structure.Selected != null)
                SelectedObjects.Add(structure.Selected);
            OnSelectionChanged();
        }

        public override void DoProcessPairAction(string Action, string Param)
        {
            base.DoProcessPairAction(Action, Param);
        }
    }
}
