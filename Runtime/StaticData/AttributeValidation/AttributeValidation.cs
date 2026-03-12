using System.Collections.Generic;
using Entin.StaticData.Sheet;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Attributes
{
    public static class AttributeValidation
    {
        private static readonly List<IAttributeValidator> Validators = new List<IAttributeValidator>();

        static AttributeValidation()
        {
            Validators.Add(new UniquenessValidator());
            Validators.Add(new LinksValidator());
            Validators.Add(new MoreLessValidator());
            Validators.Add(new MinMaxValidator());
        }

        public static void Validate<TSheet>(StaticData staticData, ValidationResult validationResult)
            where TSheet : IBaseSheet
        {
            foreach (IAttributeValidator attributeValidator in Validators)
                attributeValidator.Validate<TSheet>(staticData, validationResult);
        }
    }
}