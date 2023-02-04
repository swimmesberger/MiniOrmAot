namespace MiniOrmAot.Ado;

public record StatementBatch(IEnumerable<Statement> Statements) {
    public StatementBatch(params Statement[] statements) : this((IEnumerable<Statement>)statements) {}
}