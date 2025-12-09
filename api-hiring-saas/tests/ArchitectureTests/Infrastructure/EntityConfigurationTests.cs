using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Infrastructure.Persistence.EntityConfigurations.Base;
using Microsoft.EntityFrameworkCore;

namespace ArchitectureTests.Infrastructure;

public class EntityConfigurationTests : BaseTest
{
    [Fact]
    public void EntityConfigurations_Should_Reside_In_InfrastructureLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(EntityTypeConfiguration<>))
            .Or()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .Be(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void EntityConfigurations_Should_Reside_In_EntityConfigurationsNamespace()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(InfrastructureAssembly)
            .And()
            .AreAssignableTo(typeof(EntityTypeConfiguration<>))
            .Or()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And()
            .AreNotAbstract()
            .Should()
            .ResideInNamespace("Infrastructure.Persistence.EntityConfigurations")
            .Check(Architecture);
    }

    [Fact]
    public void BaseEntityConfigurations_Should_Reside_In_EntityConfigurationsBaseNamespace()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(InfrastructureAssembly)
            .And()
            .AreAssignableTo(typeof(EntityTypeConfiguration<>))
            .Or()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And()
            .AreAbstract()
            .Should()
            .ResideInNamespace("Infrastructure.Persistence.EntityConfigurations.Base")
            .Check(Architecture);
    }

    [Fact]
    public void EntityConfigurations_Should_Have_Configuration_Postfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(InfrastructureAssembly)
            .And()
            .AreAssignableTo(typeof(EntityTypeConfiguration<>))
            .Or()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Configuration")
            .Check(Architecture);
    }
}
