using MiniOrmAot.Ado.Parameters;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado; 

public interface IStatementGenerator<T> : IDataRecordProvider<T> {
    Statement<long> CreateCount(StatementParameters statementParameters);

    Statement<T> CreateFind(StatementParameters statementParameters);
    
    Statement<TResult> CreateFind<TResult>(StatementParameters statementParameters, RowMapper<TResult> mapper);

    Statement CreateInsert(T entity);
    
    StatementBatch CreateInsert(IEnumerable<T> entities);

    Statement CreateUpdate(T entity, int origRowVersion);
    
    StatementBatch CreateUpdate(IEnumerable<VersionedEntity<T>> entities);

    Statement CreateDelete(T entity);
    
    Statement CreateDelete(IEnumerable<T> entities);
}