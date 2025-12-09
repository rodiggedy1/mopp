using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class DomainLayerTests : BaseTest
{
    [Fact]
    public void DomainLayer_Shound_NotHaveDependencyOn_ApplicationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_Shound_NotHaveDependencyOn_InfrastructureLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_Shound_NotHaveDependencyOn_PresentationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DomainLayer_Shound_NotHaveDependencyOn_ServiceLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DomainLayer)
            .Should()
            .NotDependOnAny(ServiceLayer)
            .Check(Architecture);
    }
}
