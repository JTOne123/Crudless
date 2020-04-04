﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Context;
using UnstableSort.Crudless.Extensions;
using UnstableSort.Crudless.Mediator;

namespace UnstableSort.Crudless.Requests
{
    internal abstract class SynchronizeRequestHandlerBase<TRequest, TEntity>
        : CrudlessRequestHandler<TRequest, TEntity>
        where TEntity : class, new()
        where TRequest : ISynchronizeRequest, ICrudlessRequest<TEntity>
    {
        protected readonly RequestOptions Options;

        protected SynchronizeRequestHandlerBase(IEntityContext context, CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
            Options = RequestConfig.GetOptionsFor<TEntity>();
        }

        protected async Task<TEntity[]> SynchronizeEntities(TRequest request, CancellationToken ct)
        {
            await request.RunRequestHooks(RequestConfig, ct).Configure();

            var itemSource = RequestConfig.GetRequestItemSourceFor<TEntity>();
            var items = ((IEnumerable<object>)itemSource.ItemSource(request)).ToArray();

            items = await request.RunItemHooks<TEntity>(RequestConfig, items, ct).Configure();

            var deletedEntities = await DeleteEntities(request, ct).Configure();
            ct.ThrowIfCancellationRequested();
            
            var entities = await Context.Set<TEntity>()
                .SelectWith(request, RequestConfig)
                .FilterWith(request, RequestConfig)
                .ToArrayAsync(ct)
                .Configure();

            var auditEntities = entities
                .Select(x => (Mapper.Map<TEntity, TEntity>(x), x))
                .ToArray();

            ct.ThrowIfCancellationRequested();

            var joinedItems = RequestConfig
                .Join(items.Where(x => x != null), entities)
                .ToArray();

            var createdEntities = await CreateEntities(request, 
                joinedItems.Where(x => x.Item2 == null).Select(x => x.Item1), ct).Configure();

            ct.ThrowIfCancellationRequested();

            var updatedEntities = await UpdateEntities(
                request, joinedItems.Where(x => x.Item2 != null), ct).Configure();

            ct.ThrowIfCancellationRequested();

            var mergedEntities = updatedEntities.Concat(createdEntities).ToArray();

            await Context.ApplyChangesAsync(ct).Configure();
            ct.ThrowIfCancellationRequested();

            await request
                .RunAuditHooks(RequestConfig, deletedEntities.Concat(auditEntities), ct)
                .Configure();

            return mergedEntities;
        }

        private async Task<(TEntity, TEntity)[]> DeleteEntities(TRequest request, CancellationToken ct)
        {
            var whereClause = RequestConfig.GetSelectorFor<TEntity>().Get<TEntity>()(request);
            var notWhereClause = whereClause.Update(
                Expression.NotEqual(whereClause.Body, Expression.Constant(true)), 
                whereClause.Parameters);

            var deleteEntities = await Context.Set<TEntity>()
                .FilterWith(request, RequestConfig)
                .Where(notWhereClause)
                .ToArrayAsync(ct);

            var pairedEntities = deleteEntities
                .Select(x => (Mapper.Map<TEntity, TEntity>(x), x))
                .ToArray();

            await Context.Set<TEntity>().DeleteAsync(DataContext, deleteEntities, ct);
            ct.ThrowIfCancellationRequested();

            return pairedEntities;
        }
        
        private async Task<TEntity[]> CreateEntities(TRequest request, 
            IEnumerable<object> items, 
            CancellationToken ct)
        {
            var entities = await request.CreateEntities<TEntity>(RequestConfig, items, ct).Configure();

            await request.RunEntityHooks<TEntity>(RequestConfig, entities, ct).Configure();

            entities = await Context.Set<TEntity>().CreateAsync(DataContext, entities, ct).Configure();
            ct.ThrowIfCancellationRequested();

            return entities;
        }

        private async Task<TEntity[]> UpdateEntities(TRequest request,
            IEnumerable<Tuple<object, TEntity>> items,
            CancellationToken ct)
        {
            var entities = await request.UpdateEntities(RequestConfig, items, ct).Configure();

            await request.RunEntityHooks<TEntity>(RequestConfig, entities, ct).Configure();

            entities = await Context.Set<TEntity>().UpdateAsync(DataContext, entities, ct).Configure();
            ct.ThrowIfCancellationRequested();

            return entities;
        }
    }

    internal class SynchronizeRequestHandler<TRequest, TEntity>
        : SynchronizeRequestHandlerBase<TRequest, TEntity>,
          IRequestHandler<TRequest>
        where TEntity : class, new()
        where TRequest : ISynchronizeRequest<TEntity>, ICrudlessRequest<TEntity>
    {
        public SynchronizeRequestHandler(IEntityContext context, CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
        }

        public Task<Response> HandleAsync(TRequest request, CancellationToken token)
        {
            return HandleWithErrorsAsync(request, token, (_, ct) => (Task)SynchronizeEntities(request, ct));
        }
    }

    internal class SynchronizeRequestHandler<TRequest, TEntity, TOut>
        : SynchronizeRequestHandlerBase<TRequest, TEntity>,
          IRequestHandler<TRequest, SynchronizeResult<TOut>>
        where TEntity : class, new()
        where TRequest : ISynchronizeRequest<TEntity, TOut>, ICrudlessRequest<TEntity, TOut>
    {
        public SynchronizeRequestHandler(IEntityContext context, CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
        }

        public Task<Response<SynchronizeResult<TOut>>> HandleAsync(TRequest request, CancellationToken token)
        {
            return HandleWithErrorsAsync(request, token, _HandleAsync);
        }

        public async Task<SynchronizeResult<TOut>> _HandleAsync(TRequest request, CancellationToken token)
        {
            var entities = await SynchronizeEntities(request, token).Configure();
            var items = await entities.CreateResults<TEntity, TOut>(RequestConfig, token).Configure();
            var result = new SynchronizeResult<TOut>(items);

            return await request.RunResultHooks(RequestConfig, result, token).Configure();
        }
    }
}
