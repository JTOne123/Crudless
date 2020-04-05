﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnstableSort.Crudless.Configuration.Builders;

// ReSharper disable once CheckNamespace
namespace UnstableSort.Crudless.Configuration
{
    public static class BasicFilterExtensions
    {
        /// <summary>
        /// Adds a request filter that includes entities where the filter predicate evaluates to true:
        ///     ie: Where(entity => filter(entity) == true)
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddFilter<TBuilder, TRequest, TEntity>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Expression<Func<TEntity, bool>> filter,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            return condition == null
                ? config.AddRequestFilter((_, queryable) => queryable.Where(filter))
                : config.AddRequestFilter((request, queryable) =>
                    condition(request) ? queryable.Where(filter) : queryable);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the filter-returned predicate evaluates to true:
        ///     ie: Where(request => entity => filter'(entity) == true)
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddFilter<TBuilder, TRequest, TEntity>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Func<TRequest, Expression<Func<TEntity, bool>>> filter,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            return condition == null
                ? config.AddRequestFilter((request, queryable) =>
                    queryable.Where(filter(request)))
                : config.AddRequestFilter((request, queryable) =>
                    condition(request) ? queryable.Where(filter(request)) : queryable);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the predicate evaluates to true:
        ///     ie: Where((request, entity) => filter(request, entity) == true)
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddFilter<TBuilder, TRequest, TEntity>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Expression<Func<TRequest, TEntity, bool>> filter,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var body = filter.Body.ReplaceParameter(filter.Parameters[0], rParamExpr);
            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(body, filter.Parameters[1]));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the provided comparator evaluates to
        /// true for the request's requestValue and the entity's entityValue:
        ///     ie: Where(entity => comparator(request.requestValue, entity.entityProp))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddBinaryFilter<TBuilder, TRequest, TEntity, TRequestProp, TEntityProp>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Expression<Func<TRequest, TRequestProp>> requestValue,
            Expression<Func<TEntity, TEntityProp>> entityValue,
            Expression<Func<TRequestProp, TEntityProp, bool>> comparator,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");
            
            var rValueExpr = Expression.Invoke(requestValue, rParamExpr);
            var eValueExpr = Expression.Invoke(entityValue, eParamExpr);

            var compareExpr = Expression.Invoke(comparator, rValueExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(compareExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the provided comparator evaluates to
        /// true for the provided value and the entity's entityValue:
        ///     ie: Where(entity => comparator(value, entity.entityProp))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddBinaryFilter<TBuilder, TRequest, TEntity, TValue, TEntityProp>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            TValue value,
            Expression<Func<TEntity, TEntityProp>> entityValue,
            Expression<Func<TValue, TEntityProp, bool>> comparator,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");

            var rValueExpr = Expression.Constant(value, typeof(TValue));
            var eValueExpr = Expression.Invoke(entityValue, eParamExpr);

            var compareExpr = Expression.Invoke(comparator, rValueExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(compareExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the request's requestCollection
        /// contains the entity's entityValue:
        ///     ie: Where(entity => request.requestCollection.Contains(entity.entityValue))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddContainsFilter<TBuilder, TRequest, TEntity, TItem>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Expression<Func<TRequest, ICollection<TItem>>> requestCollection,
            Expression<Func<TEntity, TItem>> entityValue,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");

            var rCollectionExpr = Expression.Invoke(requestCollection, rParamExpr);
            var eValueExpr = Expression.Invoke(entityValue, eParamExpr);

            var containsInfo = typeof(Enumerable)
                .GetMethods()
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TItem));

            var rContainsExpr = Expression.Call(containsInfo, rCollectionExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(rContainsExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the request's requestCollection
        /// contains the entity's entityValueMember:
        ///     ie: Where(entity => request.requestCollection.Contains(entity.entityValueMember))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddContainsFilter<TBuilder, TRequest, TEntity, TItem>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            Expression<Func<TRequest, ICollection<TItem>>> requestCollection,
            string entityValueMember,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");

            var rCollectionExpr = Expression.Invoke(requestCollection, rParamExpr);
            var eValueExpr = Expression.PropertyOrField(eParamExpr, entityValueMember);

            var containsInfo = typeof(Enumerable)
                .GetMethods()
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TItem));

            var rContainsExpr = Expression.Call(containsInfo, rCollectionExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(rContainsExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the provided collection
        /// contains the entity's entityValue:
        ///     ie: Where(entity => collection.Contains(entity.entityValue))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddContainsFilter<TBuilder, TRequest, TEntity, TItem>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            ICollection<TItem> collection,
            Expression<Func<TEntity, TItem>> entityValue,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");

            var rCollectionExpr = Expression.Constant(collection, typeof(ICollection<TItem>));
            var eValueExpr = Expression.Invoke(entityValue, eParamExpr);

            var containsInfo = typeof(Enumerable)
                .GetMethods()
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TItem));

            var rContainsExpr = Expression.Call(containsInfo, rCollectionExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(rContainsExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }

        /// <summary>
        /// Adds a request filter that includes entities where the provided collection
        /// contains the entity's entityValueMember:
        ///     ie: Where(entity => collection.Contains(entity.entityValueMember))
        /// </summary>
        /// <param name="condition">If condition is supplied, then the filter will only be applied when condition evaluates to true</param>
        public static TBuilder AddContainsFilter<TBuilder, TRequest, TEntity, TItem>(
            this RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder> config,
            ICollection<TItem> collection,
            string entityValueMember,
            Func<TRequest, bool> condition = null)
            where TEntity : class
            where TBuilder : RequestEntityConfigBuilderCommon<TRequest, TEntity, TBuilder>
        {
            var rParamExpr = Expression.Parameter(typeof(TRequest), "r");
            var eParamExpr = Expression.Parameter(typeof(TEntity), "e");

            var rCollectionExpr = Expression.Constant(collection, typeof(ICollection<TItem>));
            var eValueExpr = Expression.PropertyOrField(eParamExpr, entityValueMember);

            var containsInfo = typeof(Enumerable)
                .GetMethods()
                .Single(x => x.Name == nameof(Enumerable.Contains) && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TItem));

            var rContainsExpr = Expression.Call(containsInfo, rCollectionExpr, eValueExpr);

            var filterClause = Expression.Quote(Expression.Lambda<Func<TEntity, bool>>(rContainsExpr, eParamExpr));
            var filterLambda = Expression.Lambda<Func<TRequest, Expression<Func<TEntity, bool>>>>(filterClause, rParamExpr);

            return config.AddFilter(filterLambda.Compile(), condition);
        }
    }
}