using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using MassTransit;

namespace ArchitectureTests.Service;

public class ConsumerTests : BaseTest
{
    [Fact]
    public void MessageBrokerConsumers_Should_Have_MessageConsumerPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IConsumer))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("MessageConsumer")
            .Check(Architecture);
    }

    [Fact]
    public void MessageBrokerConsumers_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IConsumer))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void MessageBrokerConsumers_Should_Reside_In_ServiceLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IConsumer))
            .Should()
            .Be(ServiceLayer)
            .Check(Architecture);
    }
}
