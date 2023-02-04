using System.Collections.Immutable;

namespace MiniOrmAot.Ado.Parameters;

public record OrderParameters(IEnumerable<OrderParameter> Parameters) {
    public static readonly OrderParameters Empty = new OrderParameters(ImmutableArray<OrderParameter>.Empty);

    public OrderParameters(OrderParameter parameter) : this(ImmutableArray.Create(parameter)) { }
    
    public OrderParameters(string name, OrderType orderType = OrderType.Asc) : this(new OrderParameter(name, orderType)) { }
    
    public OrderParameters Add(OrderParameters parameters) {
        var values = new List<OrderParameter>();
        values.AddRange(Parameters);
        values.AddRange(parameters.Parameters);
        return new OrderParameters(values);
    }
}