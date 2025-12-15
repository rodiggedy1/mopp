using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Infrastructure.TaskScheduler;

namespace ArchitectureTests.Service;

public class ScheduledTaskTests : BaseTest
{
    [Fact]
    public void ScheduledTasks_Should_Have_ScheduledTaskPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(ScheduledTaskBase))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("ScheduledTask")
            .Check(Architecture);
    }
}
