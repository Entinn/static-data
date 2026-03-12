using System;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Sheet.Receivers
{
    public interface IDataReceiver
    {
        Type DataType { get; }
        ValidationResult ValidationResult { get; }

        void Validate(StaticData staticData);
    }
}