using System;

namespace Statecharts.NET.Utilities
{
    public class OneOfBase<T0, T1>
    {
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1, T2> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                case 3 when f3 != null: return f3(_value3);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                case 3 when f3 != null: f3(_value3); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1, T2, T3> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        private readonly T4 _value4;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                case 3 when f3 != null: return f3(_value3);
                case 4 when f4 != null: return f4(_value4);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                case 3 when f3 != null: f3(_value3); return;
                case 4 when f4 != null: f4(_value4); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        private readonly T4 _value4;
        private readonly T5 _value5;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                case 3 when f3 != null: return f3(_value3);
                case 4 when f4 != null: return f4(_value4);
                case 5 when f5 != null: return f5(_value5);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                case 3 when f3 != null: f3(_value3); return;
                case 4 when f4 != null: f4(_value4); return;
                case 5 when f5 != null: f5(_value5); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
        private readonly int _index;
        private readonly T0 _value0;
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        private readonly T4 _value4;
        private readonly T5 _value5;
        private readonly T6 _value6;

        private OneOfBase(int index, T0 value0 = default, T1 value1 = default, T2 value2 = default, T3 value3 = default, T4 value4 = default, T5 value5 = default, T6 value6 = default)
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

        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4, Func<T5, TResult> f5, Func<T6, TResult> f6)
        {
            switch (_index)
            {
                case 0 when f0 != null: return f0(_value0);
                case 1 when f1 != null: return f1(_value1);
                case 2 when f2 != null: return f2(_value2);
                case 3 when f3 != null: return f3(_value3);
                case 4 when f4 != null: return f4(_value4);
                case 5 when f5 != null: return f5(_value5);
                case 6 when f6 != null: return f6(_value6);
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

        public void Switch(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6)
        {
            switch (_index)
            {
                case 0 when f0 != null: f0(_value0); return;
                case 1 when f1 != null: f1(_value1); return;
                case 2 when f2 != null: f2(_value2); return;
                case 3 when f3 != null: f3(_value3); return;
                case 4 when f4 != null: f4(_value4); return;
                case 5 when f5 != null: f5(_value5); return;
                case 6 when f6 != null: f6(_value6); return;
                default: throw new InvalidOperationException("Unexpected index, which indicates a problem in the OneOfBase codegen.");
            }
        }

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
            ReferenceEquals(this, obj) || !(obj is null) && obj is OneOfBase<T0, T1, T2, T3, T4, T5, T6> other && Equals(other);

        public override string ToString()
        {
            string FormatValue<T>(Type type, T value) => $"{type.FullName}"; // TODO: {value?.ToString()}
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
}