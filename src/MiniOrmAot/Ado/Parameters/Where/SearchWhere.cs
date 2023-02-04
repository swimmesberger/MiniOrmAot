using System.Collections.Immutable;

namespace MiniOrmAot.Ado.Parameters.Where; 

// row value where: WHERE LOWER(x) LIKE %LOWER(a)%
public record SearchWhere(IEnumerable<QueryParameter> Parameters) : IWhereStatement {
    public static readonly SearchWhere Empty = new SearchWhere(Enumerable.Empty<QueryParameter>());
    
    public SearchWhere(QueryParameter parameter) : this(ImmutableArray.Create(parameter)) { }
    
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((SearchWhere)where);
    
    public SearchWhere Add(SearchWhere where) {
        var parameters = new List<QueryParameter>();
        parameters.AddRange(Parameters);
        parameters.AddRange(where.Parameters);
        return new SearchWhere(Parameters: parameters);
    }
    
    public SearchWhere MapParameterNames(Func<string, string> selector) {
        return new SearchWhere(Parameters.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList());
    }
}