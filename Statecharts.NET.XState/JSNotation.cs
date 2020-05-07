using System;
using System.Collections.Generic;
using System.Linq;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;

namespace Statecharts.NET.XState
{
    internal static class JPropertyFunctions
    {
        public static TResult Map<TResult>(
            this JSValue @this,
            Func<SimpleValue, TResult> fValue,
            Func<JSProperty, TResult> fProperty,
            Func<IEnumerable<TResult>, TResult> fObject,
            Func<IEnumerable<TResult>, TResult> fArray)
        {
            TResult Recurse(JSValue value) =>
                value.Map(fValue, fProperty, fObject, fArray);

            switch (@this)
            {
                case SimpleValue value:
                    return fValue(value);
                case ObjectValue @object:
                    return fObject(@object.Properties.Select(fProperty));
                case ArrayValue array:
                    return fArray(array.Values.Select(Recurse));
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        public static string AsString(this JSValue @this)
            => @this.Map(
                value => $"{(value.Value is string && !value.DontEscape ? $"'{value.Value}'" : value.Value)}",
                property => $"{property.Key}: {property.Value.AsString()}",
                properties => $"{{ {string.Join(", ", properties)} }}",
                values => $"[ {string.Join(", ", values)} ]");

        public static ObjectValue With(this ObjectValue @this, params JSProperty[] additionalProperties)
            => ObjectValue(@this.Properties.Concat(additionalProperties));
        public static ObjectValue With(this ObjectValue @this, ObjectValue @object)
            => ObjectValue(@this.Properties.Concat(@object.Properties));

        public static ArrayValue With(this ArrayValue @this, params JSValue[] additionalValues)
            => ArrayValue(@this.Values.Concat(additionalValues));
    }

    public static class JPropertyConstructorFunctions
    {
        public static SimpleValue SimpleValue(object value, bool dontEscape = false) => new SimpleValue(value, dontEscape);
        public static ObjectValue ObjectValue(IEnumerable<JSProperty> properties) => new ObjectValue(properties);
        public static ObjectValue ObjectValue(params JSProperty[] properties) => new ObjectValue(properties);
        public static ArrayValue ArrayValue(IEnumerable<JSValue> values) => new ArrayValue(values);
        public static ArrayValue ArrayValue(params JSValue[] values) => new ArrayValue(values);
        public static JSProperty JSProperty(string key, JSValue value) => new JSProperty(key, value);
        public static JSProperty JSProperty(string key, IEnumerable<JSProperty> properties) => new JSProperty(key, ObjectValue(properties));
        public static JSProperty JSProperty(string key, IEnumerable<JSValue> values) => new JSProperty(key, ArrayValue(values));
    }

    public abstract class JSValue { }

    
    public sealed class SimpleValue : JSValue
    {
        public object Value { get; }
        public bool DontEscape { get; }

        public SimpleValue(object value, bool dontEscape = false)
        {
            Value = value;
            DontEscape = dontEscape;
        }

        public static implicit operator SimpleValue(string value) => new SimpleValue(value);
        public static implicit operator SimpleValue(int value) => new SimpleValue(value);
        public static implicit operator SimpleValue(bool value) => new SimpleValue(value);
        public static implicit operator SimpleValue(double value) => new SimpleValue(value);
        public static implicit operator SimpleValue(char value) => new SimpleValue(value);
    }
    public sealed class ObjectValue : JSValue
    {
        public IEnumerable<JSProperty> Properties { get; }

        public ObjectValue(IEnumerable<JSProperty> properties)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public static implicit operator ObjectValue(List<JSProperty> properties) => new ObjectValue(properties);
        public static implicit operator ObjectValue(JSProperty[] properties) => new ObjectValue(properties);
    }
    public sealed class ArrayValue : JSValue
    {
        public IEnumerable<JSValue> Values { get; }

        public ArrayValue(IEnumerable<JSValue> properties)
        {
            Values = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        public static implicit operator ArrayValue(List<JSValue> values) => new ArrayValue(values);
        public static implicit operator ArrayValue(JSValue[] values) => new ArrayValue(values);
    }

    public class JSProperty
    {
        public string Key { get; }
        public JSValue Value { get; }

        public JSProperty(string key, JSValue value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static implicit operator JSProperty((string key, JSValue value) tuple) => new JSProperty(tuple.key, tuple.value);
        public static implicit operator JSProperty((string key, int value) tuple) => new JSProperty(tuple.key, SimpleValue(tuple.value));
        public static implicit operator JSProperty((string key, string value) tuple) => new JSProperty(tuple.key, SimpleValue(tuple.value));
        public static implicit operator JSProperty((string key, IEnumerable<JSProperty> properties) tuple) => new JSProperty(tuple.key, ObjectValue(tuple.properties));
        public static implicit operator JSProperty((string key, IEnumerable<JSValue> values) tuple) => new JSProperty(tuple.key, ArrayValue(tuple.values));

    }
}
