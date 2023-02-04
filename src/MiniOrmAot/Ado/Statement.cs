using MiniOrmAot.Ado.Parameters;
using MiniOrmAot.Ado.Parameters.Where;
using MiniOrmAot.Common;
using MiniOrmAot.Mapping;

namespace MiniOrmAot.Ado;

public record Statement {
    public static readonly Statement Empty = new Statement();

    public StatementType Type { get; init; } = StatementType.Find;
    public string From { get; init; } = string.Empty;
    public StatementParameters Parameters { get; init; } = StatementParameters.Empty;

    private IPropertyMapper PropertyToColumnMapper { get; init; } = EmptyPropertyMapping.EmptyMapper;

    public Statement(StatementType type, IPropertyMapper propertyToColumnMapper) {
        Type = type;
        PropertyToColumnMapper = propertyToColumnMapper;
        From = PropertyToColumnMapper.TypeName;
    }

    private Statement() { }

    public Statement MapToColumnNames() {
        return this with {
            From = PropertyToColumnMapper.MappedTypeName,
            Parameters = Parameters.MapParameterNames(PropertyToColumnMapper.MapName)
        };
    }
}

public record Statement<T> : Statement {
    public RowMapper<T> RowMapper { get; init; }
    
    public Statement(StatementType type, IPropertyMapper propertyToColumnMapper, RowMapper<T> rowMapper) : base(type, propertyToColumnMapper) {
        RowMapper = rowMapper;
    }
    
    public Statement<T> AddWhereParameter(string name, object value) {
        return this with {
            Parameters = Parameters.Add(new StatementParameters {
                WhereParameters = new WhereParameters(name, value)
            })
        };
    }
}