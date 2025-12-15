using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using DTO.MessageBroker;

namespace ArchitectureTests.DTO;

public class MessageTests : BaseTest
{
    [Fact]
    public void Messages_Should_Reside_In_DtoLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(MessageBase))
            .Should()
            .Be(DTOLayer)
            .Check(Architecture);
    }

    [Fact]
    public void Messages_Should_Have_MessagePostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(DTOAssembly)
            .And()
            .AreAssignableTo(typeof(MessageBase))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Message")
            .Check(Architecture);
    }

    [Fact]
    public void Messages_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(DTOAssembly)
            .And()
            .AreAssignableTo(typeof(MessageBase))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .Check(Architecture);
    }
}
