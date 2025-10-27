using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entin.StaticData.CsvReader;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public interface IDataReceiver : IValidatable
    {
        string FileName { get; }
    }

    public interface IValidatable
    {
        bool HasError { get; }
        string ErrorText { get; }

        void Validate(StaticData staticData);
    }

    public abstract class BaseFileReceiver : IDataReceiver
    {
        public string FileName { get; }
        public bool HasError { get; private set; }
        public string ErrorText { get; private set; }

        protected BaseFileReceiver(string fileName)
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

    public abstract class KeyValueFileReceiver<TSheet> : BaseFileReceiver where TSheet : KeyValueSheet
    {
        protected KeyValueFileReceiver(string fileName) : base(fileName)
        {
        }

        public TSheet Receive(TextAsset text)
        {
            return Reader.ParseKeyValue<TSheet>(text.text);
        }
    }

    public abstract class FileReceiver<TSheet> : BaseFileReceiver where TSheet : BaseSheet
    {

        protected FileReceiver(string fileName) : base(fileName)
        {
        }

        public TSheet[] Receive(TextAsset text)
        {
            return Reader.Parse<TSheet>(text.text);
        }

        public sealed override void Validate(StaticData staticData)
        {
            ValidateAttributes(staticData);
            ValidateData(staticData);
        }

        protected abstract void ValidateData(StaticData staticData);

        private void ValidateAttributes(StaticData staticData)
        {
            foreach (PropertyInfo propertyInfo in typeof(TSheet).GetProperties())
            {
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(UniqueAttribute), true).Any())
                {
                    ValidateUniqueness(staticData, propertyInfo);
                }

                Attribute[] linkAttributes = Attribute.GetCustomAttributes(propertyInfo, typeof(LinkAttribute), true);

                if (linkAttributes.Any())
                {
                    LinkAttribute linkAttribute = linkAttributes.Single() as LinkAttribute;
                    ValidateLinks(propertyInfo, staticData, linkAttribute.Type, linkAttribute.FiledKey, linkAttribute.CanBeEmpty);
                }
            }
        }

        private void ValidateUniqueness(StaticData staticData, PropertyInfo propertyInfo)
        {
            HashSet<object> hashSet = new HashSet<object>();
            IEnumerable<object> values = staticData.Get<TSheet>().Select(propertyInfo.GetValue);
            foreach (object value in values)
            {
                if (!hashSet.Add(value))
                    AddError($"Unique map key ({propertyInfo.Name}) duplicate value found: {value}");
            }
        }

        private void ValidateLinks(PropertyInfo propertyInfo, StaticData staticData, Type type, string propertyKey, bool canBeEmpty)
        {
            if (!staticData.Receivers.ContainsKey(type))
            {
                AddError("Static data don't have " + type);
                return;
            }

            foreach (TSheet baseSheet in staticData.Get<TSheet>())
            {
                bool have = false;
                object value2 = propertyInfo.GetValue(baseSheet);

                if (canBeEmpty)
                {
                    if (value2 == default)
                        have = true;

                    if (value2 is string text)
                    {
                        if (string.IsNullOrEmpty(text))
                            have = true;
                    }
                }

                foreach (BaseSheet item in staticData.GetAll<BaseSheet>())
                {
                    object value1 = type.GetProperty(propertyKey).GetValue(item);
                    if (!value1.Equals(value2))
                        continue;

                    have = true;
                    break;
                }

                if (!have)
                {
                    AddError($"No one {value2} in {type}");
                }
            }
        }

    }
}