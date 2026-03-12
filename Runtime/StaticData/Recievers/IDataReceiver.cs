using Entin.StaticData.Validation;

namespace Entin.StaticData.Sheet.Receivers
{
    public interface IDataReceiver
    {
        string FileName { get; }
        ValidationResult ValidationResult { get; }

        void Validate(StaticData staticData);
    }
}