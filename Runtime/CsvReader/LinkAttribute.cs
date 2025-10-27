using System;

namespace Entin.StaticData.CsvReader
{
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class LinkAttribute : Attribute
    {
        public readonly Type Type;
        public readonly string FiledKey;
        public readonly bool CanBeEmpty;

        public LinkAttribute(Type type, string filedKey, bool canBeEmpty = false)
        {
            Type = type;
            FiledKey = filedKey;
            CanBeEmpty = canBeEmpty;
        }
    }
}