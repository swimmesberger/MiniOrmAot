using Scriban;

namespace MiniOrmAot.Analyzers; 

public class InitializationContext {
    public const string CommonNamespace = "MiniOrmAot.Common";
    public const string MapperNamespace = "MiniOrmAot.Mapping";
    public const string GenerateNamespace = "MiniOrmAot.Gen";
    public const string GenerateMapperAttributeName = CommonNamespace + ".GenerateMapperAttribute";
    public const string GenerateMapperSimpleAttributeName = "GenerateMapper";
    public const string TenantIdColumnAttributeName = CommonNamespace + ".TenantIdColumnAttribute";
    public const string JsonColumnAttributeName = CommonNamespace + ".JsonColumnAttribute";
    public const string SqlIgnoreAttributeName = CommonNamespace + ".SqlIgnoreAttribute";
    public const string SqlTableAttributeName = CommonNamespace + ".SqlTableAttribute";
    public const string SqlColumnNameAttributeName = CommonNamespace + ".SqlColumnNameAttribute";
    public const string DataModelSuffix = "DataModel";
    public static readonly string Version = typeof(InitializationContext).Assembly.GetName().Version.ToString();

    public Template DataMapperTemplate { get; }
    public Template DataMapperServiceExtensionsTemplate { get; }

    public InitializationContext(Template dataMapperTemplate, Template dataMapperServiceExtensionsTemplate) {
        DataMapperTemplate = dataMapperTemplate;
        DataMapperServiceExtensionsTemplate = dataMapperServiceExtensionsTemplate;
    }
}