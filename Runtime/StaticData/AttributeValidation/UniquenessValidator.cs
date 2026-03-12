using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Attributes
{
    public class UniquenessValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, ValidationResult validationResult)
            where TSheet : IBaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(UniqueAttribute), true).Any())
                    ValidateUniqueness<TSheet>(staticData, propertyInfo, validationResult);
            }
        }

        private void ValidateUniqueness<TSheet>(StaticData staticData, PropertyInfo propertyInfo, ValidationResult validationResult)
            where TSheet : IBaseSheet
        {
            HashSet<object> hashSet = new HashSet<object>();
            IEnumerable<object> values = staticData.Get<TSheet>().Select(x => propertyInfo.GetValue(x));
            foreach (object value in values)
            {
                if (!hashSet.Add(value))
                    validationResult.AddError($"Unique map key ({propertyInfo.Name}) duplicate value found: {value}");
            }
        }
    }
}