using MiniOrmAot.Ado.Parameters;

namespace MiniOrmAot.Ado; 

public record MaterializedStatement(string Sql) {
    public static readonly MaterializedStatement Empty = new MaterializedStatement(string.Empty);
    
    public IEnumerable<QueryParameter> Parameters { get; init; } = Enumerable.Empty<QueryParameter>();

    public override string ToString() {
        if (this == Empty) return "EmptyStatement";
        return Sql;
    }
}

public record MaterializedStatement<T> : MaterializedStatement {
    public RowMapper<T> RowMapper { get; }

    public MaterializedStatement(MaterializedStatement statement, RowMapper<T> rowMapper) : base(statement) {
        RowMapper = rowMapper;
    }

    public override string ToString() => base.ToString();
}