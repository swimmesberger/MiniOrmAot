using MiniOrmAot.Common;

namespace MiniOrmAot.Mapping; 

public interface IPropertyMapper {
    string TypeName { get; }
    string MappedTypeName { get; }
    IEnumerable<string> Keys { get; }

    bool IsSqlIgnored(string key, StatementType statementType);

    string MapName(string key);
}