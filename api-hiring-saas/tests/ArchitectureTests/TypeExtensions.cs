namespace ArchitectureTests;

public static class TypeExtensions
{
    public static bool IsRecord(this Type type)
    {
        // Check for properties unique to records in C#
        return type.GetProperties().Any(p => p.Name == "EqualityContract");
    }
}
