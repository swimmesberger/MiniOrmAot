using System.Data.Common;

namespace MiniOrmAot.Ado; 

public delegate ValueTask<T> RowMapper<T>(DbDataReader record, CancellationToken cancellationToken = default);