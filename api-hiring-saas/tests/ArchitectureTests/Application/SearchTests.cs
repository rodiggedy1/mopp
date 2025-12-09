using Application.Common.Search;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Application;

public class SearchTests : BaseTest
{
    [Fact]
    public void Searchable_Classes_Should_Have_SearchablePostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ISearchable))
            .Should()
            .HaveNameEndingWith("Searchable")
            .Check(Architecture);
    }

    [Fact]
    public void Searchable_Classes_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ISearchable))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void FullSearchCriteria_Interfaces_Should_Have_FullSearchCriteriaPostfix()
    {
        ArchRuleDefinition
            .Interfaces()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(IFullSearchCriteria<>))
            .Should()
            .HaveNameEndingWith("FullSearchCriteria")
            .Check(Architecture);
    }
}
