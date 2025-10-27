using Entin.StaticData.CsvReader;

namespace Entin.StaticData.Sheet
{
    public abstract class BaseSheet
    {
        internal BaseSheet() { }
    }

    public abstract class KeyValueSheet { }

    public abstract class KeySheet<TKey> : BaseSheet
    {
        [Unique]
        public TKey Key { get; }
    }

    public class GlobalVariable2 : KeyValueSheet
    {
        public string Some { get; }
    }

    public class Sailor : KeySheet<string>
    {
        public float Strength { get; }
    }
}