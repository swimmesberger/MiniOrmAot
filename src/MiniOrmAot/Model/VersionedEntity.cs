namespace MiniOrmAot.Model; 

public record VersionedEntity<T>(T Entity) {
    public int RowVersion { get; init; }

    public VersionedEntity(T entity, int rowVersion) : this(entity) {
        RowVersion = rowVersion;
    }
}