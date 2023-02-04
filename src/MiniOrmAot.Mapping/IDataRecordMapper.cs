using System.Data.Common;

namespace MiniOrmAot.Mapping; 

public interface IDataRecordMapper<T> : IPropertyMapping, IHasMetadataProvider {
    IReadOnlyList<PropertyMetadata> Properties { get; }

    ValueTask<T> EntityFromRecordAsync(DbDataReader record, CancellationToken cancellationToken = default);

    IRecord RecordFromEntity(T record);
}