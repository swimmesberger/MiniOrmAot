using System.Data.Common;
using MiniOrmAot.Ado.Parameters;
using MiniOrmAot.Ado.Parameters.Where;
using MiniOrmAot.Common;
using MiniOrmAot.Mapping;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado.Impl; 

public class AdoStatementGenerator<T> : IStatementGenerator<T> where T: IDataModelBase {
    public IDataRecordMapper<T> DataRecordMapper { get; }

    public AdoStatementGenerator(IDataRecordMapper<T> recordMapper) {
        DataRecordMapper = recordMapper;
    }

    public Statement<long> CreateCount(StatementParameters statementParameters) {
        ValueTask<long> RowMapper(DbDataReader record, CancellationToken token) => record.GetValueAsync<long>(0, token);
        return new Statement<long>(StatementType.Count, DataRecordMapper.ByPropertyName(), RowMapper) {
            Parameters = statementParameters
        };
    }

    public Statement<T> CreateFind(StatementParameters statementParameters)
        => CreateFind(statementParameters, DataRecordMapper.EntityFromRecordAsync);

    public Statement<TResult> CreateFind<TResult>(StatementParameters statementParameters, RowMapper<TResult> mapper) {
        if (statementParameters.SelectParameters.IsAll) {
            statementParameters = statementParameters.WithSelect(DataRecordMapper.ByPropertyName().Keys);
        }
        return new(StatementType.Find, DataRecordMapper.ByPropertyName(), mapper) {
            Parameters = statementParameters
        };
    }

    public Statement CreateInsert(T entity) {
        var entityRecord = DataRecordMapper.RecordFromEntity(entity).ByPropertyName();
        var insertValues = entityRecord.Keys
            .Select(propertyName => new QueryParameter(propertyName, entityRecord.GetTypedValue(propertyName)))
            .Where(p => {
                // ignore the property if it has the SqlIgnore attribute and the default value
                var propMapper = DataRecordMapper.ByPropertyName();
                if (!propMapper.IsSqlIgnored(p.Name, StatementType.Create))
                    return true;
                var propType = entityRecord.GetPropertyType(p.Name);
                var defaultValue = propType.IsValueType ? Activator.CreateInstance(propType) : null;
                if (Equals(defaultValue, p.Value)) {
                    return false;
                }
                return true;
            })
            .ToList();
        if (insertValues.Count == 0) return Statement.Empty;

        var columnNames = insertValues.Select(q => q.Name);
        var insertParameters = new InsertParameters() {
            ColumnNames = columnNames,
            Values = new []{ insertValues }
        };
        return new Statement(StatementType.Create, DataRecordMapper.ByPropertyName()) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = new StatementParameters() { 
                InsertParameters = insertParameters
            }
        };
    }

    public StatementBatch CreateInsert(IEnumerable<T> entities) {
        return new StatementBatch(entities.Select(CreateInsert).ToList());
    }

    public StatementBatch CreateUpdate(IEnumerable<VersionedEntity<T>> versionedEntities) {
        return new StatementBatch(versionedEntities.Select(CreateUpdate).ToList());
    }

    public Statement CreateUpdate(T entity, int origRowVersion) {
        return CreateUpdate(new VersionedEntity<T>(entity, origRowVersion));
    }

    private Statement CreateUpdate(VersionedEntity<T> versionedEntity) {
        const string idColumnName = nameof(IDataModelBase.Id);
        const string rowVersionColumnName = nameof(IDataModelBase.RowVersion);
        const string creationColumnName = nameof(IDataModelBase.CreationTime);
        
        var record = DataRecordMapper.RecordFromEntity(versionedEntity.Entity).ByPropertyName();
        var updateParameterValues = GetPropertyNames()
            .Where(propertyName => propertyName != idColumnName && propertyName != creationColumnName)
            .Select(propertyName => new QueryParameter(propertyName, record.GetTypedValue(propertyName), $"{propertyName}"))
            .ToList();
        var whereParameters = new List<QueryParameter>() {
            new(idColumnName, versionedEntity.Entity.Id),
            new(rowVersionColumnName, versionedEntity.RowVersion, "curRowVersion")
        };
        return new Statement(StatementType.Update, DataRecordMapper.ByPropertyName()) {
            Parameters = new StatementParameters() {
                Update = new UpdateParameters() {
                    Values = updateParameterValues
                },
                Where = whereParameters
            }
        };
    }

    public Statement CreateDelete(T entity) => CreateDelete(new[] { entity });

    public Statement CreateDelete(IEnumerable<T> entities) {
        return new Statement(StatementType.Delete, DataRecordMapper.ByPropertyName()) {
            From = DataRecordMapper.ByPropertyName().MappedTypeName,
            Parameters = new StatementParameters() {
                WhereParameters = new WhereParameters(
                    new QueryParameter(nameof(IDataModelBase.Id), entities.Select(e => e.Id))
                )
            }
        };
    }

    private IEnumerable<string> GetPropertyNames() => DataRecordMapper.ByPropertyName().Keys;
}