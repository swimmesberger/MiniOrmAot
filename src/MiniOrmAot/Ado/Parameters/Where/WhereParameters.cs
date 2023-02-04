using System.Collections.Immutable;

namespace MiniOrmAot.Ado.Parameters.Where;

public record WhereParameters : IWhereStatement {
    public static readonly WhereParameters Empty = new WhereParameters();

    public IEnumerable<IWhereStatement> Statements { get; init; } = Enumerable.Empty<IWhereStatement>();

    public WhereParameters() { }

    public WhereParameters(IEnumerable<IWhereStatement> statements) {
        Statements = Simplify(statements);
    }

    public WhereParameters(IWhereStatement statement) : this(ImmutableArray.Create(statement)) { }
    
    public WhereParameters(IEnumerable<QueryParameter> parameters) : this(new SimpleWhere(parameters)) { }

    public WhereParameters(QueryParameter parameter) : this(new SimpleWhere(parameter)) { }
    
    public WhereParameters(string name, object value) : this(new QueryParameter(name, value)) { }

    public IEnumerable<QueryParameter> Parameters => Statements.SelectMany(s => s.Parameters);
    
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((WhereParameters)where);
    
    public WhereParameters Add(WhereParameters where) {
        var statements = new List<IWhereStatement>();
        statements.AddRange(Statements);
        statements.AddRange(where.Statements);
        return new WhereParameters(statements);
    }
    
    public WhereParameters Add(IWhereStatement statement) {
        var statements = new List<IWhereStatement>();
        statements.AddRange(Statements);
        statements.Add(statement);
        return new WhereParameters(statements);
    }

    public WhereParameters MapParameterNames(Func<string, string> selector) 
        => new WhereParameters(Statements.Select(s => s.MapParameterNames(selector)).ToList());
    
    private static IEnumerable<IWhereStatement> Simplify(IEnumerable<IWhereStatement> statements) {
        var simpleWhere = SimpleWhere.Empty;
        var rowValueWhere = RowValueWhere.Empty;
        var searchWhere = SearchWhere.Empty;
        foreach (var whereStatement in statements) {
            switch (whereStatement) {
                case SimpleWhere addedSimpleWhere:
                    simpleWhere = simpleWhere.Add(addedSimpleWhere);
                    break;
                case RowValueWhere addedCompositeWhere:
                    rowValueWhere = rowValueWhere.Add(addedCompositeWhere);
                    break;
                case SearchWhere addedSearchWhere:
                    searchWhere = searchWhere.Add(addedSearchWhere);
                    break;
                default:
                    throw new ArgumentException($"Can't find where statement type {whereStatement.GetType()}");
            }
        }
        var whereStatements = new List<IWhereStatement>();
        if (simpleWhere.Parameters.Any()) {
            whereStatements.Add(simpleWhere);
        }
        if (rowValueWhere.Parameters.Any()){
            whereStatements.Add(rowValueWhere);
        }
        if (searchWhere.Parameters.Any()) {
            whereStatements.Add(searchWhere);
        }
        return whereStatements;
    }
}