using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class ServiceLayerTests : BaseTest
{

    [Fact]
    public void ServiceLayer_Shound_NotHaveDependencyOn_PresentationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(ServiceLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }
}
