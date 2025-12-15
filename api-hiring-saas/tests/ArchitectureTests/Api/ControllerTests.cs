using Api.Controllers;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Api;

public class ControllerTests : BaseTest
{
    [Fact]
    public void Controllers_Should_Have_ControllerPosftix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(ApiControllerBase))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Controller")
            .Check(Architecture);
    }

    [Fact]
    public void Controllers_Should_Reside_In_PresentationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(ApiControllerBase))
            .Should()
            .Be(PresentationLayer)
            .Check(Architecture);
    }
}
