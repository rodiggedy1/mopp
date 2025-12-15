using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Application;

public class QueryTests: BaseTest
{
    [Fact]
    public void Queries_Should_Reside_In_ApplicationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IQueryBase))
            .Should()
            .Be(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void Queries_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(IQueryBase))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void QueryHandlers_Should_Reside_In_ApplicationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IQueryHandlerBase))
            .Should()
            .Be(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void QueryHandlers_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(IQueryHandlerBase))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }


    [Fact]
    public void Queries_Should_Have_QueryPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(IQueryBase))
            .Should()
            .HaveNameEndingWith("Query")
            .Check(Architecture);
    }

    [Fact]
    public void Queries_Should_Implement_IQueryBase()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("Query")
            .Should()
            .ImplementInterface(typeof(IQueryBase))
            .Check(Architecture);
    }

    [Fact]
    public void QueryHandlers_Should_Implement_IQueryHandlerBase()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .ImplementInterface(typeof(IQueryHandlerBase))
            .Check(Architecture);
    }

    [Fact]
    public void QueryHandlers_Should_Have_QueryHandlerPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(IQueryHandlerBase))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .Check(Architecture);
    }
}
