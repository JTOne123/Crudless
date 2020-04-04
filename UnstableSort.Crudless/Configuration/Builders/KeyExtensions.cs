﻿using System;
using System.Linq.Expressions;
using UnstableSort.Crudless.Configuration.Builders;

// ReSharper disable once CheckNamespace
namespace UnstableSort.Crudless.Configuration
{
    public static class KeyExtensions
    {
        public static RequestEntityConfigBuilder<TRequest, TEntity> UseKeys<TRequest, TEntity, TItemKey, TEntityKey>(
            this RequestEntityConfigBuilder<TRequest, TEntity> config,
            Expression<Func<TRequest, TItemKey>> requestKeyExpr,
            Expression<Func<TEntity, TEntityKey>> entityKeyExpr)
            where TEntity : class
        {
            return config
                .UseEntityKey(entityKeyExpr)
                .UseRequestKey(requestKeyExpr);
        }
        
        public static RequestEntityConfigBuilder<TRequest, TEntity> UseKeys<TRequest, TEntity>(
            this RequestEntityConfigBuilder<TRequest, TEntity> config,
            string requestKeyMember,
            string entityKeyMember)
            where TEntity : class
        {
            return config
                .UseEntityKey(entityKeyMember)
                .UseRequestKey(requestKeyMember);
        }

        public static RequestEntityConfigBuilder<TRequest, TEntity> UseKeys<TRequest, TEntity>(
            this RequestEntityConfigBuilder<TRequest, TEntity> config,
            string[] requestKeyMembers,
            string[] entityKeyMembers)
            where TEntity : class
        {
            return config
                .UseEntityKey(entityKeyMembers)
                .UseRequestKey(requestKeyMembers);
        }

        public static RequestEntityConfigBuilder<TRequest, TEntity> UseKeys<TRequest, TEntity>(
            this RequestEntityConfigBuilder<TRequest, TEntity> config,
            string keyMember) 
            where TEntity : class 
                => config.UseKeys(keyMember, keyMember);

        public static RequestEntityConfigBuilder<TRequest, TEntity> UseKeys<TRequest, TEntity>(
            this RequestEntityConfigBuilder<TRequest, TEntity> config,
            string[] keyMembers)
            where TEntity : class
                => config.UseKeys(keyMembers, keyMembers);

        public static BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> UseKeys<TRequest, TItem, TEntity, TItemKey, TEntityKey>(
            this BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> config,
            Expression<Func<TItem, TItemKey>> requestItemKeyExpr,
            Expression<Func<TEntity, TEntityKey>> entityKeyExpr)
            where TEntity : class
        {
            return config
                .UseEntityKey(entityKeyExpr)
                .UseRequestItemKey(requestItemKeyExpr);
        }

        public static BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> UseKeys<TRequest, TItem, TEntity>(
            this BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> config,
            string itemKeyProperty,
            string entityKeyProperty)
            where TEntity : class
        {
            return config
                .UseEntityKey(entityKeyProperty)
                .UseRequestItemKey(itemKeyProperty);
        }
        
        public static BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> UseKeys<TRequest, TItem, TEntity>(
            this BulkRequestEntityConfigBuilder<TRequest, TItem, TEntity> config,
            string keyProperty)
            where TEntity : class
                => config.UseKeys(keyProperty, keyProperty);
    }
}
