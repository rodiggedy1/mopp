using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class PresentationLayerTests : BaseTest
{

    [Fact]
    public void PresentationLayer_Shound_NotHaveDependencyOn_ServiceLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(PresentationLayer)
            .Should()
            .NotDependOnAny(ServiceLayer)
            .Check(Architecture);
    }
}
