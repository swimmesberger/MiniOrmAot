using System.Collections;
using System.Text;
using MiniOrmAot.Ado.Parameters;
using MiniOrmAot.Ado.Parameters.Where;
using MiniOrmAot.Common;
using MiniOrmAot.Model;

namespace MiniOrmAot.Ado.Impl; 

public class AdoStatementMaterializer : IStatementMaterializer {
    private const string LimitParamName = "_Limit_0";

    public MaterializedStatements MaterializeBatch(StatementBatch statement) {
        return new MaterializedStatements(statement.Statements.Select(MaterializeStatement));
    }

    public MaterializedStatement<T> MaterializeStatement<T>(Statement<T> statement) {
        var materializedStatement = MaterializeStatement((Statement)statement);
        return new MaterializedStatement<T>(materializedStatement, statement.RowMapper);
    }
    
    private MaterializedStatement MaterializeStatement(Statement statement) {
        if (statement == Statement.Empty) return MaterializedStatement.Empty;
        // map all parameters to column name schema
        statement = statement.MapToColumnNames();
        return statement.Type switch {
            StatementType.Count => MaterializeCount(statement),
            StatementType.Create => MaterializeInsert(statement),
            StatementType.Delete => MaterializeDelete(statement),
            StatementType.Find => MaterializeFind(statement),
            StatementType.Update => MaterializeUpdate(statement),
            _ => throw new ArgumentException()
        };
    }

    private MaterializedStatement MaterializeInsert(Statement statement) {
        var sql = $"INSERT INTO {statement.GetTableName()} ({statement.GetInsertColumns()}) VALUES {statement.GetInsertParameters()}";
        var parameters = statement.Parameters.InsertParameters.Values.SelectMany((values, idx) => {
            return values.Select(q => q with { ParameterName = $"{q.ParameterName}_{idx}" });
        });
        return new MaterializedStatement(sql) {
            Parameters = parameters
        };
    }
    
