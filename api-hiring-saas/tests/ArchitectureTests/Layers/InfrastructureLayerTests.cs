using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class InfrastructureLayerTests : BaseTest
{
    [Fact]
    public void InfrastructureLayer_Shound_NotHaveDependencyOn_ServiceLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(InfrastructureLayer)
            .Should()
            .NotDependOnAny(ServiceLayer)
            .Check(Architecture);
    }
    [Fact]
    public void InfrastructureLayer_Shound_NotHaveDependencyOn_PresentationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(InfrastructureLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }
}
