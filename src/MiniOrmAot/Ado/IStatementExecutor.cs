namespace MiniOrmAot.Ado; 

public interface IStatementExecutor {
    Task<T?> QueryScalarAsync<T>(MaterializedStatements<T> statements, CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> StreamAsync<T>(MaterializedStatements<T> statements, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(MaterializedStatements statements, CancellationToken cancellationToken = default);
}