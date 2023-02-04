using MiniOrmAot.Common;
using MiniOrmAot.Model;

namespace MiniOrmAot.Test;

[GenerateMapper]
public record CartDataModel : DataModel {
    [TenantIdColumn]
    public Guid ShopId { get; init; }
    public Guid? CustomerId { get; init; }
    public DateTimeOffset LastAccess { get; init; } = SystemClock.GetNow();
}