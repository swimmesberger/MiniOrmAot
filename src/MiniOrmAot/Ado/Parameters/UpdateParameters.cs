namespace MiniOrmAot.Ado.Parameters; 

public record UpdateParameters {
    public static readonly UpdateParameters Empty = new UpdateParameters();
    
    public IEnumerable<QueryParameter> Values { get; init; } = Enumerable.Empty<QueryParameter>();
    
    public UpdateParameters MapParameterNames(Func<string, string> selector) {
        return new UpdateParameters() {
            Values = Values.Select(p => p with { Name = selector.Invoke(p.Name) }).ToList()
        };
    }

    public UpdateParameters Add(UpdateParameters parameters) {
        var values = new List<QueryParameter>();
        values.AddRange(Values);
        values.AddRange(parameters.Values);
        return parameters with {
            Values = values
        };
    }
}