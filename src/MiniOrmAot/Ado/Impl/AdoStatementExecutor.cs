using System.Data.Common;
using System.Runtime.CompilerServices;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado.Impl; 

public class AdoStatementExecutor : IStatementExecutor {
    private readonly IHasConnectionProvider _unitOfWorkManager;
    private readonly IDbParameterTypeSetter _typeSetter;

    public AdoStatementExecutor(IHasConnectionProvider unitOfWorkManager, IDbParameterTypeSetter typeSetter) {
        _unitOfWorkManager = unitOfWorkManager;
        _typeSetter = typeSetter;
    }

    public async Task<T?> QueryScalarAsync<T>(MaterializedStatements<T> statements, CancellationToken cancellationToken = default) {
        if (statements.IsEmpty) return default;
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statements, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var read = await reader.ReadAsync(cancellationToken);
        return read && reader.FieldCount != 0 ? await statements.RowMapper.Invoke(reader, cancellationToken) : default;
    }
    
    public IAsyncEnumerable<T> StreamAsync<T>(MaterializedStatements<T> statements, CancellationToken cancellationToken = default) {
        return statements.IsEmpty ? AsyncEnumerable.Empty<T>() : StreamAsyncImpl(statements, cancellationToken);
    }

    private async IAsyncEnumerable<T> StreamAsyncImpl<T>(MaterializedStatements<T> statements, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statements, connectionProvider, cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken)) {
            yield return await statements.RowMapper(reader, cancellationToken);
        }
    }

    public async Task<int> ExecuteAsync(MaterializedStatements statements, CancellationToken cancellationToken = default) {
        if (statements.IsEmpty) return 0;
        await using var connectionProvider = _unitOfWorkManager.ConnectionProvider;
        await using var cmd = await CreateCommand(statements, connectionProvider, cancellationToken);
        var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }
    
    private async Task<DbBatch> CreateCommand(MaterializedStatements materializedStatements, IConnectionProvider connectionProvider, 
        CancellationToken cancellationToken = default) {
        var dbConnection = await connectionProvider
                .GetDbConnectionAsync(cancellationToken: cancellationToken);
        var batch = dbConnection.CreateBatch();
        batch.Connection = dbConnection;
        batch.Transaction = connectionProvider.CurrentTransaction;
        foreach (var materializedStatement in materializedStatements.Statements) {
            var cmd = batch.CreateBatchCommand();
            cmd.CommandText = materializedStatement.Sql;
            foreach (var queryParameter in materializedStatement.Parameters) {
                var parameter = connectionProvider.Factory.CreateParameter();
                parameter = SetParameter(parameter, queryParameter.ParameterName, queryParameter.TypedValue);
                cmd.Parameters.Add(parameter);
            }
            batch.BatchCommands.Add(cmd);
        }
        return batch;
    }
    
    private DbParameter SetParameter(DbParameter parameter, string name, TypedValue typedValue = default) {
        parameter.ParameterName = name;
        parameter.Value = typedValue.Value ?? DBNull.Value;
        _typeSetter.SetParameterType(parameter, typedValue);
        return parameter;
    }
}