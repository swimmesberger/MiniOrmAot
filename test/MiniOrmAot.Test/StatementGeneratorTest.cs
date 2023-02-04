using FluentAssertions;
using MiniOrmAot.Ado;
using MiniOrmAot.Ado.Impl;
using MiniOrmAot.Ado.Parameters;
using MiniOrmAot.Ado.Parameters.Where;
using MiniOrmAot.Gen;
using Xunit.Abstractions;

namespace MiniOrmAot.Test; 

public class StatementGeneratorTest {
    private static readonly Guid CustomerId = new Guid("AB5BEC83-2BB6-4F3C-860A-AEE56A0415AA");
    
    private readonly ITestOutputHelper _testOutputHelper;
    
    public StatementGeneratorTest(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CreateFind() {
        var generator = new AdoStatementGenerator<CartDataModel>(new CartDataRecordMapper());
        IStatementMaterializer materializer = new AdoStatementMaterializer();
        var statement = generator.CreateFind(new StatementParameters() {
            Select = new[] { nameof(CartDataModel.Id), nameof(CartDataModel.CustomerId) },
            Where = new QueryParameter[] { 
                new(nameof(CartDataModel.CustomerId), CustomerId), 
                new(nameof(CartDataModel.LastAccess), WhereComparator.GreaterOrEqual, DateTimeOffset.Now)
            },
            OrderBy = new OrderParameter[] { new(nameof(CartDataModel.CreationTime)) },
            Limit = 1000
        });
        var sqlStatement = materializer.MaterializeStatement(statement);
        _testOutputHelper.WriteLine(sqlStatement.Sql);
        sqlStatement.Sql.Should().BeEquivalentTo(
            "SELECT id,customer_id FROM \"cart\" WHERE customer_id = @CustomerId_0 AND last_access >= @LastAccess_1 ORDER BY creation_time ASC LIMIT @_Limit_0");
    }
}