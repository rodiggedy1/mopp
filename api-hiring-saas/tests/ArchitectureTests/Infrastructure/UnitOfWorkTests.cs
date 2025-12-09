using Application.Common.Interfaces;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Infrastructure;

public class UnitOfWorkTests : BaseTest
{
    [Fact]
    public void UnitOfWork_Impplementations_Should_Reside_In_InfrastructureLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IUnitOfWork))
            .Should()
            .Be(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void UnitOfWork_Impplementations_Should_Reside_In_PersistenceNamespace()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(InfrastructureAssembly)
            .And()
            .ImplementInterface(typeof(IUnitOfWork))
            .Should()
            .ResideInNamespace("Infrastructure.Persistence")
            .Check(Architecture);
    }

    [Fact]
    public void UnitOfWork_Impplementations_Should_Have_UnitOfWork_Postfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(InfrastructureAssembly)
            .And()
            .ImplementInterface(typeof(IUnitOfWork))
            .Should()
            .HaveNameEndingWith("UnitOfWork")
            .Check(Architecture);
    }
}
