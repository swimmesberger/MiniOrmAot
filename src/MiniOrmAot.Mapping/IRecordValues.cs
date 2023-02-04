namespace MiniOrmAot.Mapping; 

public interface IRecordValues : IPropertyMapper, IRecordMetadataProvider {
    public const int DbTypeJson = 10_000;
    public const int DbTypeUndefined = 0;

    object? GetObject(string key);
}