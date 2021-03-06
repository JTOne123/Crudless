﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UnstableSort.Crudless.Context;
using UnstableSort.Crudless.Integration.EntityFrameworkCore;
using UnstableSort.Crudless.Integration.EntityFrameworkExtensions.Configuration;
using UnstableSort.Crudless.Integration.EntityFrameworkExtensions.Extensions;

namespace UnstableSort.Crudless.Integration.EntityFrameworkExtensions
{
    public class BulkDataAgent : IBulkCreateDataAgent, IBulkUpdateDataAgent, IBulkDeleteDataAgent
    {
        public async Task<TEntity[]> CreateAsync<TEntity>(DataContext<TEntity> context,
            IEnumerable<TEntity> items,
            CancellationToken token = default(CancellationToken))
            where TEntity : class
        {
            token.ThrowIfCancellationRequested();

            var set = context.EntitySet.Implementation as EntityFrameworkEntitySet<TEntity>;
            var entities = items.ToArray();

            await DetachEntities(entities, set.Context, EntityState.Added, token);
            
            await set.Context.BulkInsertAsync(entities,
                operation => operation.Configure(BulkConfigurationType.Insert, context),
                token);

            return entities;
        }

        public async Task<TEntity[]> UpdateAsync<TEntity>(DataContext<TEntity> context,
            IEnumerable<TEntity> items,
            CancellationToken token = default(CancellationToken))
            where TEntity : class
        {
            token.ThrowIfCancellationRequested();

            var set = context.EntitySet.Implementation as EntityFrameworkEntitySet<TEntity>;
            var entities = items.ToArray();

            await DetachEntities(entities, set.Context, EntityState.Modified, token);

            await set.Context.BulkUpdateAsync(entities,
                operation => operation.Configure(BulkConfigurationType.Update, context),
                token);

            return entities;
        }

        public async Task<TEntity[]> DeleteAsync<TEntity>(DataContext<TEntity> context,
            IEnumerable<TEntity> items,
            CancellationToken token = default(CancellationToken))
            where TEntity : class
        {
            token.ThrowIfCancellationRequested();

            var set = context.EntitySet.Implementation as EntityFrameworkEntitySet<TEntity>;
            var entities = items.ToArray();

            await DetachEntities(entities, set.Context, EntityState.Deleted, token);

            await set.Context.BulkDeleteAsync(entities,
                operation => operation.Configure(BulkConfigurationType.Delete, context),
                token);

            return entities;
        }

        private async Task DetachEntities<TEntity>(TEntity[] entities, DbContext context, EntityState state, CancellationToken token)
            where TEntity : class
        {
            var entries = context
                .ChangeTracker
                .Entries()
                .Where(x => entities.Contains(x.Entity) && x.State == state)
                .ToArray();

            foreach (var entry in entries)
                entry.State = EntityState.Detached;

            if (context.ChangeTracker.Entries().Any(x => x.Entity is TEntity))
                await context.SaveChangesAsync(token);
        }
    }
}
