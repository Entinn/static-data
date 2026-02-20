using System;

namespace Entin.StaticData.CsvReader
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute { }
}