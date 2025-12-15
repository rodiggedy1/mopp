using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using AutoMapper;

namespace ArchitectureTests.Application;

public class MapperTests : BaseTest
{
    [Fact]
    public void MapperProfiles_Should_Reside_In_ApplicationLayer()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .AreAssignableTo(typeof(Profile))
            .Should()
            .Be(ApplicationLayer)
            .Check(Architecture);
    }

    [Fact]
    public void MapperProfiles_Should_BeSealed()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .AreAssignableTo(typeof(Profile))
            .Should()
            .BeSealed()
            .Check(Architecture);
    }

    [Fact]
    public void MapperProfiles_Should_Inherit_From_Profile()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .HaveNameEndingWith("MapperProfile")
            .Should()
            .BeAssignableTo(typeof(Profile))
            .Check(Architecture);
    }

    [Fact]
    public void MapperProfiles_Should_Have_MapperProfilePostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ResideInAssembly(ApplicationAssembly)
            .And()
            .AreAssignableTo(typeof(Profile))
            .Should()
            .HaveNameEndingWith("MapperProfile")
            .Check(Architecture);
    }

       //[Fact]
    //public void MapperProfiles_Should_Have_PublicParameterlessConctructor()
    //{
    //    IEnumerable<Class> mapperProfileTypes = ArchRuleDefinition
    //        .Classes()
    //        .That()
    //        .ResideInAssembly(ApplicationAssembly)
    //        .And()
    //        .AreAssignableTo(typeof(Profile))
    //        .GetObjects(Architecture);

    //    var failingMapperProfiles = new List<Class>();

    //    foreach (var mapperProfileType in mapperProfileTypes)
    //    {
    //        IEnumerable<MethodMember> constructors = mapperProfileType.GetConstructors();

    //        if (!constructors.Any(c => c.Visibility == Visibility.Public && !c.Parameters.Any()))
    //        {
    //            failingMapperProfiles.Add(mapperProfileType);
    //        }
    //    }

    //    failingMapperProfiles.Should().BeEmpty();
    //}
}
