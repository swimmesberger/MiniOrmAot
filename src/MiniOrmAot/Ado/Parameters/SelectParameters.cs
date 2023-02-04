using System.Collections.Immutable;

namespace MiniOrmAot.Ado.Parameters;

public record SelectParameters(IReadOnlyList<string> Properties, bool IsAll = false) {
    public static readonly SelectParameters All = new SelectParameters(ImmutableArray<string>.Empty, true);
    public static readonly SelectParameters Empty = new SelectParameters(ImmutableArray<string>.Empty, false);

    public bool IsEmpty => Properties.Count <= 0;

    public SelectParameters(params string[] properties) : this(properties, false) { }
    
    public SelectParameters(IEnumerable<string> properties) : this(properties.ToImmutableArray()) { }
    
    public SelectParameters MapParameterNames(Func<string, string> selector) {
        return this with { Properties = Properties.Select(selector).ToImmutableArray() };
    }
    
    public SelectParameters Add(SelectParameters select) {
        if (IsAll && select.IsAll) {
            return All;
        }
        if (!IsAll && select.IsAll) {
            return this;
        }
        return new SelectParameters(Properties.Concat(select.Properties).ToImmutableArray());
    }
}