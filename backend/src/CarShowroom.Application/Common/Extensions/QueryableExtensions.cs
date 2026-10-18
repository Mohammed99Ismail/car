using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using CarShowroom.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Common.Extensions;

public static class QueryableExtensions
{
    private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> PropertyCache = new();

    /// <summary>
    /// Applies ordering then paging directly on the source entity query, then projects to the destination DTO.
    /// This ensures SQL translation always succeeds and only the requested page of rows is fetched.
    /// </summary>
    public static async Task<PagedListModel<TDestination>> ToPagedListAsync<TSource, TDestination>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TDestination>> projection,
        QueryParameters parameters,
        string defaultOrder,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByProperty(parameters.OrderName, parameters.IsDescending, defaultOrder)
            .Skip(parameters.Skip)
            .Take(parameters.Limit)
            .Select(projection)
            .ToListAsync(cancellationToken);

        return new PagedListModel<TDestination>
        {
            Data = items,
            Length = count,
            Page = parameters.Page,
            Limit = parameters.Limit
        };
    }

    public static async Task<PagedListModel<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        QueryParameters parameters,
        string defaultOrder,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByProperty(parameters.OrderName, parameters.IsDescending, defaultOrder)
            .Skip(parameters.Skip)
            .Take(parameters.Limit)
            .ToListAsync(cancellationToken);

        return new PagedListModel<T>
        {
            Data = items,
            Length = count,
            Page = parameters.Page,
            Limit = parameters.Limit
        };
    }

    /// <summary>
    /// Orders by a public property name of <typeparamref name="T"/>. Unknown names fall back to <paramref name="defaultOrder"/>,
    /// so user input can never inject arbitrary expressions.
    /// </summary>
    public static IOrderedQueryable<T> OrderByProperty<T>(
        this IQueryable<T> source, string? propertyName, bool descending, string defaultOrder)
    {
        var property = Resolve(typeof(T), propertyName) ?? Resolve(typeof(T), defaultOrder)
            ?? throw new InvalidOperationException($"Default order '{defaultOrder}' is not a property of {typeof(T).Name}.");

        var parameter = Expression.Parameter(typeof(T), "x");
        var body = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(body, parameter);

        var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var call = Expression.Call(typeof(Queryable), method, [typeof(T), property.PropertyType],
            source.Expression, Expression.Quote(lambda));

        return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
    }

    private static PropertyInfo? Resolve(Type type, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return PropertyCache.GetOrAdd((type, name), key =>
            key.Item1.GetProperty(key.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
    }
}
