using MiniOrmAot.Ado.Parameters.Where;

namespace MiniOrmAot.Ado.Parameters;

public record StatementParameters {
    public static readonly StatementParameters Empty = new StatementParameters();
    
    public SelectParameters SelectParameters { get; init; } = SelectParameters.All;
    public IEnumerable<string> Select {
        get => SelectParameters.Properties;
        init => SelectParameters = new SelectParameters(value);
    }
    
    public WhereParameters WhereParameters { get; init; } = WhereParameters.Empty;
    public IEnumerable<QueryParameter> Where {
        get => WhereParameters.Parameters;
        init => WhereParameters = new WhereParameters(value);
    }
    
    public OrderParameters OrderParameters { get; init; } = OrderParameters.Empty;
    public IEnumerable<OrderParameter> OrderBy {
        get => OrderParameters.Parameters;
        init => OrderParameters = new OrderParameters(value);
    }

    public InsertParameters InsertParameters { get; init; } = InsertParameters.Empty;
    public IEnumerable<QueryParameter> Insert {
        get => InsertParameters.Parameters;
        init => InsertParameters = new InsertParameters(value);
    }

    public UpdateParameters Update { get; init; } = UpdateParameters.Empty;
    public long? Limit { get; init; }

    public StatementParameters Add(StatementParameters parameters) {
        var orderBy = new List<OrderParameter>();
        orderBy.AddRange(OrderBy);
        orderBy.AddRange(parameters.OrderBy);
        return this with {
            SelectParameters = SelectParameters.Add(parameters.SelectParameters),
            WhereParameters = WhereParameters.Add(parameters.WhereParameters),
            OrderBy = orderBy,
            InsertParameters = InsertParameters.Add(parameters.InsertParameters),
            Update = Update.Add(parameters.Update),
            Limit = Limit == null && parameters.Limit == null ? null : Math.Min(Limit ?? int.MaxValue, parameters.Limit ?? int.MaxValue)
        };
    }

    public StatementParameters WithSelect(params string[] properties) {
        return Add(new StatementParameters() { SelectParameters = new SelectParameters(properties) });
    }
    
    public StatementParameters WithSelect(IEnumerable<string> properties) {
        return Add(new StatementParameters() { SelectParameters = new SelectParameters(properties) });
    }

    public StatementParameters WithWhere(string name, object value) {
        return WithWhere(new QueryParameter(name, value));
    }
    
    public StatementParameters WithWhere(QueryParameter parameter) {
        return WithWhere(new[] { parameter });
    }
    
    public StatementParameters WithWhere(IEnumerable<QueryParameter> parameters) {
        return Add(new StatementParameters() { WhereParameters = new WhereParameters(parameters) });
    }
    
    public StatementParameters WithWhere(IWhereStatement whereStatement) {
        return Add(new StatementParameters() { WhereParameters = new WhereParameters(whereStatement) });
    }
    
    public StatementParameters WithOrderBy(string name, OrderType orderType = OrderType.Asc) {
        return WithOrderBy(new OrderParameter(name, orderType));
    }
    
    public StatementParameters WithOrderBy(OrderParameter parameter) {
        return WithOrderBy(new[] { parameter });
    }
    
    public StatementParameters WithOrderBy(IEnumerable<OrderParameter> parameters) {
        return Add(new StatementParameters() { OrderBy = parameters });
    }
    
    public StatementParameters WithOrderBy(OrderParameters parameters) {
        return Add(new StatementParameters() { OrderParameters = parameters });
    }
    
    public StatementParameters WithLimit(long limit) {
        return Add(new StatementParameters() { Limit = limit });
    }
    
    public StatementParameters MapParameterNames(Func<string, string> selector) {
        return this with {
            SelectParameters = SelectParameters.MapParameterNames(selector),
            WhereParameters = WhereParameters.MapParameterNames(selector),
            OrderBy = OrderBy.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList(),
            InsertParameters = InsertParameters.MapParameterNames(selector),
            Update = Update.MapParameterNames(selector),
            Limit = Limit
        };
    }

    public static StatementParameters CreateWhere(string name, object value) => Empty.WithWhere(name, value);
}