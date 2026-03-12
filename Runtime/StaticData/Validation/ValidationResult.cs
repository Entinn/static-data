using UnityEngine;

namespace Entin.StaticData.Validation
{
    public class ValidationResult
    {
        public bool HasWarning { get; private set; }
        public string WarningText { get; private set; }

        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }

        public void AddWarning(string text)
        {
            HasWarning = true;
            WarningText += $"---> {text}\n";
        }

        public void AddError(string text)
        {
            HasError = true;
            ErrorText += $"---> {text}\n";
        }
    }
}
