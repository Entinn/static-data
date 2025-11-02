using System;
using System.Collections.Generic;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public static class AttributeValidation
    {
        private static readonly List<IAttributeValidator> Validators = new List<IAttributeValidator>();

        static AttributeValidation()
        {
            Validators.Add(new UniquenessValidator());
            Validators.Add(new LinksValidator());
        }

        public static void Validate<TSheet>(StaticData staticData, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (IAttributeValidator attributeValidator in Validators)
                attributeValidator.Validate<TSheet>(staticData, onError);
        }
    }
}