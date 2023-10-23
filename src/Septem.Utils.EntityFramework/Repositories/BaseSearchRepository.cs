using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Septem.Utils.Domain;
using Septem.Utils.EntityFramework.Entities;
using Septem.Utils.Helpers.ActionInvoke.Collection;
using Septem.Utils.Helpers.ActionInvoke.Collection.Enums;

namespace Septem.Utils.EntityFramework.Repositories;

public class BaseSearchRepository<TEntity, TContext, TSearch> : BaseRepository<TEntity, TContext>
    where TSearch : CollectionQuery
    where TEntity : BaseEntity, new()
    where TContext : DbContext
{
    private readonly ICollection<Func<TSearch, SearchPropertyOption>> _searchExpressions = new List<Func<TSearch, SearchPropertyOption>>();
    private readonly ICollection<KeyValuePair<int, MemberExpression>> _memberExpressions = new List<KeyValuePair<int, MemberExpression>>();
    private readonly ICollection<MemberExpression> _implicitMemberExpressions = new List<MemberExpression>();

    public BaseSearchRepository(TContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<bool> ExistsAsync(TSearch search, CancellationToken cancellationToken) =>
        await GetSearchQuery(search).AnyAsync(cancellationToken);

    public async Task<ICollection<TDomain>> SearchAsync<TDomain>(TSearch search, CancellationToken cancellationToken) where TDomain : BaseDomain
    {
        var query = GetSearchQuery(search);
        var arrayAsync = await query.ProjectTo<TDomain>(Mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);
        return arrayAsync;
    }

    public async Task<TDomain> SearchFirstAsync<TDomain>(TSearch search, CancellationToken cancellationToken) where TDomain : BaseDomain
    {
        var query = GetSearchQuery(search);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        return Mapper.Map<TDomain>(entity);
    }

    public void ForImplicitSearch<TMember>(Expression<Func<TEntity, TMember>> entityExpr)
    {
        if (!(entityExpr.Body is MemberExpression body))
            return;
        _implicitMemberExpressions.Add(body);
    }

    public void ForSearchMember<TMember>(Func<TSearch, SearchPropertyOption> searchExpr, params Expression<Func<TEntity, TMember>>[] entityExpressions)
    {
        _searchExpressions.Add(searchExpr);
        foreach (LambdaExpression entityExpression in entityExpressions)
        {
            if (entityExpression.Body is MemberExpression body)
                _memberExpressions.Add(new KeyValuePair<int, MemberExpression>(_searchExpressions.Count - 1, body));
        }
    }

    private Expression<Func<TEntity, bool>> GetImplicitSearchExpression(TSearch search)
    {
        Expression<Func<TEntity, bool>> left = null;
        if (string.IsNullOrWhiteSpace(search.SearchString))
            return null;
        for (var index = 0; index < _implicitMemberExpressions.Count; ++index)
        {
            var parameterExpression = Expression.Parameter(typeof(TEntity));
            var memberExpression = _implicitMemberExpressions.ElementAt(index);
            var expression = memberExpression.ToString().Split('.').Skip(1).Aggregate<string, Expression>(parameterExpression, Expression.PropertyOrField);
            var right = Expression.Lambda<Func<TEntity, bool>>(MakeComparison(expression, search.Comparison, search.SearchString), parameterExpression);
            left = left == null ? right : ExpressionExtensions<TEntity>.CombineOr(left, right);
        }
        return left;
    }

    private Expression<Func<TEntity, bool>> GetSearchExpression(TSearch search)
    {
        Expression<Func<TEntity, bool>> left = null;
        for (var i = 0; i < _searchExpressions.Count; i++)
        {
            var parameterExpression = Expression.Parameter(typeof(TEntity));
            var memberExpressions = _memberExpressions.Where(x => x.Key == i).Select(x => x.Value);
            var searchPropertyOption1 = _searchExpressions.ElementAt(i)(search);
            if (searchPropertyOption1.CanApply)
            {
                Expression<Func<TEntity, bool>> expression1 = null;
                foreach (var memberExpression in memberExpressions)
                {
                    var expression2 = memberExpression.ToString().Split('.').Skip(1).Aggregate<string, Expression>(parameterExpression, Expression.PropertyOrField);
                    Expression body;

                    if (expression2.Type == typeof(DateTime))
                    {
                        var prp = memberExpression.ToString().Split('.').Skip(1).ToList();
                        prp.Add("Date");
                        expression2 = prp.Aggregate<string, Expression>(parameterExpression, Expression.PropertyOrField);
                    }

                    if (expression2.Type.IsArray)
                    {
                        body = MakeArrayComparison(expression2, searchPropertyOption1.GetValue());
                    }
                    else
                    {
                        body = searchPropertyOption1 is ICollectionSearchPropertyOption searchPropertyOption2
                            ? MakeComparison(expression2, SearchOptions.Contains, searchPropertyOption2.CollectionObject)
                            : MakeComparison(expression2, searchPropertyOption1.SearchOption, searchPropertyOption1.GetStringValue());
                    }
                    var right = Expression.Lambda<Func<TEntity, bool>>(body, parameterExpression);
                    expression1 = expression1 != null ? ExpressionExtensions<TEntity>.CombineOr(expression1, right) : right;
                }
                if (expression1 != null)
                {
                    if (left == null)
                    {
                        left = expression1;
                    }
                    else
                    {
                        Expression<Func<TEntity, bool>> expression2;
                        switch (searchPropertyOption1.JoinOption)
                        {
                            case JoinOptions.And:
                                expression2 = ExpressionExtensions<TEntity>.CombineAnd(left, expression1);
                                break;
                            case JoinOptions.Or:
                                expression2 = ExpressionExtensions<TEntity>.CombineOr(left, expression1);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        left = expression2;
                    }
                }
            }
        }
        return left;
    }

    protected IQueryable<TEntity> GetSearchQuery(TSearch search)
    {
        var searchExpression1 = GetSearchExpression(search);
        var searchExpression2 = GetImplicitSearchExpression(search);
        var queryable = GetExistingSet();

        if (searchExpression1 != null)
            queryable = queryable.Where(searchExpression1);
        if (searchExpression2 != null)
            queryable = queryable.Where(searchExpression2);

        if (!string.IsNullOrWhiteSpace(search.OrderBy))
        {
            var propertyInfo = typeof(TEntity).GetProperties().FirstOrDefault(x => string.Equals(search.OrderBy, x.Name, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo != null)
                queryable = search.OrderDirection == OrderDirection.Asc ? queryable.OrderBy(propertyInfo.Name) : queryable.OrderByDescending(propertyInfo.Name);
        }

        //If have pagination but order by not provided
        if (search.PageSize > 0 && string.IsNullOrWhiteSpace(search.OrderBy))
            queryable.OrderBy("Uid");

        if (search.PageSize > 0 && search.PageNumber > 0)
            queryable = queryable.Skip(search.PageSize * (search.PageNumber - 1)).Take(search.PageSize);
        else if (search.PageSize > 0)
            queryable = queryable.Take(search.PageSize);


        return queryable;
    }

    private static Expression MakeComparison(Expression left, SearchOptions comparison, object value)
    {
        if (comparison == SearchOptions.Like)
        {
            if (value is string str)
                value = str.ToLower();
        }
        switch (comparison)
        {
            case SearchOptions.Equals:
                return MakeBinary(ExpressionType.Equal, left, value?.ToString());
            case SearchOptions.Like:
                return Expression.Call(Expression.Call(MakeString(left), "ToLower", Type.EmptyTypes), "Contains", Type.EmptyTypes, (Expression)Expression.Constant(value, typeof(string)));
            case SearchOptions.Contains:
                {
                    var valueType = value.GetType();

                    if (valueType.IsArray)
                    {
                        var elementType = valueType.GetElementType()!;
                        var containsMethod = typeof(Enumerable)
                            .GetMethods()
                            .Single(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(elementType);
                        
                        return Expression.Call(containsMethod, Expression.Constant(value, valueType), left);
                    }
                    else
                    {
                        return Expression.Call(Expression.Constant(value, valueType), "Contains", Type.EmptyTypes, left);
                    }
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
        }
    }

    private Expression MakeArrayComparison(Expression left, object value)
    {
        var method = typeof(IEnumerable<Guid>).GetMethod("Contains");
        if ((object)method == null)
            throw new InvalidOperationException();
        return Expression.Call(method, left, Expression.Constant(value, typeof(Guid)));
    }

    private static Expression MakeString(Expression source)
    {
        return !(source.Type == typeof(string)) ? Expression.Call(source, "ToString", Type.EmptyTypes) : source;
    }

    private static Expression MakeBinary(ExpressionType type, Expression left, string value)
    {
        object obj = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                obj = null;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                    left = Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type));
            }
            else
            {
                var type1 = Nullable.GetUnderlyingType(left.Type);
                if ((object)type1 == null)
                    type1 = left.Type;
                var type2 = type1;
                obj = type2.IsEnum ? Enum.Parse(type2, value) : (type2 == typeof(Guid) ? Guid.Parse(value) : Convert.ChangeType(value, type2));
            }
        }
        var constantExpression = Expression.Constant(obj, left.Type);
        return Expression.MakeBinary(type, left, constantExpression);
    }
}

public static class ExpressionExtensions<T>
{
    public static Expression<Func<T, bool>> CombineAnd(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        Expression<Func<T, bool>> combined = Expression.Lambda<Func<T, bool>>(
            Expression.And(
                left.Body,
                new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body) ?? throw new InvalidOperationException()
            ), left.Parameters);

        return combined;
    }
    public static Expression<Func<T, bool>> CombineOr(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        Expression<Func<T, bool>> combined = Expression.Lambda<Func<T, bool>>(
            Expression.Or(
                left.Body,
                new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body) ?? throw new InvalidOperationException()
            ), left.Parameters);

        return combined;
    }
}