    private MaterializedStatement MaterializeUpdate(Statement statement) {
        var sql = new StringBuilder("UPDATE");
        sql.Append(' ').Append(statement.GetTableName());
        sql.Append(" SET");
        var first = true;
        foreach (var parameter in statement.Parameters.Update.Values) {
            if (first) {
                first = false;
            } else {
                sql.Append(',');
            }
            sql.Append(' ').Append(parameter.Name).Append(" = ").Append('@').Append(parameter.ParameterName);
        }
        var parameters = new List<QueryParameter>();
        parameters.AddRange(statement.Parameters.Update.Values);
        var addWhereResult = AddWhereClause(sql, parameters, statement.Parameters.WhereParameters);
        if (addWhereResult == AddStatementResult.EmptyResult) return MaterializedStatement.Empty;
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private MaterializedStatement MaterializeDelete(Statement statement) {
        var sql = new StringBuilder($"DELETE FROM {statement.GetTableName()}");
        var parameters = new List<QueryParameter>();
        var addWhereResult = AddWhereClause(sql, parameters, statement.Parameters.WhereParameters);
        if (addWhereResult == AddStatementResult.EmptyResult) return MaterializedStatement.Empty;
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }
    
    private MaterializedStatement MaterializeFind(Statement statement) {
        var sql = new StringBuilder($"SELECT {statement.GetSelect()} FROM {statement.GetTableName()}");
        var parameters = new List<QueryParameter>();
        var addWhereResult = AddWhereClause(sql, parameters, statement.Parameters.WhereParameters);
        if (addWhereResult == AddStatementResult.EmptyResult) return MaterializedStatement.Empty;
        AddOrderByClause(sql, statement.Parameters.OrderBy);
        if (statement.Parameters.Limit != null) {
            sql.Append($" LIMIT @{LimitParamName}");
            parameters.Add(new QueryParameter(LimitParamName, statement.Parameters.Limit.Value));
        }
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private MaterializedStatement MaterializeCount(Statement statement) {
        var sql = new StringBuilder($"SELECT COUNT(*) FROM {statement.GetTableName()}");
        var parameters = new List<QueryParameter>();
        var addResult = AddWhereClause(sql, parameters, statement.Parameters.WhereParameters);
        if (addResult == AddStatementResult.EmptyResult) return MaterializedStatement.Empty;
        return new MaterializedStatement(sql.ToString()) {
            Parameters = parameters
        };
    }

    private AddStatementResult AddWhereClause(StringBuilder sql, List<QueryParameter> newParams, WhereParameters whereParameters)
        => AddWhereClause(sql, newParams, whereParameters.Statements);

    private AddStatementResult AddWhereClause(StringBuilder sql, List<QueryParameter> newParams, IEnumerable<IWhereStatement> whereStatements) {
        var addResult = AddStatementResult.Added;
        var first = true;
        foreach (var whereStatement in whereStatements) {
            switch (whereStatement) {
                case SimpleWhere simpleWhere:
                    addResult = AddSimpleWhereClause(sql, newParams, simpleWhere.Parameters, ref first);
                    break;
                case RowValueWhere rowValueWhere:
                    addResult = AddRowValueWhereClause(sql, newParams, rowValueWhere, ref first);
                    break;
                case SearchWhere searchWhere:
                    addResult = AddSearchWhereClause(sql, newParams, searchWhere, ref first);
                    break;
                default:
                    throw new ArgumentException($"Unsupported where clause '{whereStatement.GetType()}'");
            }
            if (addResult == AddStatementResult.EmptyResult) return AddStatementResult.EmptyResult;
        }
        return addResult;
    }
    
    private AddStatementResult AddRowValueWhereClause(StringBuilder sql, List<QueryParameter> newParams, RowValueWhere rowValueWhere, ref bool first) {
        if (!rowValueWhere.Parameters.Any()) {
            return AddStatementResult.Added;
        }
        AddAndOrWhere(sql, ref first);
        sql.Append(" (");
        sql.Append(string.Join(',', rowValueWhere.Parameters.Select(p => $"{p.Name}")));
        sql.Append(')');
        sql.Append(ComparatorToSqlOperator(rowValueWhere.Comparator));
        sql.Append(" (");
        sql.Append(string.Join(',', rowValueWhere.Parameters.Select(p => $"@{p.ParameterName}")));
        sql.Append(')');
        newParams.AddRange(rowValueWhere.Parameters);
        return AddStatementResult.Added;
    }

    private AddStatementResult AddSimpleWhereClause(StringBuilder sql, List<QueryParameter> newParams, IEnumerable<QueryParameter> parameters, ref bool first) {
        parameters = PreprocessParameters(parameters);
        var addResult = AddStatementResult.Added;
        foreach (var queryParameter in parameters) {
            var result = TryAddInClause(sql, newParams, queryParameter, ref first);
            if (result == AddStatementResult.EmptyResult) {
                return AddStatementResult.EmptyResult;
            }
            if (result == AddStatementResult.Skipped) {
                AddComparisonClause(sql, newParams, queryParameter, ref first);
            }
        }
        return addResult;
    }

    private void AddAndOrWhere(StringBuilder sql, ref bool first) {
        if (first) {
            first = false;
            sql.Append(" WHERE");
        } else {
            sql.Append(" AND");
        }
    }

    private void AddComparisonClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter parameter, ref bool first) {
        AddAndOrWhere(sql, ref first);
        if (parameter.Value == null) {
            sql.Append($" {parameter.Name} IS NULL");
        } else {
            sql.Append($" {parameter.Name} {ComparatorToSqlOperator(parameter.Comparator)} @{parameter.ParameterName}");
            newParams.Add(parameter);
        }
    }

    private AddStatementResult TryAddInClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter queryParameter, ref bool first) {
        if (queryParameter.Value is string)
            return AddStatementResult.Skipped;
        if(queryParameter.Value is not IEnumerable enumerable)
            return AddStatementResult.Skipped;
        var inParameters = CreateInParameters(queryParameter, enumerable).ToList();
        if (inParameters.Count <= 0) {
            return AddStatementResult.EmptyResult;
        }

        if (inParameters.Count == 1) {
            queryParameter = inParameters[0];
            AddComparisonClause(sql, newParams, queryParameter, ref first);
            return AddStatementResult.Added;
        }
        AddAndOrWhere(sql, ref first);
        var inParamList = string.Join(',', inParameters.Select(p => $"@{p.ParameterName}"));
        sql.Append($" {queryParameter.Name} IN({inParamList})");
        newParams.AddRange(inParameters);
        return AddStatementResult.Added;
    }

