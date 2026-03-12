using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;
using Entin.StaticData.Validation;

namespace Entin.StaticData.Attributes
{
    public class LinksValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, ValidationResult validationResult)
            where TSheet : IBaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                Attribute[] linkAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(LinkAttribute), true);

                foreach (var attribute in linkAttributes)
                {
                    if (attribute is LinkAttribute linkAttribute)
                        ValidateLinks<TSheet>(propertyInfo, staticData, linkAttribute.Type, linkAttribute.PropertyKey, linkAttribute.CanBeEmpty, validationResult);
                }
            }
        }

        private void ValidateLinks<TSheet>(PropertyInfo propertyInfo, StaticData staticData, Type type, string propertyKey, bool canBeEmpty, ValidationResult validationResult)
            where TSheet : IBaseSheet
        {
            if (!staticData.Receivers.ContainsKey(type))
            {
                validationResult.AddError("Static data don't have " + type);
                return;
            }

            foreach (TSheet baseSheet in staticData.Get<TSheet>())
            {
                bool have = false;
                object value2 = propertyInfo.GetValue(baseSheet);

                if (canBeEmpty)
                {
                    if (value2 == null)
                        have = true;

                    if (value2 is string text)
                    {
                        if (string.IsNullOrEmpty(text))
                            have = true;
                    }
                }

                if (!staticData.TryGet(type, out IList sheets))
                {
                    validationResult.AddError("Static data don't have " + type);
                    return;
                }

                foreach (var item in sheets)
                {
                    PropertyInfo property = type.GetProperty(propertyKey);
                    if (property == null)
                    {
                        validationResult.AddError($"No property with key {propertyKey}, in table {type}");
                        return;
                    }

                    object value1 = property.GetValue(item);
                    if (!value1.Equals(value2))
                        continue;

                    have = true;
                    break;
                }

                if (!have)
                {
                    validationResult.AddError($"No one {value2} in {type}");
                }
            }
        }
    }
}