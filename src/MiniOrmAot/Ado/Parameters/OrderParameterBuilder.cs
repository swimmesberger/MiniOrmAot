namespace MiniOrmAot.Ado.Parameters; 

public class OrderParameterBuilder {
    private readonly List<OrderParameter> _columns = new();
    public IReadOnlyList<OrderParameter> Columns => _columns;

    public OrderParameterBuilder() { }

    public OrderParameterBuilder(IEnumerable<OrderParameter> columns) {
        _columns = columns.ToList();
    }

    public OrderParameterBuilder Ascending(string name) {
        _columns.Add(new OrderParameter(name));
        return this;
    }

    public OrderParameterBuilder Descending(string name) {
        _columns.Add(new OrderParameter(name, OrderType.Desc));
        return this;
    }
}