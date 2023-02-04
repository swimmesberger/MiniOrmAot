namespace MiniOrmAot.Ado; 

public interface IStatementMaterializer {
    MaterializedStatements MaterializeBatch(StatementBatch statement);
    MaterializedStatement<T> MaterializeStatement<T>(Statement<T> statement);
}