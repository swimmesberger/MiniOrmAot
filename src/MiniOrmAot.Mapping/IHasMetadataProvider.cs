namespace MiniOrmAot.Mapping; 

public interface IHasMetadataProvider {
    IRecordMetadataProvider MetadataProvider { get; }
}