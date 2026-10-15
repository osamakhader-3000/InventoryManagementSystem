using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagementSystemDataAccess.Persistence.Extensions
{
    public static class QueryableSearchExtensions
    {
        // Case-insensitive "contains" using PostgreSQL ILIKE; no-op when term is empty
        public static IQueryable<T> WhereContainsIgnoreCase<T>(
            this IQueryable<T> query,
            Expression<Func<T, string>> selector,
            string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return query;

            var pattern = $"%{EscapeLike(term.Trim())}%";
            Expression<Func<string>> patternAccessor = () => pattern; // keeps it a SQL parameter

            var iLike = typeof(NpgsqlDbFunctionsExtensions).GetMethod(
                nameof(NpgsqlDbFunctionsExtensions.ILike),
                new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

            var body = Expression.Call(
                iLike,
                Expression.Constant(EF.Functions),
                selector.Body,
                patternAccessor.Body);

            return query.Where(Expression.Lambda<Func<T, bool>>(body, selector.Parameters));
        }

        // Escape LIKE wildcards so user input like "50%" or "a_b" is matched literally
        private static string EscapeLike(string value) =>
            value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
    }
}
