using System.Collections.Immutable;

namespace MiniOrmAot.Ado.Parameters.Where;

// row value where: WHERE (x, y, ...) > (a, b, ...)
public record RowValueWhere(IEnumerable<QueryParameter> Parameters, WhereComparator Comparator) : IWhereStatement {
    public static readonly RowValueWhere Empty = new RowValueWhere(Enumerable.Empty<QueryParameter>(), WhereComparator.Equal);
    
    public RowValueWhere(QueryParameter parameter, WhereComparator comparator) : 
        this(ImmutableArray.Create(parameter), comparator) { }
    
    IWhereStatement IWhereStatement.MapParameterNames(Func<string, string> selector) => MapParameterNames(selector);

    IWhereStatement IWhereStatement.Add(IWhereStatement where) => Add((RowValueWhere)where);
    
    public RowValueWhere Add(RowValueWhere where) {
        if (this == Empty) return where;
        if (where == Empty) return this;
        var parameters = new List<QueryParameter>();
        parameters.AddRange(Parameters);
        parameters.AddRange(where.Parameters);
        return this with { Parameters = parameters };
    }
    
    public RowValueWhere MapParameterNames(Func<string, string> selector) {
        return this with { Parameters = Parameters.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList() };
    }
}