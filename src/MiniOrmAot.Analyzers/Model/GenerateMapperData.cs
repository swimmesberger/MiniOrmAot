using System;
using MiniOrmAot.Analyzers.Mapping;

namespace MiniOrmAot.Analyzers.Model {
    public class GenerateMapperData {
        public GenerateMapperEntity[] EntityTypes { get; set; } = Array.Empty<GenerateMapperEntity>();
        public PropertyNamingPolicy NamingPolicy { get; set; } = PropertyNamingPolicy.SnakeCase;
    }
}