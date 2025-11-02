using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public class UniquenessValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(UniqueAttribute), true).Any())
                    ValidateUniqueness<TSheet>(staticData, propertyInfo, onError);
            }
        }

        private void ValidateUniqueness<TSheet>(StaticData staticData, PropertyInfo propertyInfo, Action<string> onError)
            where TSheet : BaseSheet
        {
            HashSet<object> hashSet = new HashSet<object>();
            IEnumerable<object> values = staticData.Get<TSheet>().Select(propertyInfo.GetValue);
            foreach (object value in values)
            {
                if (!hashSet.Add(value))
                    onError($"Unique map key ({propertyInfo.Name}) duplicate value found: {value}");
            }
        }
    }
}