using System;

namespace Statecharts.NET.Utilities
{
    public class OneOfBase<T0, T1>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1>(T0 t) => new OneOfBase<T0, T1>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1>(T1 t) => new OneOfBase<T0, T1>(1, value1: t);

        private bool Equals(OneOfBase<T0, T1> other)
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
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
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
    public class OneOfBase<T0, T1, T2>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2>(T0 t) => new OneOfBase<T0, T1, T2>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2>(T1 t) => new OneOfBase<T0, T1, T2>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2>(T2 t) => new OneOfBase<T0, T1, T2>(2, value2: t);

        private bool Equals(OneOfBase<T0, T1, T2> other)
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
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
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
    public class OneOfBase<T0, T1, T2, T3>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3>(T0 t) => new OneOfBase<T0, T1, T2, T3>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3>(T1 t) => new OneOfBase<T0, T1, T2, T3>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3>(T2 t) => new OneOfBase<T0, T1, T2, T3>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3>(T3 t) => new OneOfBase<T0, T1, T2, T3>(3, value3: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4>(4, value4: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5>(5, value5: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5, T6>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;
        internal readonly T6 _value6;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
                case T6 _: _index = 6; _value6 = (T6)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(5, value5: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6>(T6 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6>(6, value6: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5, T6> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                case 6: return Equals(_value6, other._value6);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
                case 6: return FormatValue(typeof(T6), _value6);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 6: return ((_value6?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;
        internal readonly T6 _value6;
        internal readonly T7 _value7;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default, T7 value7 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
                case T6 _: _index = 6; _value6 = (T6)(object)this; return;
                case T7 _: _index = 7; _value7 = (T7)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(5, value5: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T6 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(6, value6: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(T7 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7>(7, value7: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                case 6: return Equals(_value6, other._value6);
                case 7: return Equals(_value7, other._value7);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
                case 6: return FormatValue(typeof(T6), _value6);
                case 7: return FormatValue(typeof(T7), _value7);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 6: return ((_value6?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 7: return ((_value7?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;
        internal readonly T6 _value6;
        internal readonly T7 _value7;
        internal readonly T8 _value8;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default, T7 value7 = default, T8 value8 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
            _value8 = value8;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
                case T6 _: _index = 6; _value6 = (T6)(object)this; return;
                case T7 _: _index = 7; _value7 = (T7)(object)this; return;
                case T8 _: _index = 8; _value8 = (T8)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(5, value5: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T6 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(6, value6: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T7 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(7, value7: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T8 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>(8, value8: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                case 6: return Equals(_value6, other._value6);
                case 7: return Equals(_value7, other._value7);
                case 8: return Equals(_value8, other._value8);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
                case 6: return FormatValue(typeof(T6), _value6);
                case 7: return FormatValue(typeof(T7), _value7);
                case 8: return FormatValue(typeof(T8), _value8);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 6: return ((_value6?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 7: return ((_value7?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 8: return ((_value8?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;
        internal readonly T6 _value6;
        internal readonly T7 _value7;
        internal readonly T8 _value8;
        internal readonly T9 _value9;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default, T7 value7 = default, T8 value8 = default, T9 value9 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
            _value8 = value8;
            _value9 = value9;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
                case T6 _: _index = 6; _value6 = (T6)(object)this; return;
                case T7 _: _index = 7; _value7 = (T7)(object)this; return;
                case T8 _: _index = 8; _value8 = (T8)(object)this; return;
                case T9 _: _index = 9; _value9 = (T9)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(5, value5: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T6 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(6, value6: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T7 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(7, value7: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T8 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(8, value8: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T9 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(9, value9: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                case 6: return Equals(_value6, other._value6);
                case 7: return Equals(_value7, other._value7);
                case 8: return Equals(_value8, other._value8);
                case 9: return Equals(_value9, other._value9);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
                case 6: return FormatValue(typeof(T6), _value6);
                case 7: return FormatValue(typeof(T7), _value7);
                case 8: return FormatValue(typeof(T8), _value8);
                case 9: return FormatValue(typeof(T9), _value9);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 6: return ((_value6?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 7: return ((_value7?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 8: return ((_value8?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 9: return ((_value9?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public class OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        internal readonly int _index;
        internal readonly T0 _value0;
        internal readonly T1 _value1;
        internal readonly T2 _value2;
        internal readonly T3 _value3;
        internal readonly T4 _value4;
        internal readonly T5 _value5;
        internal readonly T6 _value6;
        internal readonly T7 _value7;
        internal readonly T8 _value8;
        internal readonly T9 _value9;
        internal readonly T10 _value10;

        protected OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default, T7 value7 = default, T8 value8 = default, T9 value9 = default, T10 value10 = default)
        {
            _index = index;
            _value0 = value0;
            _value1 = value1;
            _value2 = value2;
            _value3 = value3;
            _value4 = value4;
            _value5 = value5;
            _value6 = value6;
            _value7 = value7;
            _value8 = value8;
            _value9 = value9;
            _value10 = value10;
        }

        protected OneOfBase()
        {
            switch (this)
            {
                case T0 _: _index = 0; _value0 = (T0)(object)this; return;
                case T1 _: _index = 1; _value1 = (T1)(object)this; return;
                case T2 _: _index = 2; _value2 = (T2)(object)this; return;
                case T3 _: _index = 3; _value3 = (T3)(object)this; return;
                case T4 _: _index = 4; _value4 = (T4)(object)this; return;
                case T5 _: _index = 5; _value5 = (T5)(object)this; return;
                case T6 _: _index = 6; _value6 = (T6)(object)this; return;
                case T7 _: _index = 7; _value7 = (T7)(object)this; return;
                case T8 _: _index = 8; _value8 = (T8)(object)this; return;
                case T9 _: _index = 9; _value9 = (T9)(object)this; return;
                case T10 _: _index = 10; _value10 = (T10)(object)this; return;
            }
        }

        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(0, value0: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(1, value1: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T2 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(2, value2: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T3 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(3, value3: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T4 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(4, value4: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T5 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(5, value5: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T6 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(6, value6: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T7 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(7, value7: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T8 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(8, value8: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T9 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(9, value9: t);
        public static implicit operator OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T10 t) => new OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(10, value10: t);

        private bool Equals(OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> other)
        {
            if (_index != other._index) return false;
            switch (_index)
            {
                case 0: return Equals(_value0, other._value0);
                case 1: return Equals(_value1, other._value1);
                case 2: return Equals(_value2, other._value2);
                case 3: return Equals(_value3, other._value3);
                case 4: return Equals(_value4, other._value4);
                case 5: return Equals(_value5, other._value5);
                case 6: return Equals(_value6, other._value6);
                case 7: return Equals(_value7, other._value7);
                case 8: return Equals(_value8, other._value8);
                case 9: return Equals(_value9, other._value9);
                case 10: return Equals(_value10, other._value10);
                default: return false;
            }
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || (!(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> other && Equals(other));

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}: {value?.ToString()}";
            switch (_index)
            {
                case 0: return FormatValue(typeof(T0), _value0);
                case 1: return FormatValue(typeof(T1), _value1);
                case 2: return FormatValue(typeof(T2), _value2);
                case 3: return FormatValue(typeof(T3), _value3);
                case 4: return FormatValue(typeof(T4), _value4);
                case 5: return FormatValue(typeof(T5), _value5);
                case 6: return FormatValue(typeof(T6), _value6);
                case 7: return FormatValue(typeof(T7), _value7);
                case 8: return FormatValue(typeof(T8), _value8);
                case 9: return FormatValue(typeof(T9), _value9);
                case 10: return FormatValue(typeof(T10), _value10);
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
                    case 3: return ((_value3?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 4: return ((_value4?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 5: return ((_value5?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 6: return ((_value6?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 7: return ((_value7?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 8: return ((_value8?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 9: return ((_value9?.GetHashCode() ?? 0) * 397) ^ _index;
                    case 10: return ((_value10?.GetHashCode() ?? 0) * 397) ^ _index;
                    default: return 0 ^ _index;
                }
            }
        }
    }
    public static class OneOfBaseExtensions {
        public static TResult Match<T0, T1, TResult>(this OneOfBase<T0, T1> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1>(this OneOfBase<T0, T1> oneOf, Action<T0> f0, Action<T1> f1)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, TResult>(this OneOfBase<T0, T1, T2> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2>(this OneOfBase<T0, T1, T2> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, TResult>(this OneOfBase<T0, T1, T2, T3> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3>(this OneOfBase<T0, T1, T2, T3> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, TResult>(this OneOfBase<T0, T1, T2, T3, T4> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4>(this OneOfBase<T0, T1, T2, T3, T4> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5>(this OneOfBase<T0, T1, T2, T3, T4, T5> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, T6, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                case 6 when f6 != null: return f6(oneOf._value6);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5, T6>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                case 6 when f6 != null: f6(oneOf._value6); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, T6, T7, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                case 6 when f6 != null: return f6(oneOf._value6);
                case 7 when f7 != null: return f7(oneOf._value7);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5, T6, T7>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6, Action<T7> f7)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                case 6 when f6 != null: f6(oneOf._value6); return;
                case 7 when f7 != null: f7(oneOf._value7); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7, Func<T8, TResult> f8)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                case 6 when f6 != null: return f6(oneOf._value6);
                case 7 when f7 != null: return f7(oneOf._value7);
                case 8 when f8 != null: return f8(oneOf._value8);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6, Action<T7> f7, Action<T8> f8)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                case 6 when f6 != null: f6(oneOf._value6); return;
                case 7 when f7 != null: f7(oneOf._value7); return;
                case 8 when f8 != null: f8(oneOf._value8); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7, Func<T8, TResult> f8, Func<T9, TResult> f9)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                case 6 when f6 != null: return f6(oneOf._value6);
                case 7 when f7 != null: return f7(oneOf._value7);
                case 8 when f8 != null: return f8(oneOf._value8);
                case 9 when f9 != null: return f9(oneOf._value9);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6, Action<T7> f7, Action<T8> f8, Action<T9> f9)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                case 6 when f6 != null: f6(oneOf._value6); return;
                case 7 when f7 != null: f7(oneOf._value7); return;
                case 8 when f8 != null: f8(oneOf._value8); return;
                case 9 when f9 != null: f9(oneOf._value9); return;
                default: throw new InvalidOperationException();
            }
        }

        public static TResult Match<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> oneOf, Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6, Func<T7, TResult> f7, Func<T8, TResult> f8, Func<T9, TResult> f9, Func<T10, TResult> f10)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: return f0(oneOf._value0);
                case 1 when f1 != null: return f1(oneOf._value1);
                case 2 when f2 != null: return f2(oneOf._value2);
                case 3 when f3 != null: return f3(oneOf._value3);
                case 4 when f4 != null: return f4(oneOf._value4);
                case 5 when f5 != null: return f5(oneOf._value5);
                case 6 when f6 != null: return f6(oneOf._value6);
                case 7 when f7 != null: return f7(oneOf._value7);
                case 8 when f8 != null: return f8(oneOf._value8);
                case 9 when f9 != null: return f9(oneOf._value9);
                case 10 when f10 != null: return f10(oneOf._value10);
                default: throw new InvalidOperationException();
            }
        }
        public static void Switch<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> oneOf, Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6, Action<T7> f7, Action<T8> f8, Action<T9> f9, Action<T10> f10)
        {
            switch (oneOf._index)
            {
                case 0 when f0 != null: f0(oneOf._value0); return;
                case 1 when f1 != null: f1(oneOf._value1); return;
                case 2 when f2 != null: f2(oneOf._value2); return;
                case 3 when f3 != null: f3(oneOf._value3); return;
                case 4 when f4 != null: f4(oneOf._value4); return;
                case 5 when f5 != null: f5(oneOf._value5); return;
                case 6 when f6 != null: f6(oneOf._value6); return;
                case 7 when f7 != null: f7(oneOf._value7); return;
                case 8 when f8 != null: f8(oneOf._value8); return;
                case 9 when f9 != null: f9(oneOf._value9); return;
                case 10 when f10 != null: f10(oneOf._value10); return;
                default: throw new InvalidOperationException();
            }
        }

    }
}