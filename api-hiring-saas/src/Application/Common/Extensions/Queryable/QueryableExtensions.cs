using Domain.Entities.Base.Interfaces;
using DTO.Enums;
using DTO.Pagination;
using DTO.Sorting;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Common.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, IPaginated paging)
    {
        return source
          .Skip((paging.PageNumber - 1) * paging.PageSize)
          .Take(paging.PageSize);
    }

    public static IQueryable<T> ApplySorting<T, TSort>(this IQueryable<T> source, SortOptions<TSort>? sortOptions)
        where TSort : Enum
    {
        if (sortOptions == null || string.IsNullOrEmpty(sortOptions.Field.ToString()))
        {
            return source;
        }

        ParameterExpression parameter = Expression.Parameter(source.ElementType, "");

        MemberExpression property = Expression.Property(parameter, sortOptions.Field.ToString());
        LambdaExpression lambda = Expression.Lambda(property, parameter);

        string methodName = sortOptions.SortOrder == SortOrder.Asc ? "OrderBy" : "OrderByDescending";

        Expression methodCallExpression = Expression.Call(
                              typeof(Queryable),
                              methodName,
                              new Type[] { typeof(T), property.Type },
                              source.Expression,
                              Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(methodCallExpression);
    }

    public static IQueryable<T> Search<T>(this IQueryable<T> source, string? query, List<string>? fieldsToSkip = null)
    {
        if (string.IsNullOrEmpty(query))
            return source;

        return source.Where(ApplyWhere<T>(query.ToLower(), fieldsToSkip));
    }

    public static IQueryable<T> FilterByStatus<T>(this IQueryable<T> source, List<Status>? statuses)
        where T : class, IWithStatus
    {
        return source.Where(e =>
            statuses == null ||
            statuses.Count() == 0 ||
            statuses.Contains(e.Status));
    }

    private static Expression<Func<T, bool>> ApplyWhere<T>(string query, List<string>? fieldsToSkip = null)
    {
        var stringFields = GetStringProperties<T>(fieldsToSkip);

        // a reference parameter
        var x = Expression.Parameter(typeof(T), "x");

        // contains method
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        Expression<Func<T, bool>>? expression = null;

        foreach (var field in stringFields)
        {
            // reference a field
            var fieldExpression = Expression.Property(x, field);

            // your value
            var valueExpression = Expression.Constant(query);

            var fieldToLowerExpression = Expression.Call(
                fieldExpression,
                typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));

            // call the contains from a property and apply the value
            var containsValueExpression = Expression.Call(fieldToLowerExpression, containsMethod, valueExpression);

            // create your final lambda Expression

            if (expression == null)
            {
                expression = Expression.Lambda<Func<T, bool>>(containsValueExpression, x);
            }
            else
            {
                var secondExpression = Expression.Lambda<Func<T, bool>>(containsValueExpression, x);
                var body = Expression.OrElse(expression.Body, secondExpression.Body);
                expression = Expression.Lambda<Func<T, bool>>(body, expression.Parameters[0]);
            }
        }

        return expression;
    }
    private static List<string> GetStringProperties<T>(List<string>? fieldsToSkip = null)
    {
        List<string> fields = new List<string>();
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo p in properties)
        {
            // Only work with strings
            if (p.PropertyType != typeof(string)) { continue; }

            // Skip field if listed in fieldsToSkip
            if (fieldsToSkip != null && fieldsToSkip.Contains(p.Name)) { continue; }

            // If not writable then cannot null it; if not readable then cannot check it's value
            if (!p.CanRead) { continue; }

            MethodInfo mget = p.GetGetMethod(false);

            // Get and set methods have to be public
            if (mget == null) { continue; }

            fields.Add(p.Name);
        }
        return fields;
    }
}
