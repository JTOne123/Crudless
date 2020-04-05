﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnstableSort.Crudless.Configuration.Builders;
using UnstableSort.Crudless.Requests;

namespace UnstableSort.Crudless.Configuration
{
    public abstract class BulkRequestProfile<TRequest, TItem>
        : RequestProfileCommon<TRequest>
        where TRequest : IBulkRequest
    {
        private readonly Expression<Func<TRequest, ICollection<TItem>>> _defaultItemSource;

        public BulkRequestProfile()
        {
        }

        public BulkRequestProfile(Expression<Func<TRequest, ICollection<TItem>>> defaultItemSource)
        {
            _defaultItemSource = defaultItemSource;
        }

        /// <summary>
        /// Begins a configuration for an entity type.
        /// See the docs for more information on configuring entities for requests.
        /// </summary>
        public BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> Entity<TEntity>()
            where TEntity : class
        {
            var builder = new BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity>();

            if (_defaultItemSource != null)
                builder.HasRequestItems(_defaultItemSource);

            _requestEntityBuilders[typeof(TEntity)] = builder;

            return builder;
        }
    }

    public class DefaultBulkRequestProfile<TRequest> : BulkRequestProfile<TRequest, TRequest>
        where TRequest : IBulkRequest
    {
    }
}
