using Entin.StaticData.CsvReader;

namespace Entin.StaticData.Sheet
{
    public abstract class KeySheet<TKey> : IBaseSheet
    {
        [Unique]
        public virtual TKey Key { get; }
    }
}