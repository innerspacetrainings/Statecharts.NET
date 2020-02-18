using System;

namespace Statecharts.NET.Utilities
{
    public struct OneOf<T0, T1>
    {
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;

        private OneOf(int index, T0 value0 = default, T1 value1 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
        }

        public static implicit operator OneOf<T0, T1>(T0 t) => new OneOf<T0, T1>(0, value0: t);
        public static implicit operator OneOf<T0, T1>(T1 t) => new OneOf<T0, T1>(1, value1: t);

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                default: throw new InvalidOperationException();
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                default: throw new InvalidOperationException();
            }
        }

        private bool Equals(OneOf<T0, T1> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            !(obj is null) && obj is OneOf<T0, T1> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOf codegen.");
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                switch (_index)
                {
                    case 0: return ((_value0?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 1: return ((_value1?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    
    public struct OneOf<T0, T1, T2>
    {
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;

        private OneOf(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
        }

        public static implicit operator OneOf<T0, T1, T2>(T0 t) => new OneOf<T0, T1, T2>(0, value0: t);
        public static implicit operator OneOf<T0, T1, T2>(T1 t) => new OneOf<T0, T1, T2>(1, value1: t);
        public static implicit operator OneOf<T0, T1, T2>(T2 t) => new OneOf<T0, T1, T2>(2, value2: t);

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                default: throw new InvalidOperationException();
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                default: throw new InvalidOperationException();
            }
        }

        private bool Equals(OneOf<T0, T1, T2> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            !(obj is null) && obj is OneOf<T0, T1, T2> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOf codegen.");
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                switch (_index)
                {
                    case 0: return ((_value0?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 1: return ((_value1?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 2: return ((_value2?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    
}