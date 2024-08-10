using System;

namespace Tool.Utilities.Events
{
    public class ValueBindery<V> where V : struct, IEquatable<V>
    {
        private V _value;

        public V Value
        {
            set
            {
                if (_value.Equals(value)) return;
                _value = value;
                _valueChangeEvent?.Invoke(Value);
            }
            get => _value;
        }

        private Action<V> _valueChangeEvent;
        public ValueBindery(V value = default) => _value = value;
        public void UnRegister(Action<V> valueChangeEvent) => _valueChangeEvent -= valueChangeEvent;
        public void OnRegister(Action<V> valueChangeEvent) => _valueChangeEvent += valueChangeEvent;

        public void OnRegisterWithValue(Action<V> valueChangeEvent)
        {
            _valueChangeEvent += valueChangeEvent;
            _valueChangeEvent?.Invoke(Value);
        }

        public override string ToString() => _value.ToString();
    }

    public class StringBindery
    {
        private string _value;

        public string Value
        {
            set
            {
                if (_value.Equals(value)) return;
                _value = value;
                _valueChangeEvent?.Invoke(Value);
            }
            get => _value;
        }

        private Action<string> _valueChangeEvent;
        public StringBindery(string value = default) => _value = value;
        public void UnRegister(Action<string> valueChangeEvent) => _valueChangeEvent -= valueChangeEvent;
        public void OnRegister(Action<string> valueChangeEvent) => _valueChangeEvent += valueChangeEvent;

        public void OnRegisterWithValue(Action<string> valueChangeEvent)
        {
            _valueChangeEvent += valueChangeEvent;
            _valueChangeEvent?.Invoke(Value);
        }

        public override string ToString() => _value.ToString();
    }

}