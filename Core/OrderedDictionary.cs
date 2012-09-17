using System.Linq;

namespace System.Collections.Generic
{
    [Serializable]
    internal class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _dict;
        private readonly List<KeyValuePair<TKey, TValue>> _list;

        public OrderedDictionary(int capacity)
        {
            this._dict = new Dictionary<TKey, TValue>(capacity);
            this._list = new List<KeyValuePair<TKey, TValue>>(capacity);
        }

        public OrderedDictionary()
        {
            this._dict = new Dictionary<TKey, TValue>();
            this._list = new List<KeyValuePair<TKey, TValue>>();
        }

        /// <summary>
        /// Gets the Dictionary class backing the OrderedDictionary.
        /// </summary>
        public Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                return this._dict;
            }
        }

        /// <summary>
        /// Get the ordered list of keys.
        /// This is a copy of the keys in the original Dictionary.
        /// </summary>
        public IList<TKey> OrderedKeys
        {
            get
            {
                var retList = new List<TKey>(this._list.Count);
                retList.AddRange(this._list.Select(kvp => kvp.Key));
                return retList;
            }
        }

        /// <summary>
        /// Get the ordered list of values.
        /// This is a copy of the values in the original Dictionary.
        /// </summary>
        public IList<TValue> OrderedValues
        {
            get
            {
                var retList = new List<TValue>(this._list.Count);
                retList.AddRange(this._list.Select(kvp => kvp.Value));
                return retList;
            }
        }

        /// <summary>
        /// Get/Set the value associated with the specified index.
        /// </summary>
        /// <param name="index">The index of the value to get or set.</param>
        /// <returns>The associated value.</returns>
        public TValue this[int index]
        {
            get
            {
                return this._list[index].Value;
            }
            set
            {
                if (index < 0 || index >= this._list.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                this._dict[this._list[index].Key] = value;
                this._list[index] = new KeyValuePair<TKey, TValue>(this._list[index].Key, value);
            }
        }


        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds a key-value pair to the KeyedList.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The associated value.</param>
        public void Add(TKey key, TValue value)
        {
            this._dict.Add(key, value);
            this._list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Test if the OrderedDictionary contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the key is found.</returns>
        public bool ContainsKey(TKey key)
        {
            return this._dict.ContainsKey(key);
        }

        /// <summary>
        /// Get an unordered list of keys.
        /// This collection refers back to the keys in the original Dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return this._dict.Keys;
            }
        }

        /// <summary>
        /// Get an unordered list of values.
        /// This collection refers back to the values in the original Dictionary.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return this._dict.Values;
            }
        }

        /// <summary>
        /// Remove the entry.
        /// </summary>
        /// <param name="key">The key identifying the key-value pair.</param>
        /// <returns>True if removed.</returns>
        public bool Remove(TKey key)
        {
            bool found = this._dict.Remove(key);
            if (found)
            {
                this._list.RemoveAt(this.IndexOf(key));
            }
            return found;
        }

        /// <summary>
        /// Attempt to get the value, given the key, without throwing an exception if not found.
        /// </summary>
        /// <param name="key">The key indentifying the entry.</param>
        /// <param name="value">The value, if found.</param>
        /// <returns>True if found.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._dict.TryGetValue(key, out value);
        }

        /// <summary>
        /// Get/Set the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The associated value.</returns>
        public TValue this[TKey key]
        {
            get
            {
                return this._dict[key];
            }
            set
            {
                if (this._dict.ContainsKey(key))
                {
                    this._dict[key] = value;
                    this._list[this.IndexOf(key)] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Adds a key-value pair to the OrderedDictionary.
        /// </summary>
        /// <param name="item">The KeyValuePair instance.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears all entries in the OrderedDictionary.
        /// </summary>
        public void Clear()
        {
            this._dict.Clear();
            this._list.Clear();
        }

        /// <summary>
        /// Test if the OrderedDictionary contains the key in the key-value pair.
        /// </summary>
        /// <param name="item">The key-value pair.</param>
        /// <returns>True if the key is found.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copy the entire key-value pairs to the KeyValuePair array, starting
        /// at the specified index of the target array.  The array is populated 
        /// as an ordered list.
        /// </summary>
        /// <param name="array">The KeyValuePair array.</param>
        /// <param name="arrayIndex">The position to start the copy.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the number of entries in the KeyedList.
        /// </summary>
        public int Count
        {
            get
            {
                return this._list.Count;
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the key in the specified KeyValuePair instance.  The Value
        /// property is ignored.
        /// </summary>
        /// <param name="item">The key-value identifying the entry.</param>
        /// <returns>True if removed.</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Returns an ordered KeyValuePair enumerator.
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        /// <summary>
        /// Returns an ordered System.Collections KeyValuePair objects.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion


        #region IList<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Given the key-value pair, find the index.
        /// </summary>
        /// <param name="item">The key-value pair.</param>
        /// <returns>The index, or -1 if not found.</returns>
        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            return this.IndexOf(item.Key);
        }

        /// <summary>
        /// Insert the key-value pair at the specified index location.
        /// </summary>
        /// <param name="index">The zero-based insert point.</param>
        /// <param name="item">The key-value pair.</param>
        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            this.Insert(index, item.Key, item.Value);
        }

        /// <summary>
        /// Remove the entry at the specified index.
        /// </summary>
        /// <param name="index">The index to the entry to be removed.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this._list.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this._dict.Remove(this._list[index].Key);
            this._list.RemoveAt(index);
        }

        /// <summary>
        /// Get/Set the key-value pair at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The key-value pair.</returns>
        KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
        {
            get
            {
                return this._list[index];
            }
            set
            {
                if (index < 0 || index >= this._list.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                this._list[index] = value;
                this._dict[value.Key] = value.Value;
            }
        }

        #endregion


        /// <summary>
        /// Returns the key at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The key at the index.</returns>
        public TKey GetKey(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this._list[index].Key;
        }

        /// <summary>
        /// Get the index of a particular key.
        /// </summary>
        /// <param name="key">The key to find the index of.</param>
        /// <returns>The index of the key, or -1 if not found.</returns>
        public int IndexOf(TKey key)
        {
            for (int i = 0; i < this._list.Count; i++)
            {
                if (this._list[i].Key.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Insert the key-value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based insert point.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, TKey key, TValue value)
        {
            if (index < 0 || index >= this._list.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this._dict.Add(key, value);
            this._list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
    }
}
