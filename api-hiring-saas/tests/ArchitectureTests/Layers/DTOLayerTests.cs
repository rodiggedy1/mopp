using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Layers;

public class DTOLayerTests : BaseTest
{
    [Fact]
    public void DTOLayer_Shound_NotHaveDependencyOn_DomainLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DTOLayer)
            .Should()
            .NotDependOnAny(DomainLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DTOLayer_Shound_NotHaveDependencyOn_ApplicationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DTOLayer)
            .Should()
            .NotDependOnAny(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DTOLayer_Shound_NotHaveDependencyOn_InfrastructureLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DTOLayer)
            .Should()
            .NotDependOnAny(InfrastructureLayer)
            .Check(Architecture);
    }

    [Fact]
    public void DTOLayer_Shound_NotHaveDependencyOn_ServiceLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DTOLayer)
            .Should()
            .NotDependOnAny(ServiceLayer)
            .Check(Architecture);
    }


    [Fact]
    public void DTOLayer_Shound_NotHaveDependencyOn_PresentationLayer()
    {
        ArchRuleDefinition
            .Types()
            .That()
            .Are(DTOLayer)
            .Should()
            .NotDependOnAny(PresentationLayer)
            .Check(Architecture);
    }
}
