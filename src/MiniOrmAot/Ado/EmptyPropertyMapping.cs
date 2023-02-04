using MiniOrmAot.Common;
using MiniOrmAot.Mapping;

namespace MiniOrmAot.Ado; 

public class EmptyPropertyMapping : IPropertyMapping {
    public static readonly IPropertyMapping Empty = new EmptyPropertyMapping();
    public static readonly IPropertyMapper EmptyMapper = new EmptyPropertyMapper();

    public IPropertyMapper ByColumName() => EmptyMapper;

    public IPropertyMapper ByPropertyName() => EmptyMapper;

    private class EmptyPropertyMapper : IPropertyMapper {
        public string TypeName => string.Empty;
        public string MappedTypeName => string.Empty;

        public IEnumerable<string> Keys => Enumerable.Empty<string>();
        
        public string MapName(string key) => key;

        public bool IsSqlIgnored(string key, StatementType statementType) => false;
    }
}