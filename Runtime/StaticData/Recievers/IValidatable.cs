namespace Entin.StaticData.Sheet.Receivers
{
    public interface IValidatable
    {
        bool HasError { get; }
        string ErrorText { get; }

        void Validate(StaticData staticData);
    }
}