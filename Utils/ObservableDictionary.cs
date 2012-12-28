using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CipherPark.AngelJacket.Core.Utils
{
    [XmlRoot("dictionary")]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable
    {
        private Dictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {            
            _internalDictionary.Add(key, value);
            OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(key, value, DictionaryChangeAction.ValueKeyPairAdded));
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _internalDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            TValue value = this[key];
            bool result = _internalDictionary.Remove(key);
            if(result)
                OnDictionaryChange(new DictionaryChangeEventArgs<TKey, TValue>(key, value, DictionaryChangeAction.ValueKeyPairRemoved));
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _internalDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _internalDictionary[key];
            }
            set
            {                
                OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(key, _internalDictionary[key], DictionaryChangeAction.ValueChanging));
                _internalDictionary[key] = value;
                OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(key, _internalDictionary[key], DictionaryChangeAction.ValueChanged));
            }
        }
        #endregion

        public void Clear()
        {
            Dictionary<TKey, TValue> auxDictionary = new Dictionary<TKey, TValue>(_internalDictionary);
            _internalDictionary.Clear();
            foreach (TKey key in auxDictionary.Keys)
                OnDictionaryChange(new DictionaryChangeEventArgs<TKey, TValue>(key, auxDictionary[key], DictionaryChangeAction.ValueKeyPairRemoved));
        }

        #region Explicit ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey,TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Add(item);
            this.OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(item.Key, item.Value, DictionaryChangeAction.ValueKeyPairAdded));
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            this.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            bool result = ((ICollection<KeyValuePair<TKey, TValue>>)_internalDictionary).Remove(item);
            if(result)
                OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(item.Key, item.Value, DictionaryChangeAction.ValueKeyPairRemoved));
            return result;
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_internalDictionary).GetEnumerator();
        }

        #endregion      

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
          
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion

        #region Event Dispacthers
        protected virtual void OnDictionaryChange(DictionaryChangeEventArgs<TKey, TValue> args)
        {
            SetupItemPropertyChangedHandlers(args.Value, args.Action);
            DictionaryChangeEventHandler<TKey, TValue> handler = DictionaryChange;
            if (handler != null)
                handler(this, args);
        }
        #endregion

        #region Events
        public event DictionaryChangeEventHandler<TKey, TValue> DictionaryChange;
        #endregion

        #region Helpers
        private void SetupItemPropertyChangedHandlers(TValue value, DictionaryChangeAction action)
        {
            if (value is INotifyObservableDictionaryPropertyChanged<TKey>)
            {
                System.ComponentModel.INotifyPropertyChanged pcValue = (System.ComponentModel.INotifyPropertyChanged)value;
                switch (action)
                {
                    case DictionaryChangeAction.ValueChanged:
                        pcValue.PropertyChanged += DictionaryItem_ValuePropertyChanged;
                        break;
                    case DictionaryChangeAction.ValueChanging:
                        pcValue.PropertyChanged -= DictionaryItem_ValuePropertyChanged;
                        break;
                    case DictionaryChangeAction.ValueKeyPairAdded:
                        pcValue.PropertyChanged += DictionaryItem_ValuePropertyChanged;
                        break;
                    case DictionaryChangeAction.ValueKeyPairRemoved:
                        pcValue.PropertyChanged -= DictionaryItem_ValuePropertyChanged;
                        break;
                }
            }
        }

        private void DictionaryItem_ValuePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            INotifyObservableDictionaryPropertyChanged<TKey> inodpc = (INotifyObservableDictionaryPropertyChanged<TKey>)sender;            
            this.OnDictionaryChange(new DictionaryChangeEventArgs<TKey,TValue>(inodpc.Key, (TValue)sender, DictionaryChangeAction.ValuePropertyChanged, args.PropertyName));
        }
        #endregion       
    }

    public enum DictionaryChangeAction
    {
        ValueKeyPairAdded,
        ValueKeyPairRemoved,
        ValueChanging,
        ValueChanged,
        ValuePropertyChanged
    }

    public class DictionaryChangeEventArgs<TKey, TValue>
    {
        public TKey Key { get; private set; }       
        public TValue Value { get; private set; }
        public DictionaryChangeAction Action { get; private set; }
        public string ValuePropertyName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="action"></param>
        /// <param name="valuePropertyName"></param>
        /// <remarks>propertyName is ignored unless action is DictionaryChangeAction.ValuePropertyChanged</remarks>
        public DictionaryChangeEventArgs(TKey key, TValue value, DictionaryChangeAction action, string valuePropertyName = null)
        {
            Key = key;
            Value = value;
            Action = action;
            if( action == DictionaryChangeAction.ValuePropertyChanged )
                ValuePropertyName = valuePropertyName;
        }        
    }

    public delegate void DictionaryChangeEventHandler<TKey, TValue>(object sender, DictionaryChangeEventArgs<TKey, TValue> args);

    public interface INotifyObservableDictionaryPropertyChanged<TKey> : System.ComponentModel.INotifyPropertyChanged
    {
        TKey Key { get; }
    }
}
