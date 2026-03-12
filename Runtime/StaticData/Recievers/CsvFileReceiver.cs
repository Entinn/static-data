using System;
using Entin.StaticData.CsvReader;
using Entin.StaticData.Validation;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public interface ISheetContainer<T>
        where T : IBaseSheet
    {
        T[] GetSheets();
    }

    public abstract class CsvFileReceiver<TSheet> : BaseDataReceiver<TSheet>
        where TSheet : IBaseSheet
    {
        public override Type DataType { get; }
        public override ValidationResult ValidationResult { get; } = new ValidationResult();
        private string FileName { get; }

        protected CsvFileReceiver(string fileName)
        {
            FileName = fileName;
            DataType = typeof(TSheet);
        }

        public override TSheet[] Receive()
        {
            TextAsset file = GetFile(FileName);

            if (typeof(TSheet).IsAssignableFrom(typeof(KeyValueSheet)))
            {
                TSheet parsed = Reader.ParseKeyValue<TSheet>(file.text);
                return new []{ parsed };
            }

            return Reader.Parse<TSheet>(file.text);
        }

        private TextAsset GetFile(string fileName)
        {
            string path = "Files/" + fileName;
            return Load<TextAsset>(path);
        }

        private T Load<T>(string path) where T : UnityEngine.Object
        {
            T resource = Resources.Load<T>(path);
            if (resource == null)
                Debug.LogError($"Resource {typeof(T)} not found in {path}");

            return resource;
        }
    }
}