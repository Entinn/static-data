using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entin.StaticData.Sheet;
using Entin.StaticData.Sheet.Receivers;
using UnityEngine;

namespace Entin.StaticData
{
    public interface IStaticData
    {
        IReadOnlyCollection<TSheet> Get<TSheet>() where TSheet : BaseSheet;
        IReadOnlyCollection<TSheet> GetAll<TSheet>();
        IReadOnlyDictionary<TKey, TSheet> GetKeyed<TKey, TSheet>() where TSheet : KeySheet<TKey>;
        TSheet GetKeyed<TKey, TSheet>(TKey key) where TSheet : KeySheet<TKey>;
        T GetKeyValue<T>() where T : KeyValueSheet;
    }

    public class StaticData : IStaticData
    {
        public Dictionary<Type, List<IDataReceiver>> Receivers => _receivers;
        private readonly Dictionary<Type, List<IDataReceiver>> _receivers = new Dictionary<Type, List<IDataReceiver>>();

        private readonly Dictionary<Type, object> _keyedSheets = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _allSheets = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _keyValueSheets = new Dictionary<Type, object>();

        public void AddKeyed<TReceiver, TSheet, TKeySheet>()
            where TSheet : KeySheet<TKeySheet>
            where TReceiver : FileReceiver<TSheet>
        {
            TReceiver receiver = Activator.CreateInstance<TReceiver>();
            Type type = GetGenericArgument(receiver);

            Add<TReceiver, TSheet>(type, receiver);
            AddToKeyedSheets<TReceiver, TSheet, TKeySheet>(type, receiver);
        }

        public void AddKeyValue<TReceiver, TSheet>()
            where TSheet : KeyValueSheet
            where TReceiver : KeyValueFileReceiver<TSheet>
        {
            TReceiver receiver = Activator.CreateInstance<TReceiver>();
            Type type = GetGenericArgument(receiver);

            AddReceiver(type, receiver);
            AddToKeyValueSheets<TReceiver, TSheet>(type, receiver);
        }

        public void Add<TReceiver, TSheet>()
            where TSheet : BaseSheet
            where TReceiver : FileReceiver<TSheet>
        {
            if (typeof(TSheet).IsAssignableFrom(typeof(KeySheet<>)))
                Debug.LogWarning($"Adding keyed sheet through non keyed method: {typeof(TSheet)}");

            TReceiver receiver = Activator.CreateInstance<TReceiver>();
            Type type = GetGenericArgument(receiver);

            Add<TReceiver, TSheet>(type, receiver);
        }

        private void Add<TReceiver, TSheet>(Type type, TReceiver receiver)
            where TSheet : BaseSheet
            where TReceiver : FileReceiver<TSheet>
        {
            AddReceiver(type, receiver);
            AddToAllSheets<TReceiver, TSheet>(type, receiver);
        }

        private void AddToAllSheets<TReceiver, TSheet>(Type type, TReceiver receiver)
            where TSheet : BaseSheet
            where TReceiver : FileReceiver<TSheet>
        {
            TSheet[] receivedItems = receiver.Receive(GetFile(receiver.FileName));

            if (_allSheets.TryGetValue(type, out object sheets))
            {
                ((List<TSheet>)sheets).AddRange(receivedItems);
            }
            else
            {
                var items = new List<TSheet>();
                items.AddRange(receivedItems);
                _allSheets.Add(type, items);
            }
        }

        private void AddToKeyValueSheets<TReceiver, TSheet>(Type type, TReceiver receiver)
            where TSheet : KeyValueSheet
            where TReceiver : KeyValueFileReceiver<TSheet>
        {
            TSheet receiverItems = receiver.Receive(GetFile(receiver.FileName));

            if (!_keyValueSheets.TryAdd(type, receiverItems))
                Debug.LogError("Key value sheet already added " + type);
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

        public T GetKeyValue<T>() where T : KeyValueSheet
        {
            return _keyValueSheets[typeof(T)] as T;
        }

        private void AddToKeyedSheets<TReceiver, TSheet, TKeySheet>(Type type, TReceiver receiver)
            where TSheet : KeySheet<TKeySheet>
            where TReceiver : FileReceiver<TSheet>
        {
            TSheet[] receiverItems = receiver.Receive(GetFile(receiver.FileName));

            if (_keyedSheets.TryGetValue(type, out object sheets))
            {
                Dictionary<TKeySheet, TSheet> dict = (Dictionary<TKeySheet, TSheet>)sheets;

                foreach (var item in receiverItems)
                    dict.Add(item.Key, item);
            }
            else
            {
                Dictionary<TKeySheet, TSheet> dict = new Dictionary<TKeySheet, TSheet>();

                foreach (var item in receiverItems)
                    dict.Add(item.Key, item);

                _keyedSheets.Add(type, dict);
            }
        }

        private void AddReceiver(Type type, IDataReceiver receiver)
        {
            if (!_receivers.ContainsKey(type))
                _receivers.Add(type, new List<IDataReceiver>());

            _receivers[type].Add(receiver);
        }

        private static Type GetGenericArgument(IDataReceiver receiver)
        {
            Type type = receiver.GetType();
            var types = type.GetGenericArguments();

            while (types.Length == 0)
            {
                type = type.BaseType;
                types = type.GetGenericArguments();
            }

            return types.First();
        }

        public IReadOnlyCollection<TSheet> Get<TSheet>() where TSheet : BaseSheet
        {
            Type type = typeof(TSheet);

            if (!_allSheets.TryGetValue(type, out object sheets))
                throw new Exception("Static data dont have " + type);

            return sheets as List<TSheet>;
        }

        public IReadOnlyCollection<T> GetAll<T>()
        {
            List<T> result = new List<T>();

            foreach (var list in _allSheets.Values)
            {
                if (list is IEnumerable enumerable)
                {
                    foreach (object item in enumerable)
                    {
                        if (item is T typedItem)
                            result.Add(typedItem);
                    }
                }
            }

            return result;
        }

        public IReadOnlyDictionary<TKey, TSheet> GetKeyed<TKey, TSheet>() where TSheet : KeySheet<TKey>
        {
            Type type = typeof(TSheet);

            if (!_keyedSheets.TryGetValue(type, out object sheets))
                throw new Exception("Static data dont have " + type);

            return sheets as Dictionary<TKey, TSheet>;
        }

        public TSheet GetKeyed<TKey, TSheet>(TKey key) where TSheet : KeySheet<TKey>
        {
            IReadOnlyDictionary<TKey, TSheet> dict = GetKeyed<TKey, TSheet>();
            return dict[key];
        }

        public bool Validate()
        {
            try
            {
                bool wasError = false;

                foreach (var receivers in _receivers)
                {
                    foreach (var receiver in receivers.Value)
                    {
                        receiver.Validate(this);
                        if (receiver.HasError)
                        {
                            wasError = true;
                            string errorText = $"Validation failed in table {receiver.FileName}. Watch errors below.\n";
                            errorText += receiver.ErrorText;
                            Debug.LogError(errorText);
                        }
                    }
                }

                return !wasError;
            }
            catch (Exception e)
            {
                Debug.LogError("Exception occur on validation " + e);
                return true;
            }
        }
    }
}