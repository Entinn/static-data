using Entin.StaticData.Attributes;
using Entin.StaticData.CsvReader;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class FileReceiver<TSheet> : BaseFileReceiver where TSheet : BaseSheet
    {
        protected FileReceiver(string fileName) : base(fileName)
        {
        }

        public TSheet[] Receive(TextAsset text)
        {
            return Reader.Parse<TSheet>(text.text);
        }

        public sealed override void Validate(StaticData staticData)
        {
            ValidateAttributes(staticData);
            ValidateData(staticData);
        }

        protected abstract void ValidateData(StaticData staticData);

        private void ValidateAttributes(StaticData staticData)
        {
            AttributeValidation.Validate<TSheet>(staticData, AddError);
        }
    }
}