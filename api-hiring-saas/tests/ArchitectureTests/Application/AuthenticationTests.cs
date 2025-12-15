using Application.Features.Authentication.Core;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;

namespace ArchitectureTests.Application;

public class AuthenticationTests : BaseTest
{
    [Fact]
    public void AuthTokenProviders_Should_Have_AuthTokenProviderPostfix()
    {
        ArchRuleDefinition
            .Classes()
            .That()
            .ImplementInterface(typeof(IAuthTokenProvider))
            .Should()
            .HaveNameEndingWith("AuthTokenProvider")
            .Check(Architecture);
    }
}
