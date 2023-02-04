using System.Data.Common;

namespace MiniOrmAot.Ado; 

public interface IConnectionProvider : IAsyncDisposable {
    DbTransaction? CurrentTransaction { get; }

    IConnectionFactory Factory { get; }
    
    Task<DbConnection> GetDbConnectionAsync(CancellationToken cancellationToken = default);
}