using System.Data.Common;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado.Impl; 

public class DefaultParameterTypeSetter : IDbParameterTypeSetter {
    public void SetParameterType(DbParameter parameter, TypedValue typedValue = default) {
        //if (typedValue.IsJson && parameter is NpgsqlParameter npgsqlParameter) {
        //    npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
        //}
        parameter.DbType = typedValue.DbType ?? DbTypeUtil.GetDbType(typedValue.Value);
    }
}