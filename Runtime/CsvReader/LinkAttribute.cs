using System;

namespace Entin.StaticData.CsvReader
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LinkAttribute : Attribute
    {
        public readonly Type Type;
        public readonly string PropertyKey;
        public readonly bool CanBeEmpty;

        public LinkAttribute(Type type, string propertyKey, bool canBeEmpty = false)
        {
            Type = type;
            PropertyKey = propertyKey;
            CanBeEmpty = canBeEmpty;
        }
    }
}