using Entin.StaticData.Sheet;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Attributes
{
    public interface IAttributeValidator
    {
        void Validate<TSheet>(StaticData staticData, ValidationResult validationResult) where TSheet : IBaseSheet;
    }
}