namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class BaseFileReceiver : IDataReceiver
    {
        public string FileName { get; }
        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }

        protected internal BaseFileReceiver(string fileName)
        {
            FileName = fileName;
        }

        public abstract void Validate(StaticData staticData);

        protected void AddError(string text)
        {
            HasError = true;
            ErrorText += $"---> {text}\n";
        }
    }
}