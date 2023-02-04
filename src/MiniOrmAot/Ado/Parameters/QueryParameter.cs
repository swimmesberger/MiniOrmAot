using MiniOrmAot.Ado.Parameters.Where;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado.Parameters;

public record QueryParameter(string Name) {
    public string ParameterName { get; init; } = Name;

    public TypedValue TypedValue { get; init; }
    
    public WhereComparator Comparator { get; init; }

    public object? Value {
        get => TypedValue.Value;
        init {
            if (value is TypedValue typedValue) {
                TypedValue = typedValue;
            } else {
                TypedValue = new TypedValue() { Value = value };
            }
        }
    }
    
    public QueryParameter(string name, TypedValue value) : this(name, WhereComparator.Equal, value) { }
    
    public QueryParameter(string name, WhereComparator comparator, TypedValue value) : this(name, comparator, value, null) { }
    
    public QueryParameter(string name, TypedValue value, string parameterName) : this(name, WhereComparator.Equal, value, parameterName) { }

    public QueryParameter(string name, WhereComparator comparator, TypedValue value, string? parameterName = null) : this(name) {
        TypedValue = value;
        Comparator = comparator;
        if (parameterName != null)
            ParameterName = parameterName;
    }
    
    public QueryParameter(string name, object? value) : this(name, WhereComparator.Equal, value) { }

    public QueryParameter(string name, WhereComparator comparator, object? value) : this(name, comparator, value, null) { }

    public QueryParameter(string name, object? value, string parameterName) : this(name, WhereComparator.Equal, value, parameterName) { }
    
    public QueryParameter(string name, WhereComparator comparator, object? value, string? parameterName = null) : this(name) {
        Value = value;
        Comparator = comparator;
        if (parameterName != null)
            ParameterName = parameterName;
    }
}