using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private Dictionary<TKey, TValue> _dictionary;
        
        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        } 
         
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var newItems = new List<KeyValuePair<TKey, TValue>>();
            _dictionary.Add(item.Key, item.Value);
            newItems.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
        }

        public void Clear()
        {
            _dictionary.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the ICollection<T> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var result = _dictionary.Remove(item.Key);
            var oldItems = new List<KeyValuePair<TKey, TValue>>();

            if (result)
            {
                oldItems.Add(item);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
            }
                
            return result;
        }

        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            var newItems = new List<KeyValuePair<TKey, TValue>>();
            newItems.Add(new KeyValuePair<TKey, TValue>(key, value));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
        }

        public bool Remove(TKey key)
        {
            TValue value;
            var result = _dictionary.TryGetValue(key, out value);
            var oldItems = new List<KeyValuePair<TKey, TValue>>();
            if (result)
            {
                oldItems.Add(new KeyValuePair<TKey, TValue>(key, value));
                _dictionary.Remove(key);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set
            {
                var newItems = new List<KeyValuePair<TKey, TValue>>();
                var oldItems = new List<KeyValuePair<TKey, TValue>>();

                TValue oldValue;
                var newValue = value;


                _dictionary.TryGetValue(key, out oldValue);
                
                if (oldValue == null)
                {
                    _dictionary[key] = value; 
                    newItems.Add(new KeyValuePair<TKey, TValue>(key, newValue));

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
                }
                else if (!oldValue.Equals(newValue))
                {
                    //oldItems.Add(new KeyValuePair<TKey, TValue>(key, oldValue));
                    //newItems.Add(new KeyValuePair<TKey, TValue>(key, newValue));
                    //var changed = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems,
                    //    oldItems);
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
                    //CollectionChanged?.Invoke(this, changed);

                    //Remove old key
                    
                    if (_dictionary.Remove(key))
                    {
                        oldItems.Add(new KeyValuePair<TKey, TValue>(key, oldValue));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
                    }

                    //Re-add old key with new value

                    newItems.Add(new KeyValuePair<TKey, TValue>(key, newValue));
                    _dictionary[key] = newValue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
