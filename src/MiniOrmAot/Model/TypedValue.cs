using System.Data;
using MiniOrmAot.Mapping;

namespace MiniOrmAot.Model; 

// keep this struct small (= <16 bytes)
public record struct TypedValue(object? Value) {
    // 8 bytes on 64 bit
    public object? Value { get; init; } = Value;
    // 4 bytes
    private readonly int _dbType = IRecordValues.DbTypeUndefined;
    // Total: 12 bytes
    
    public bool IsJson {
        get => _dbType == IRecordValues.DbTypeJson;
        init => _dbType = value ? IRecordValues.DbTypeJson : IRecordValues.DbTypeUndefined;
    }

    public DbType? DbType {
        get {
            if (IsJson) return System.Data.DbType.Object;
            return _dbType == IRecordValues.DbTypeUndefined ? null : (DbType)_dbType;
        }
        init {
            if (value.HasValue) {
                _dbType = (int)value;
            } else {
                _dbType = IRecordValues.DbTypeUndefined;
            }
        }
    }

    public static TypedValue CreateJsonValue(object? value) => new() {
        Value = value,
        IsJson = true
    };
}