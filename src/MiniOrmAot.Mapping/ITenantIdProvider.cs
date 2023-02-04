namespace MiniOrmAot.Mapping; 

public interface ITenantIdProvider<in T> {
    string TenantIdPropertyName { get; }
    object GetTenantId(T record);
}