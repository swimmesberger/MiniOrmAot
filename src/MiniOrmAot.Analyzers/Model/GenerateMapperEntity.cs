using System.Collections.Generic;
using System.Linq;

namespace MiniOrmAot.Analyzers.Model {
    public class GenerateMapperEntity {
        public string Namespace { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string FullTypeName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public IEnumerable<MapperEntityProperty> Properties { get; set; } = Enumerable.Empty<MapperEntityProperty>();
        public MapperEntityProperty? TenantIdProperty { get; set; }
    }
}