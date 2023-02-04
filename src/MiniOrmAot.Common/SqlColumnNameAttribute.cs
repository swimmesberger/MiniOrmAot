using System;

namespace MiniOrmAot.Common; 

public class SqlColumnNameAttribute : Attribute {
    public string Value { get; }

    public SqlColumnNameAttribute(string value) {
        Value = value;
    }
}