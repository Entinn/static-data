using Entin.StaticData.CsvReader;

namespace Entin.StaticData.Sheet
{
    public abstract class KeySheet<TKey> : BaseSheet
    {
        [Unique]
        public TKey Key { get; }
    }
}