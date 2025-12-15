using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Domain.Events;

namespace ArchitectureTests.Domain;

public class DomainEventTests: BaseTest
{
    [Fact]
    public void DomainEvents_Should_Reside_In_DomainLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .Be(DomainLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(DomainAssembly)
            .And()
            .ImplementInterface(typeof(IDomainEvent))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void DomainEvents_Should_HaveEventPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(DomainAssembly)
            .And()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("Event")
            .Check(Architecture);
    }
}
