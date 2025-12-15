using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using FluentAssertions;
using FluentValidation;

namespace ArchitectureTests.Application;

public class ValidatorTests: BaseTest
{

    [Fact]
    public void Validators_Should_Have_ValidatorPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .AreAssignableTo(typeof(AbstractValidator<>))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Validator")
            .Check(Architecture);
    }

    [Fact]
    public void Validators_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .AreAssignableTo(typeof(AbstractValidator<>))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void Validators_Should_Have_PublicConctructor()
    {
        IEnumerable<Class> validatorTypes = ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .AreAssignableTo(typeof(AbstractValidator<>))
            .And()
            .AreNotAbstract()
            .GetObjects(Architecture);

        var failingValidators = new List<Class>();

        foreach (var validatorType in validatorTypes)
        {
            IEnumerable<MethodMember> constructors = validatorType.GetConstructors();

            if (!constructors.Any(c => c.Visibility == Visibility.Public))
            {
                failingValidators.Add(validatorType);
            }
        }

        failingValidators.Should().BeEmpty();
    }
}
