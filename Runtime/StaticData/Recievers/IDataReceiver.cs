namespace Entin.StaticData.Sheet.Receivers
{
    public interface IDataReceiver : IValidatable
    {
        string FileName { get; }
    }
}