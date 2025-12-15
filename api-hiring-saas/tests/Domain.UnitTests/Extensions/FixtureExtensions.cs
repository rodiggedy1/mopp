using AutoFixture;

namespace Domain.UnitTests.Extensions;

public static class FixtureExtensions
{
    public static void DisableCircularReferenceException(this Fixture fixture)
    {
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
    }
}
