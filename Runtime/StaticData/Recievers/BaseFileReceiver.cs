using Entin.StaticData.Attributes;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class BaseFileReceiver<TSheet> : IDataReceiver
        where TSheet : BaseSheet
    {
        public string FileName { get; }
        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }

        protected internal BaseFileReceiver(string fileName)
        {
            FileName = fileName;
        }

        public void Validate(StaticData staticData)
        {
            ValidateAttributes(staticData);
            ValidateData(staticData);
        }

        protected abstract void ValidateData(StaticData staticData);

        private void ValidateAttributes(StaticData staticData)
        {
            AttributeValidation.Validate<TSheet>(staticData, AddError);
        }

        protected void AddError(string text)
        {
            HasError = true;
            ErrorText += $"---> {text}\n";
        }
    }
}