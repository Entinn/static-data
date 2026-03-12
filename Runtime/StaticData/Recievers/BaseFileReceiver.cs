using Entin.StaticData.Attributes;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class BaseFileReceiver<TSheet> : IDataReceiver
        where TSheet : BaseSheet
    {
        public string FileName { get; }

        public ValidationResult ValidationResult { get; } = new ValidationResult();

        protected internal BaseFileReceiver(string fileName)
        {
            FileName = fileName;
        }

        public void Validate(StaticData staticData)
        {
            ValidateAttributes(staticData);
            ValidateData(staticData, ValidationResult);
        }

        protected abstract void ValidateData(StaticData staticData, ValidationResult validationResult);

        private void ValidateAttributes(StaticData staticData)
        {
            AttributeValidation.Validate<TSheet>(staticData, ValidationResult);
        }
    }
}