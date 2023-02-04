namespace MiniOrmAot.Model; 

public abstract record DataModel : IDataModelBase {
    public Guid Id { get; init; } = Guid.NewGuid();
    // concurrencyToken
    public int RowVersion { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public DateTimeOffset LastModificationTime { get; init; }

    protected DataModel() {
        CreationTime = SystemClock.GetNow();
        LastModificationTime = CreationTime;
    }

    public virtual bool Equals(DataModel? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        // ignore CreationTime, LastModificationTime because those are not part of the domain model and should not be changed in the domain layer
        return Id.Equals(other.Id) && RowVersion == other.RowVersion;
    }
    
    public override int GetHashCode() {
        // ignore CreationTime, LastModificationTime because those are not part of the domain model and should not be changed in the domain layer
        return HashCode.Combine(Id, RowVersion);
    }
}