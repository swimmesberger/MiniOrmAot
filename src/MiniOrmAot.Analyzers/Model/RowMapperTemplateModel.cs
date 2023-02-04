namespace MiniOrmAot.Analyzers.Model {
    public class RowMapperTemplateModel {
        public string CommonNamespace { get; set; } = InitializationContext.CommonNamespace;
        public string MapperNamespace { get; set; } = InitializationContext.MapperNamespace;
        public string Namespace { get; set; } = InitializationContext.GenerateNamespace;
        public string Version { get; set; } = InitializationContext.Version;
        public GenerateMapperEntity Entity { get; set; } = new GenerateMapperEntity();
    }
}