namespace MiniOrmAot.Mapping; 

public interface IRecordMetadataProvider {
    // database type
    int? GetObjectType(string key);

    // clr type
    Type GetPropertyType(string key);
}