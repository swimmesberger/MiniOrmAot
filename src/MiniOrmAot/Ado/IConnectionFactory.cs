using System.Data.Common;

namespace MiniOrmAot.Ado; 

public interface IConnectionFactory {
    DbConnection CreateConnection();
    
    DbParameter CreateParameter();
}