public class ExpressionParameterReplacer : ExpressionVisitor
{
    private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements { get; set; }

    public ExpressionParameterReplacer
        (IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
    {
        ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

        for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
        { ParameterReplacements.Add(fromParameters[i], toParameters[i]); }
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (ParameterReplacements.TryGetValue(node, out var replacement))
        { node = replacement; }

        return base.VisitParameter(node);
    }
}

internal static class QueryAbleExtensions
{
    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName)
    {
        var type = typeof(TSource);
        var property = type.GetProperty(propertyName);
        var parameterExpression = Expression.Parameter(type, "x");
        var lambdaExpression = Expression.Lambda(Expression.Property(parameterExpression, propertyName), parameterExpression);
        if (property == null)
            throw new MissingFieldException(propertyName);

        var methodInfo = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(OrderBy) && m.IsGenericMethodDefinition).Single(m => m.GetParameters().ToList().Count == 2).MakeGenericMethod(type, property.PropertyType);
        return (IOrderedQueryable<TSource>)methodInfo.Invoke(methodInfo, new object[] { query, lambdaExpression });
    }

    public static IOrderedQueryable<TSource> ThenBy<TSource>(this IQueryable<TSource> query, string propertyName)
    {
        var type = typeof(TSource);
        var property = type.GetProperty(propertyName);
        var parameterExpression = Expression.Parameter(type, "x");
        var lambdaExpression = Expression.Lambda(Expression.Property(parameterExpression, propertyName), parameterExpression);
        if (property == null)
            throw new MissingFieldException(propertyName);

        var methodInfo = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(ThenBy) && m.IsGenericMethodDefinition).Single(m => m.GetParameters().ToList().Count == 2).MakeGenericMethod(type, property.PropertyType);
        return (IOrderedQueryable<TSource>)methodInfo.Invoke(methodInfo, new object[] { query, lambdaExpression });
    }

    public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> query, string propertyName)
    {
        var type = typeof(TSource);
        var property = type.GetProperty(propertyName);
        var parameterExpression = Expression.Parameter(type, "x");
        var lambdaExpression = Expression.Lambda(Expression.Property(parameterExpression, propertyName), parameterExpression);
        if (property == null)
            throw new MissingFieldException(propertyName);

        var methodInfo = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(OrderByDescending) && m.IsGenericMethodDefinition).Single(m => m.GetParameters().ToList().Count == 2).MakeGenericMethod(type, property.PropertyType);
        return (IOrderedQueryable<TSource>)methodInfo.Invoke(methodInfo, new object[] { query, lambdaExpression });
    }

    public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IQueryable<TSource> query, string propertyName)
    {
        var type = typeof(TSource);
        var property = type.GetProperty(propertyName);
        var parameterExpression = Expression.Parameter(type, "x");
        var lambdaExpression = Expression.Lambda(Expression.Property(parameterExpression, propertyName), parameterExpression);
        if (property == null)
            throw new MissingFieldException(propertyName);

        var methodInfo = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(ThenByDescending) && m.IsGenericMethodDefinition).Single(m => m.GetParameters().ToList().Count == 2).MakeGenericMethod(type, property.PropertyType);
        return (IOrderedQueryable<TSource>)methodInfo.Invoke(methodInfo, new object[] { query, lambdaExpression });
    }
}