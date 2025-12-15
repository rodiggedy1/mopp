using System.Reflection;

namespace Application.Common.Helpers;

public static class AssemblyHelper
{
    public static Assembly[] Assemblies => AppDomain.CurrentDomain.GetAssemblies()
        .Where(a => a.GetName().Name!.Contains("Liberty"))
        .ToArray();
}
