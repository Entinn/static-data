using Entin.StaticData.CsvReader;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class FileReceiver<TSheet> : BaseFileReceiver<TSheet>
        where TSheet : BaseSheet
    {
        protected FileReceiver(string fileName) : base(fileName)
        {
        }

        public TSheet[] Receive(TextAsset text)
        {
            return Reader.Parse<TSheet>(text.text);
        }
    }
}