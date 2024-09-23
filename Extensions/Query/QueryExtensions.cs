using System.Linq.Expressions;

namespace API.Extensions.Query
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, string sortBy, bool isDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var parameter = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            return isDescending ? source.OrderByDescending(lambda) : source.OrderBy(lambda);
        }

        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var skipNumber = (pageNumber - 1) * pageSize;
            return source.Skip(skipNumber).Take(pageSize);
        }
    }
}