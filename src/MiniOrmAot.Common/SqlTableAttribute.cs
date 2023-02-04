using System;

namespace MiniOrmAot.Common; 

public class SqlTableAttribute : Attribute {
    public string Value { get; }

    public SqlTableAttribute(string value) {
        Value = value;
    }
}