    private AddStatementResult AddSearchWhereClause(StringBuilder sql, List<QueryParameter> newParams, SearchWhere searchWhere, ref bool first) {
        var parameters = searchWhere.Parameters.ToList();
        if (!parameters.Any()) {
            return AddStatementResult.Added;
        }
        AddAndOrWhere(sql, ref first);
        if (parameters.Count == 1) {
            AddLikeClause(sql, newParams, parameters[0]);
            return AddStatementResult.Added;
        }
        sql.Append(' ').Append('(');
        var innerFirst = true;
        foreach (var queryParameter in parameters) {
            if (queryParameter.Value == null) {
                throw new MiniOrmDbException($"Null parameter {queryParameter.Name} not allowed in search");
            }
            if (innerFirst) {
                innerFirst = false;
            } else {
                sql.Append(" OR ");
            }
            AddLikeClause(sql, newParams, queryParameter);
        }
        sql.Append(')');
        return AddStatementResult.Added;
    }

    private void AddLikeClause(StringBuilder sql, List<QueryParameter> newParams, QueryParameter queryParameter) {
        sql.Append($"LOWER({queryParameter.Name}) LIKE @{queryParameter.ParameterName}");
        newParams.Add(queryParameter with { Value = $"%{queryParameter.Value!.ToString()!.ToLowerInvariant()}%" });
    }

    private void AddOrderByClause(StringBuilder sql, IEnumerable<OrderParameter> parameters) {
        var first = true;
        var firstOrder = true;
        foreach (var parameter in parameters) {
            if (first) {
                sql.Append(" ORDER BY");
                first = false;
            }
            if (firstOrder) {
                firstOrder = false;
            } else {
                sql.Append(',');
            }
            sql.Append($" {parameter.Name} {OrderTypeToSql(parameter.OrderType)}");
        }
    }

    private static string ComparatorToSqlOperator(WhereComparator comparator) {
        switch (comparator) {
            case WhereComparator.Equal:
                return "=";
            case WhereComparator.Greater:
                return ">";
            case WhereComparator.Less:
                return "<";
            case WhereComparator.GreaterOrEqual:
                return ">=";
            case WhereComparator.LessOrEqual:
                return "<=";
            default:
                throw new ArgumentException($"Operator '{comparator}' is invalid.");
        }
    }

    private static string OrderTypeToSql(OrderType orderType) {
        return orderType switch {
            OrderType.Asc => "ASC",
            OrderType.Desc => "DESC",
            _ => throw new ArgumentException()
        };
    }
    

    private static IEnumerable<QueryParameter> CreateInParameters(QueryParameter queryParameter, IEnumerable enumerable) {
        return enumerable.OfType<object>().Select((value, idx) => queryParameter with {
            Value = value, 
            ParameterName = $"{queryParameter.ParameterName}_{idx}"
        });
    }

    private static IEnumerable<QueryParameter> PreprocessParameters(IEnumerable<QueryParameter> parameters) {
        return parameters.Select((queryParameter, parameterIdx) => 
            queryParameter with { ParameterName = $"{queryParameter.ParameterName}_{parameterIdx}" });
    }

    private enum AddStatementResult {
        Added,
        Skipped,
        EmptyResult // when we detected that the sql would lead to a empty result
    }
}

internal static class StatementExtensions {
    public static string GetTableName(this Statement statement) => $"\"{statement.From}\"" ;

    public static string GetSelect(this Statement statement) {
        if (statement.Parameters.SelectParameters.IsEmpty) {
            return "1"; // SELECT 1 WHEN EMPTY
        }
        return GetColumnNamesString(statement.Parameters.SelectParameters.Properties);
    }

    public static string GetInsertColumns(this Statement statement) => GetColumnNamesString(statement.Parameters.InsertParameters.ColumnNames);
    
    public static string GetInsertParameters(this Statement statement) {
        return string.Join(',', statement.Parameters.InsertParameters.Values.Select((values, idx) =>
            "(" + GetColumnNameParametersString(values.Select(v => $"{v.ParameterName}_{idx}")) + ")"));
    }
    
    public static string GetColumnNamesString(IEnumerable<string> columnNames) => string.Join(',', columnNames);
    
    public static string GetColumnNameParametersString(IEnumerable<string> columnNames) => string.Join(',', columnNames.Select(s => $"@{s}"));
}