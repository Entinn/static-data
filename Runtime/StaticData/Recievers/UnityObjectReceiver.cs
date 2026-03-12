using System;
using Entin.StaticData.Validation;
using UnityEngine;

namespace Entin.StaticData.Sheet.Receivers
{
    public abstract class UnityObjectReceiver<TSheet, TSheetContainer> : BaseDataReceiver<TSheet>
        where TSheet : IBaseSheet
        where TSheetContainer : UnityEngine.Object, ISheetContainer<TSheet>
    {
        public override Type DataType { get; } = typeof(TSheet);
        public override ValidationResult ValidationResult { get; } = new ValidationResult();
        private string FileName { get; }

        protected UnityObjectReceiver(string fileName)
        {
            FileName = fileName;
            DataType = typeof(TSheet);
        }

        public override TSheet[] Receive()
        {
            TSheetContainer sheetContainer = GetFile(FileName);

            if (typeof(TSheet).IsAssignableFrom(typeof(KeyValueSheet)))
            {
                return new []{ sheetContainer.GetSheets()[0] };
            }

            return sheetContainer.GetSheets();
        }

        private TSheetContainer GetFile(string fileName)
        {
            string path = "Files/" + fileName;
            return Load(path);
        }

        private TSheetContainer Load(string path)
        {
            var resource = Resources.Load<TSheetContainer>(path);
            if (resource == null)
                Debug.LogError($"Resource {typeof(TSheetContainer)} not found in {path}");

            return resource;
        }
    }
}