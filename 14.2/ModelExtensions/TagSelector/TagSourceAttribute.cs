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
