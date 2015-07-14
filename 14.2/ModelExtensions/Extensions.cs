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

using DevExpress.ExpressApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XAF_Bootstrap.ModelExtensions
{
    public enum XAFBootstrapMenuAlign
    {
        Left,
        Right
    }
    
    public interface IModelMenuAlign : IModelNode
    {
        [Category("XAF Bootstrap")]
        XAFBootstrapMenuAlign MenuAlign { get; set; }
    }

    public interface IModelGlyphicon : IModelNode
    {
        [Category("XAF Bootstrap")]
        String Glyphicon { get; set; }
    }
}
