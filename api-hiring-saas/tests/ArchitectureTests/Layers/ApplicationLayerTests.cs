using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class ApplicationLayerTests : BaseTest
{
    [Fact]
    public void ApplicationLayer_Shound_NotHaveDependencyOn_InfrastructureLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(ApplicationLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void ApplicationLayer_Shound_NotHaveDependencyOn_PresentationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(ApplicationLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void ApplicationLayer_Shound_NotHaveDependencyOn_ServiceLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(ApplicationLayer)
            .Should()
            .NotDependOnAny(ServiceLayer)
            .Check(Architecture);
    }
}
