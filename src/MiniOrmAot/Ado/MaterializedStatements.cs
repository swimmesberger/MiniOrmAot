using System.Collections.Immutable;

namespace MiniOrmAot.Ado;

public record MaterializedStatements {
    public IReadOnlyList<MaterializedStatement> Statements { get; init; } = ImmutableArray<MaterializedStatement>.Empty;
    
    public bool IsEmpty => Statements.Count <= 0;

    public MaterializedStatements(IEnumerable<MaterializedStatement> statements) {
        Statements = statements.Where(s => s != MaterializedStatement.Empty).ToImmutableArray();
    }

    public MaterializedStatements(MaterializedStatement statement) : this(ImmutableArray.Create(statement)) { }

    public override string ToString() {
        return string.Join(", ", Statements.Select(s => s.ToString()));
    }
}

public record MaterializedStatements<T> : MaterializedStatements {
    public RowMapper<T> RowMapper { get; }

    public MaterializedStatements(IEnumerable<MaterializedStatement> statements, RowMapper<T> rowMapper) : base(statements) {
        RowMapper = rowMapper;
    }
    
    public MaterializedStatements(MaterializedStatement statement, RowMapper<T> rowMapper) : base(statement) {
        RowMapper = rowMapper;
    }
    
    public MaterializedStatements(MaterializedStatements statements, RowMapper<T> rowMapper) : base(statements) {
        RowMapper = rowMapper;
    }
}