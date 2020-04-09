﻿using System;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Validation;

namespace UnstableSort.Crudless.Requests
{
    [MaybeValidate]
    public class GetRequest<TEntity, TKey, TOut> 
        : InlineConfigurableRequest, IGetRequest<TEntity, TOut>
        where TEntity : class
    {
        public TKey Key { get; set; }

        public GetRequest() { }

        public GetRequest(TKey key) { Key = key; }

        public void Configure(Action<InlineRequestProfile<GetRequest<TEntity, TKey, TOut>>> configure)
        {
            var profile = new InlineRequestProfile<GetRequest<TEntity, TKey, TOut>>();

            configure(profile);

            Profile = profile;
        }
    }

    public class GetRequestProfile<TEntity, TKey, TOut>
        : RequestProfile<GetRequest<TEntity, TKey, TOut>>
        where TEntity : class
    {
        public GetRequestProfile()
        {
            ForEntity<TEntity>().UseRequestKey(request => request.Key);
        }
    }

    [MaybeValidate]
    public class GetByIdRequest<TEntity, TOut> : GetRequest<TEntity, int, TOut>
        where TEntity : class
    {
        public GetByIdRequest(int id) : base(id) { }
    }

    public class GetByIdRequestProfile<TEntity, TOut>
        : RequestProfile<GetByIdRequest<TEntity, TOut>>
        where TEntity : class
    {
        public GetByIdRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Id");
        }
    }

    [MaybeValidate]
    public class GetByGuidRequest<TEntity, TOut> : GetRequest<TEntity, Guid, TOut>
        where TEntity : class
    {
        public GetByGuidRequest(Guid guid) : base(guid) { }
    }

    public class GetByGuidRequestProfile<TEntity, TOut>
        : RequestProfile<GetByGuidRequest<TEntity, TOut>>
        where TEntity : class
    {
        public GetByGuidRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Guid");
        }
    }

    [MaybeValidate]
    public class GetByNameRequest<TEntity, TOut> : GetRequest<TEntity, string, TOut>
        where TEntity : class
    {
        public GetByNameRequest(string name) : base(name) { }
    }

    public class GetByNameRequestProfile<TEntity, TOut>
        : RequestProfile<GetByNameRequest<TEntity, TOut>>
        where TEntity : class
    {
        public GetByNameRequestProfile()
        {
            ForEntity<TEntity>().UseEntityKey("Name");
        }
    }
}
