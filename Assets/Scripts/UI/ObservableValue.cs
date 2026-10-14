using System;

namespace UI
{
    public record ObservableValue<T>
    {
        private T _value;
        private Action<T> _action;

        public ObservableValue(T v)
        {
            _value = v;
        }

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _action?.Invoke(_value);
            }
        }

        public static ObservableValue<T> operator +(ObservableValue<T> lhs, Action<T> rhs)
        {
            if (lhs._action == null)
            {
                lhs._action = rhs;
            }
            else
            {
                lhs._action += rhs;
            }

            return lhs;
        }

        public static ObservableValue<T> operator -(ObservableValue<T> lhs, Action<T> rhs)
        {
            lhs._action -= rhs;
            return lhs;
        }

        public static implicit operator T(ObservableValue<T> value)
        {
            return value._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }
    }
}
