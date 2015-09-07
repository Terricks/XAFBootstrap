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

using DevExpress.Data.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XAF_Bootstrap.ModelExtensions
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class TagSourceAttribute : Attribute
    {        
        Type aType = null;
        CriteriaOperator aCriteria = null;
        String aKey = "Oid";
        String aValueFormat = "";
        String aImageName = "";

        String aValues = "";
        TagSourceKind aKind;

        String aSort = "";

        public TagSourceAttribute(Type type)
        {
            aType = type;            
            aKind = TagSourceKind.TypeSource;            
        }

        public TagSourceAttribute(Type type, String key = "Oid")
        {
            aType = type;            
            aKey = key;            
            aKind = TagSourceKind.TypeSource;
        }

        public TagSourceAttribute(Type type, String key = "Oid", String valueFormat = null)
        {
            aType = type;            
            aKey = key;
            aValueFormat = valueFormat;
            aKind = TagSourceKind.TypeSource;
        }

        public TagSourceAttribute(Type type, String key = "Oid", String valueFormat = "", String criteria = "", String imageName = "", String Sorting = "")
        {
            aType = type;
            if (String.Concat(criteria) != "")
                aCriteria = CriteriaOperator.Parse(criteria);
            aKey = key;
            aValueFormat = valueFormat;
            aImageName = imageName;
            aKind = TagSourceKind.TypeSource;
            aSort = Sorting;
        }

        public TagSourceAttribute(String commaValues, String imageName = "")
        {
            aValues = commaValues;
            aKind = TagSourceKind.Values;            
            aImageName = imageName;
        }

        public Type Type
        {
            get { return aType; }
        }

        public CriteriaOperator Criteria
        {
            get { return aCriteria; }
        }

        public String Key
        {
            get { return aKey; }
        }

        public String ImageName
        {
            get { return aImageName; }
        }


        public String ValueFormat
        {
            get { return aValueFormat; }
        }

        public String CommaValues
        {
            get { return aValues; }
        }

        public TagSourceKind Kind
        {
            get { return aKind; }
        }

        public String Sorting
        {
            get { return aSort; }
        } 
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TagSettingsAttribute : Attribute
    {
        public Char TextSeparator = ',';
        public Char ValueSeparator = ',';
        public TagSettingsAttribute(Char textSeparator, Char valueSeparator)
        {
            TextSeparator = textSeparator;
            ValueSeparator = valueSeparator;
        }
    }
}
