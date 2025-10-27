using Entin.StaticData.CsvReader;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class KeyValueFileReceiver<TSheet> : BaseFileReceiver where TSheet : KeyValueSheet
    {
        protected KeyValueFileReceiver(string fileName) : base(fileName)
        {
        }

        public TSheet Receive(TextAsset text)
        {
            return Reader.ParseKeyValue<TSheet>(text.text);
        }
    }
}