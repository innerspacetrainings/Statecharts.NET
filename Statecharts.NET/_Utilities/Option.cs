using System;
using System.Collections.Generic;

namespace Statecharts.NET.Utilities
{
    public static class Option
    {
        public static Option<T> From<T>(T value) => value.ToOption();
        internal static Option<T> Some<T>(T value) => new Option<T>(value, true);
        public static Option<T> None<T>() => new Option<T>(default(T), false);
    }

    public struct Option<T> : IEquatable<Option<T>>
    {
        internal T Value { get; }
        public bool HasValue { get; }

        internal Option(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public bool Equals(Option<T> other) =>
            !HasValue && !other.HasValue || HasValue && other.HasValue && EqualityComparer<T>.Default.Equals(Value, other.Value);
        public override bool Equals(object obj) => obj is Option<T> option && Equals(option);
        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
        public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
        public override int GetHashCode() => HasValue ? Value == null ? 1 : Value.GetHashCode() : 0;
        public override string ToString() => HasValue ? Value == null ? "Some(null)" : $"Some({Value})" : "None";

        public bool Contains(T value) => HasValue && (Value == null ? value == null : Value.Equals(value));
        public bool Exists(Func<T, bool> predicate) =>
            predicate == null
                ? throw new ArgumentNullException(nameof(predicate))
                : HasValue && predicate(Value);

        public T ValueOr(T alternative) => HasValue ? Value : alternative;
        public T ValueOr(Func<T> alternativeFactory) =>
            alternativeFactory == null
                ? throw new ArgumentNullException(nameof(alternativeFactory))
                : HasValue
                    ? Value
                    : alternativeFactory();
        public Option<T> Or(T alternative) => HasValue ? this : Option.Some(alternative);
        public Option<T> Or(Func<T> alternativeFactory) =>
            alternativeFactory == null
                ? throw new ArgumentNullException(nameof(alternativeFactory))
                : HasValue
                    ? this
                    : Option.Some(alternativeFactory());
        public Option<T> Else(Option<T> alternativeOption) => HasValue ? this : alternativeOption;
        public Option<T> Else(Func<Option<T>> alternativeOptionFactory) =>
            alternativeOptionFactory == null
                ? throw new ArgumentNullException(nameof(alternativeOptionFactory))
                : HasValue
                    ? this
                    : alternativeOptionFactory();

        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));
            return HasValue ? some(Value) : none();
        }
        public void Switch(Action<T> some, Action none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));

            if (HasValue)
                some(Value);
            else
                none();
        }
        public void SwitchSome(Action<T> some)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (HasValue) some(Value);
        }
        public void SwitchNone(Action none)
        {
            if (none == null) throw new ArgumentNullException(nameof(none));
            if (!HasValue) none();
        }
        public Option<TResult> Map<TResult>(Func<T, TResult> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            return Match(
                some: value => Option.Some(mapping(value)),
                none: () => Option.None<TResult>()
            );
        }
        public Option<TResult> FlatMap<TResult>(Func<T, Option<TResult>> mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            return Match(
                some: mapping,
                none: () => Option.None<TResult>()
            );
        }

        public Option<T> Filter(bool condition) => HasValue && !condition ? Option.None<T>() : this;
        public Option<T> Filter(Func<T, bool> predicate) =>
            predicate == null
                ? throw new ArgumentNullException(nameof(predicate))
                : HasValue && !predicate(Value)
                    ? Option.None<T>()
                    : this;
        public Option<T> NotNull() => HasValue && Value == null ? Option.None<T>() : this;
    }

    public static class OptionExtensions
    {
        public static Option<T> ToOption<T>(this T value) =>
            value != null ? Option.Some(value) : Option.None<T>();
        public static Option<T> ToOption<T>(this T? value) where T : struct =>
            value.HasValue ? Option.Some(value.Value) : Option.None<T>();
        public static Option<T> Flatten<T>(this Option<Option<T>> option) =>
            option.FlatMap(innerOption => innerOption);
    }
}
