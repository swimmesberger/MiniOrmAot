using System.Collections.Generic;
using System.Collections.Immutable;

namespace MiniOrmAot.Analyzers.Model {
    public class MapperEntityProperty {
        public string PropertyName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IsJson { get; set; } = false;
        public ISet<string>? IgnoredStatementTypes { get; set; }
    }
}