using System;
using Entin.StaticData.Attributes;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class BaseDataReceiver<TSheet> : IDataReceiver
        where TSheet : IBaseSheet
    {
        public abstract Type DataType { get; }
        public abstract ValidationResult ValidationResult { get; }

        public abstract TSheet[] Receive();

        public virtual void Validate(StaticData staticData)
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