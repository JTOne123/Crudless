﻿using System;
using System.Collections.Generic;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Validation;

namespace UnstableSort.Crudless.Requests
{
    [MaybeValidate]
    public class MergeRequest<TEntity, TIn> 
        : InlineConfigurableRequest, IMergeRequest<TEntity>
        where TEntity : class
    {
        public List<TIn> Items { get; set; } = new List<TIn>();

        public MergeRequest()
        {
        }

        public MergeRequest(List<TIn> items)
        {
            Items = items;
        }

        public void Configure(Action<InlineBulkRequestProfile<MergeRequest<TEntity, TIn>, TIn>> configure)
        {
            var profile = new InlineBulkRequestProfile<MergeRequest<TEntity, TIn>, TIn>(r => r.Items);

            configure(profile);

            Profile = profile;
        }
    }
    
    [MaybeValidate]
    public class MergeRequest<TEntity, TIn, TOut> 
        : InlineConfigurableRequest, IMergeRequest<TEntity, TOut>
        where TEntity : class
    {
        public List<TIn> Items { get; set; } = new List<TIn>();

        public MergeRequest()
        {
        }

        public MergeRequest(List<TIn> items)
        {
            Items = items;
        }

        public void Configure(Action<InlineBulkRequestProfile<MergeRequest<TEntity, TIn, TOut>, TIn>> configure)
        {
            var profile = new InlineBulkRequestProfile<MergeRequest<TEntity, TIn, TOut>, TIn>(r => r.Items);

            configure(profile);

            Profile = profile;
        }
    }

    [MaybeValidate]
    public class MergeByIdRequest<TEntity, TIn> : MergeRequest<TEntity, TIn>
        where TEntity : class
    {
        public MergeByIdRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByIdRequestProfile<TEntity, TIn>
        : BulkRequestProfile<MergeByIdRequest<TEntity, TIn>, TIn>
        where TEntity : class
    {
        public MergeByIdRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Id");
        }
    }

    [MaybeValidate]
    public class MergeByIdRequest<TEntity, TIn, TOut> : MergeRequest<TEntity, TIn, TOut>
        where TEntity : class
    {
        public MergeByIdRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByIdRequestProfile<TEntity, TIn, TOut>
        : BulkRequestProfile<MergeByIdRequest<TEntity, TIn, TOut>, TIn>
        where TEntity : class
    {
        public MergeByIdRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Id");
        }
    }

    [MaybeValidate]
    public class MergeByGuidRequest<TEntity, TIn> : MergeRequest<TEntity, TIn>
        where TEntity : class
    {
        public MergeByGuidRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByGuidRequestProfile<TEntity, TIn>
        : BulkRequestProfile<MergeByGuidRequest<TEntity, TIn>, TIn>
        where TEntity : class
    {
        public MergeByGuidRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Guid");
        }
    }
    
    [MaybeValidate]
    public class MergeByGuidRequest<TEntity, TIn, TOut> : MergeRequest<TEntity, TIn, TOut>
        where TEntity : class
    {
        public MergeByGuidRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByGuidRequestProfile<TEntity, TIn, TOut>
        : BulkRequestProfile<MergeByGuidRequest<TEntity, TIn, TOut>, TIn>
        where TEntity : class
    {
        public MergeByGuidRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Guid");
        }
    }

    [MaybeValidate]
    public class MergeByNameRequest<TEntity, TIn> : MergeRequest<TEntity, TIn>
        where TEntity : class
    {
        public MergeByNameRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByNameRequestProfile<TEntity, TIn>
        : BulkRequestProfile<MergeByNameRequest<TEntity, TIn>, TIn>
        where TEntity : class
    {
        public MergeByNameRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Name");
        }
    }

    [MaybeValidate]
    public class MergeByNameRequest<TEntity, TIn, TOut> : MergeRequest<TEntity, TIn, TOut>
        where TEntity : class
    {
        public MergeByNameRequest(List<TIn> items) : base(items) { }
    }

    public class MergeByNameRequestProfile<TEntity, TIn, TOut>
        : BulkRequestProfile<MergeByNameRequest<TEntity, TIn, TOut>, TIn>
        where TEntity : class
    {
        public MergeByNameRequestProfile()
            : base(request => request.Items)
        {
            ForEntity<TEntity>()
                .UseKeys("Name");
        }
    }
}
