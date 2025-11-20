using Entin.StaticData.CsvReader;

namespace Entin.StaticData.Sheet
{
    public abstract class KeySheet<TKey> : BaseSheet
    {
        [Unique]
        public virtual TKey Key { get; }
    }
}