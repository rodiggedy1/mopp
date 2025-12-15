using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using MediatR;

namespace ArchitectureTests.Application;

public class EventHandlerTests : BaseTest
{
    [Fact]
    public void EventHandlers_Should_Reside_In_ApplicationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Should()
            .Be(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void EventHandlers_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void EventHandlers_Should_Have_EventHandlerPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Should()
            .HaveNameEndingWith("EventHandler")
            .Check(Architecture);
    }

    [Fact]
    public void EventHandlers_Should_Implement_INotificationHandler()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("EventHandler")
            .Should()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Check(Architecture);
    }
}
