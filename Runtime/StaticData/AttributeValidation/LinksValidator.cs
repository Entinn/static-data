using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public class LinksValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                Attribute[] linkAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(LinkAttribute), true);

                foreach (var attribute in linkAttributes)
                {
                    if (attribute is LinkAttribute linkAttribute)
                        ValidateLinks<TSheet>(propertyInfo, staticData, linkAttribute.Type, linkAttribute.PropertyKey, linkAttribute.CanBeEmpty, onError);
                }
            }
        }

        private void ValidateLinks<TSheet>(PropertyInfo propertyInfo, StaticData staticData, Type type, string propertyKey, bool canBeEmpty, Action<string> onError)
            where TSheet : BaseSheet
        {
            if (!staticData.Receivers.ContainsKey(type))
            {
                onError("Static data don't have " + type);
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
                    onError("Static data don't have " + type);
                    return;
                }

                foreach (var item in sheets)
                {
                    PropertyInfo property = type.GetProperty(propertyKey);
                    if (property == null)
                    {
                        onError($"No property with key {propertyKey}, in table {type}");
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
                    onError($"No one {value2} in {type}");
                }
            }
        }
    }
}