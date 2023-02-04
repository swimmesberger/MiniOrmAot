using System.Data.Common;
using MiniOrmAot.Ado;
using MiniOrmAot.Ado.Impl;
using MiniOrmAot.Model;
using Npgsql;
using NpgsqlTypes;

namespace MiniOrmAot.Npgsql; 

public class NpgsqlDbParameterTypeSetter : IDbParameterTypeSetter {
    private readonly DefaultParameterTypeSetter _defaultSetter;

    public NpgsqlDbParameterTypeSetter() {
        _defaultSetter = new DefaultParameterTypeSetter();
    }

    public void SetParameterType(DbParameter parameter, TypedValue typedValue = default) {
        if (typedValue.IsJson && parameter is NpgsqlParameter npgsqlParameter) {
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Jsonb;
        } else {
            _defaultSetter.SetParameterType(parameter, typedValue);
        }
    }
}