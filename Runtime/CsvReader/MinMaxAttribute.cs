using System;

namespace Entin.StaticData.CsvReader
{
    public abstract class MinMaxAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MinIntAttribute : MinMaxAttribute
    {
        public readonly int Min;

        public MinIntAttribute(int min)
        {
            Min = min;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MinFloatAttribute : MinMaxAttribute
    {
        public readonly float Min;

        public MinFloatAttribute(float min)
        {
            Min = min;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MaxIntAttribute : MinMaxAttribute
    {
        public readonly int Max;

        public MaxIntAttribute(int max)
        {
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MaxFloatAttribute : MinMaxAttribute
    {
        public readonly float Max;

        public MaxFloatAttribute(float max)
        {
            Max = max;
        }
    }
}