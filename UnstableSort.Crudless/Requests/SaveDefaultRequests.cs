﻿using AutoMapper;
using System;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Validation;

namespace UnstableSort.Crudless.Requests
{
    [MaybeValidate]
    public class SaveRequest<TEntity, TIn> : ISaveRequest<TEntity>
        where TEntity : class
    {
        public TIn Item { get; set; }

        public SaveRequest() { }

        public SaveRequest(TIn item) { Item = item; }
    }

    public class SaveRequestProfile<TEntity, TIn>
        : RequestProfile<SaveRequest<TEntity, TIn>>
        where TEntity : class
    {
        public SaveRequestProfile()
        {
            ForEntity<TEntity>()
                .CreateEntityWith(request => Mapper.Map<TEntity>(request.Item))
                .UpdateEntityWith((request, entity) => Mapper.Map(request.Item, entity));
        }
    }

    [MaybeValidate]
    public class SaveRequest<TEntity, TIn, TOut> : ISaveRequest<TEntity, TOut>
        where TEntity : class
    {
        public TIn Item { get; set; }

        public SaveRequest() { }

        public SaveRequest(TIn item) { Item = item; }
    }

    public class SaveRequestProfile<TEntity, TIn, TOut>
        : RequestProfile<SaveRequest<TEntity, TIn, TOut>>
        where TEntity : class
    {
        public SaveRequestProfile()
        {
            ForEntity<TEntity>()
                .CreateEntityWith(request => Mapper.Map<TEntity>(request.Item))
                .UpdateEntityWith((request, entity) => Mapper.Map(request.Item, entity));
        }
    }

    [MaybeValidate]
    public class SaveRequest<TEntity, TKey, TIn, TOut>
        : ISaveRequest<TEntity, TOut>
        where TEntity : class
    {
        public TKey Key { get; set; }

        public TIn Item { get; set; }

        public SaveRequest() { }

        public SaveRequest(TKey key, TIn item)
        {
            Key = key;
            Item = item;
        }
    }

    public class SaveRequestProfile<TEntity, TKey, TIn, TOut>
        : RequestProfile<SaveRequest<TEntity, TKey, TIn, TOut>>
        where TEntity : class
    {
        public SaveRequestProfile()
        {
            ForEntity<TEntity>()
                .UseRequestKey(request => request.Key)
                .CreateEntityWith(request => Mapper.Map<TEntity>(request.Item))
                .UpdateEntityWith((request, entity) => Mapper.Map(request.Item, entity));
        }
    }

    [MaybeValidate]
    public class SaveByIdRequest<TEntity, TIn, TOut> : SaveRequest<TEntity, int, TIn, TOut>
        where TEntity : class
    {
        public SaveByIdRequest(int id, TIn data) : base(id, data) { }
    }

    public class SaveByIdRequestProfile<TEntity, TIn, TOut>
        : RequestProfile<SaveByIdRequest<TEntity, TIn, TOut>>
        where TEntity : class
    {
        public SaveByIdRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Id");
        }
    }

    [MaybeValidate]
    public class SaveByGuidRequest<TEntity, TIn, TOut> : SaveRequest<TEntity, Guid, TIn, TOut>
        where TEntity : class
    {
        public SaveByGuidRequest(Guid guid, TIn data) : base(guid, data) { }
    }

    public class SaveByGuidRequestProfile<TEntity, TIn, TOut>
        : RequestProfile<SaveByGuidRequest<TEntity, TIn, TOut>>
        where TEntity : class
    {
        public SaveByGuidRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Guid");
        }
    }

    [MaybeValidate]
    public class SaveByNameRequest<TEntity, TIn, TOut> : SaveRequest<TEntity, string, TIn, TOut>
        where TEntity : class
    {
        public SaveByNameRequest(string name, TIn data) : base(name, data) { }
    }

    public class SaveByNameRequestProfile<TEntity, TIn, TOut>
        : RequestProfile<SaveByNameRequest<TEntity, TIn, TOut>>
        where TEntity : class
    {
        public SaveByNameRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Name");
        }
    }
}