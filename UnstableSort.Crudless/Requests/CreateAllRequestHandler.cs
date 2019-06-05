﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Context;
using UnstableSort.Crudless.Extensions;
using UnstableSort.Crudless.Mediator;

namespace UnstableSort.Crudless.Requests
{
    internal abstract class CreateAllRequestHandlerBase<TRequest, TEntity>
        : CrudlessRequestHandler<TRequest, TEntity>
        where TEntity : class
        where TRequest : ICreateAllRequest, ICrudlessRequest<TEntity>
    {
        protected CreateAllRequestHandlerBase(IEntityContext context, CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
        }

        protected async Task<TEntity[]> CreateEntities(TRequest request, CancellationToken ct)
        {
            await request.RunRequestHooks(RequestConfig, ct).Configure();
            
            var itemSource = RequestConfig.GetRequestItemSourceFor<TEntity>();
            var items = ((IEnumerable<object>)itemSource.ItemSource(request)).ToArray();

            items = await request.RunItemHooks<TEntity>(RequestConfig, items, ct).Configure();
            var entities = await request.CreateEntities<TEntity>(RequestConfig, items, ct).Configure();

            await request.RunEntityHooks<TEntity>(RequestConfig, entities, ct).Configure();
            
            entities = await Context.Set<TEntity>().CreateAsync(DataContext, entities, ct).Configure();
            ct.ThrowIfCancellationRequested();
            
            await Context.ApplyChangesAsync(ct).Configure();
            ct.ThrowIfCancellationRequested();

            await request.RunAuditHooks(RequestConfig, entities.Select(x => ((TEntity)null, x)), ct).Configure();

            return entities;
        }
    }

    internal class CreateAllRequestHandler<TRequest, TEntity>
        : CreateAllRequestHandlerBase<TRequest, TEntity>,
          IRequestHandler<TRequest>
        where TEntity : class
        where TRequest : ICreateAllRequest<TEntity>, ICrudlessRequest<TEntity>
    {
        public CreateAllRequestHandler(IEntityContext context,
            CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
        }

        public Task<Response> HandleAsync(TRequest request, CancellationToken token)
        {
            return HandleWithErrorsAsync(request, token, (_, ct) => (Task)CreateEntities(request, ct));
        }
    }

    internal class CreateAllRequestHandler<TRequest, TEntity, TOut>
        : CreateAllRequestHandlerBase<TRequest, TEntity>,
          IRequestHandler<TRequest, CreateAllResult<TOut>>
        where TEntity : class
        where TRequest : ICreateAllRequest<TEntity, TOut>, ICrudlessRequest<TEntity, TOut>
    {
        public CreateAllRequestHandler(IEntityContext context,
            CrudlessConfigManager profileManager)
            : base(context, profileManager)
        {
        }

        public Task<Response<CreateAllResult<TOut>>> HandleAsync(TRequest request, CancellationToken token)
        {
            return HandleWithErrorsAsync(request, token, _HandleAsync);
        }

        private async Task<CreateAllResult<TOut>> _HandleAsync(TRequest request, CancellationToken token)
        {
            var entities = await CreateEntities(request, token).Configure();
            var items = await entities.CreateResults<TEntity, TOut>(RequestConfig, token).Configure();
            var result = new CreateAllResult<TOut>(items);

            return await request.RunResultHooks(RequestConfig, result, token).Configure();
        }
    }
}
