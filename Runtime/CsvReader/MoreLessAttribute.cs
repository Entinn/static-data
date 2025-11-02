using System;

namespace Entin.StaticData.CsvReader
{
    public abstract class MoreLessAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LessThanAttribute : MoreLessAttribute
    {
        public readonly string PropertyKey;
        public readonly bool OrEqual;

        public LessThanAttribute(string propertyKey, bool orEqual = false)
        {
            PropertyKey = propertyKey;
            OrEqual = orEqual;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MoreThanAttribute : MoreLessAttribute
    {
        public readonly string PropertyKey;
        public readonly bool OrEqual;

        public MoreThanAttribute(string propertyKey, bool orEqual = false)
        {
            PropertyKey = propertyKey;
            OrEqual = orEqual;
        }
    }
}