using System.Data;

namespace MiniOrmAot.Mapping; 

public class PropertyMetadata {
    public string PropertyName { get; init; } = string.Empty;
    public string ColumnName { get; init; } = string.Empty;
    public string TypeName { get; init; } = string.Empty;
    public bool IsJson { get; init; } = false;
    public IReadOnlySet<StatementType>? IgnoredStatementTypes { get; init; }
}