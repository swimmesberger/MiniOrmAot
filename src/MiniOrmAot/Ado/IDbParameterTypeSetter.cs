using System.Data.Common;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado; 

public interface IDbParameterTypeSetter {
    void SetParameterType(DbParameter parameter, TypedValue typedValue = default);
}