using System;
using System.Reflection;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Sheet;

namespace Entin.StaticData.Attributes
{
    public class MoreLessValidator : IAttributeValidator
    {
        public void Validate<TSheet>(StaticData staticData, Action<string> onError)
            where TSheet : BaseSheet
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                Attribute[] minAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(MoreLessAttribute), true);

                foreach (var attribute in minAttributes)
                {
                    ValidateMoreOrLess<TSheet>(staticData, propertyInfo, attribute, onError);
                }
            }
        }

        private void ValidateMoreOrLess<TSheet>(StaticData staticData, PropertyInfo propertyInfo, Attribute attribute, Action<string> onError)
            where TSheet : BaseSheet
        {
            bool isLessThan = attribute is LessThanAttribute;
            bool isMoreThan = attribute is MoreThanAttribute;

            LessThanAttribute lessThan = attribute as LessThanAttribute;
            MoreThanAttribute moreThan = attribute as MoreThanAttribute;

            if (!isLessThan && !isMoreThan)
                return;

            string propertyKey = isLessThan ? lessThan.PropertyKey : moreThan.PropertyKey;

            foreach (TSheet baseSheet in staticData.Get<TSheet>())
            {
                PropertyInfo property = typeof(TSheet).GetProperty(propertyKey);
                if (property == null)
                {
                    onError($"No property with key {propertyKey}, in table {typeof(TSheet)}");
                    return;
                }

                object value1 = propertyInfo.GetValue(baseSheet);
                object value2 = property.GetValue(baseSheet);

                if (value1 is not IComparable value1Comparable)
                {
                    onError($"Property with key {propertyInfo.Name} is not comparable");
                    return;
                }

                if (value2 is not IComparable value2Comparable)
                {
                    onError($"Property with key {propertyKey} is not comparable");
                    return;
                }

                int comparison = value1Comparable.CompareTo(value2Comparable);
                bool orEqual = isLessThan ? lessThan.OrEqual : moreThan.OrEqual;

                bool hasError = isLessThan
                    ? (orEqual ? comparison > 0 : comparison >= 0)
                    : (orEqual ? comparison < 0 : comparison <= 0);

                if (hasError)
                {
                    string operation = isLessThan
                        ? (orEqual ? "less or equal" : "less")
                        : (orEqual ? "move or equal" : "move");

                    onError($"{propertyInfo.Name} should be {operation} {propertyKey}");
                }
            }
        }
    }
}