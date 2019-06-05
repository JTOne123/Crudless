﻿using System.Collections.Generic;
using System.Linq;
using UnstableSort.Crudless.Mediator;
// ReSharper disable UnusedTypeParameter

namespace UnstableSort.Crudless.Requests
{
    public interface IMergeRequest 
        : IBulkRequest
    {
    }

    public interface IMergeRequest<TEntity> 
        : IMergeRequest, IRequest, IAuditedRequest<TEntity>, ICrudlessRequest<TEntity>
        where TEntity : class
    {
    }

    public interface IMergeRequest<TEntity, TOut> 
        : IMergeRequest, IRequest<MergeResult<TOut>>, IAuditedRequest<TEntity>, ICrudlessRequest<TEntity, TOut>
        where TEntity : class
    {
    }

    public class MergeResult<TOut> : IResultCollection<TOut>
    {
        public List<TOut> Items { get; set; }

        public MergeResult(IEnumerable<TOut> items)
        {
            Items = items.ToList();
        }
    }
}
