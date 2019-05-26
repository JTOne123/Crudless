﻿using System.Collections.Generic;
using System.Linq;
using UnstableSort.Crudless.Mediator;
// ReSharper disable UnusedTypeParameter

namespace UnstableSort.Crudless.Requests
{
    public interface IUpdateAllRequest : IBulkRequest
    {
    }

    public interface IUpdateAllRequest<TEntity> : IUpdateAllRequest, IRequest
        where TEntity : class
    {
    }

    public interface IUpdateAllRequest<TEntity, TOut> : IUpdateAllRequest, IRequest<UpdateAllResult<TOut>>
        where TEntity : class
    {
    }

    public class UpdateAllResult<TOut> : IResultCollection<TOut>
    {
        public List<TOut> Items { get; set; }

        public UpdateAllResult(IEnumerable<TOut> items)
        {
            Items = items.ToList();
        }
    }
}
