using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Application;

public class CommandTests: BaseTest
{
    [Fact]
    public void Commands_Should_Reside_In_ApplicationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(ICommandBase))
            .Should()
            .Be(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void Commands_Should_Have_CommandPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ICommandBase))
            .Should()
            .HaveNameEndingWith("Command")
            .Check(Architecture);
    }

    [Fact]
    public void Commands_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ICommandBase))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void Commands_Should_Implement_ICommandBase()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("Command")
            .Should()
            .ImplementInterface(typeof(ICommandBase))
            .Check(Architecture);
    }

    [Fact]
    public void CommandHandlers_Should_Implement_ICommandHandlerBase()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .ImplementInterface(typeof(ICommandHandlerBase))
            .Check(Architecture);
    }

    [Fact]
    public void CommandHandlers_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ICommandHandlerBase))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void CommandHandlers_Should_Have_CommandHandlerPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .ImplementInterface(typeof(ICommandHandlerBase))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .Check(Architecture);
    }
}
