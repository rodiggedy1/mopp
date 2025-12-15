using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Domain.Entities.Base;
using FluentAssertions;

namespace ArchitectureTests.Domain;

public class EntityTests: BaseTest
{
    [Fact]
    public void Entites_Should_Reside_In_DomainLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(BaseEntity))
            .Or()
            .AreAssignableTo(typeof(BaseAuditableEntity))
            .Should()
            .Be(DomainLayer)
            .Check(Architecture);
    }

    [Fact]
    public void Entites_Should_HavePrivateParameterlessConctructor()
    {
        IEnumerable<Class> entityTypes = ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(DomainAssembly)
            .And()
            .AreAssignableTo(typeof(BaseEntity))
            .Or()
            .AreAssignableTo(typeof(BaseAuditableEntity))
            .And()
            .AreNotAbstract()
            .GetObjects(Architecture);

        var failingEntites = new List<Class>();

        foreach (var entityType in entityTypes)
        {
            IEnumerable<MethodMember> constructors = entityType.GetConstructors();

            if (!constructors.Any(c => c.Visibility == Visibility.Private && !c.Parameters.Any()))
            {
                failingEntites.Add(entityType);
            }
        }

        failingEntites.Should().BeEmpty();
    }
}
