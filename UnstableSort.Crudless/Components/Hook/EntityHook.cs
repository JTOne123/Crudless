﻿using System;
using System.Threading;
using System.Threading.Tasks;

using IServiceProvider = UnstableSort.Crudless.Common.ServiceProvider.IServiceProvider;

namespace UnstableSort.Crudless
{
    public interface IBoxedEntityHook
    {
        Task Run(object request, object entity, CancellationToken ct = default(CancellationToken));
    }

    public interface IEntityHookFactory
    {
        IBoxedEntityHook Create(IServiceProvider provider);
    }

    public class FunctionEntityHook
       : IBoxedEntityHook
    {
        private readonly Func<object, object, CancellationToken, Task> _hookFunc;

        public FunctionEntityHook(Func<object, object, CancellationToken, Task> hookFunc)
        {
            _hookFunc = hookFunc;
        }

        public Task Run(object request, object entity, CancellationToken ct = default(CancellationToken)) 
            => _hookFunc(request, entity, ct);
    }

    public class FunctionEntityHookFactory : IEntityHookFactory
    {
        private readonly IBoxedEntityHook _hook;

        private FunctionEntityHookFactory(Func<object, object, CancellationToken, Task> hook)
        {
            _hook = new FunctionEntityHook(hook);
        }

        internal static FunctionEntityHookFactory From<TRequest, TEntity>(
            Func<TRequest, TEntity, CancellationToken, Task> hook)
            where TEntity : class
        {
            return new FunctionEntityHookFactory(
                (request, entity, ct) => hook((TRequest)request, (TEntity)entity, ct));
        }

        internal static FunctionEntityHookFactory From<TRequest, TEntity>(
            Func<TRequest, TEntity, Task> hook)
            where TEntity : class
        {
            return new FunctionEntityHookFactory(
                (request, entity, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return Task.FromCanceled(ct);

                    return hook((TRequest)request, (TEntity)entity);
                });
        }

        internal static FunctionEntityHookFactory From<TRequest, TEntity>(
            Action<TRequest, TEntity> hook)
            where TEntity : class
        {
            return new FunctionEntityHookFactory(
                (request, entity, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return Task.FromCanceled(ct);

                    hook((TRequest)request, (TEntity)entity);

                    return Task.CompletedTask;
                });
        }

        public IBoxedEntityHook Create(IServiceProvider provider) => _hook;
    }

    public class InstanceEntityHookFactory : IEntityHookFactory
    {
        private readonly object _instance;
        private IBoxedEntityHook _adaptedInstance;

        private InstanceEntityHookFactory(object instance, IBoxedEntityHook adaptedInstance)
        {
            _instance = instance;
            _adaptedInstance = adaptedInstance;
        }

        internal static InstanceEntityHookFactory From<TRequest, TEntity>(
            EntityHook<TRequest, TEntity> hook)
            where TEntity : class
        {
            return new InstanceEntityHookFactory(
                hook,
                new FunctionEntityHook((request, entity, ct) 
                    => hook.Run((TRequest)request, (TEntity)entity, ct)));
        }

        public IBoxedEntityHook Create(IServiceProvider provider) => _adaptedInstance;
    }

    public class TypeEntityHookFactory : IEntityHookFactory
    {
        private Func<IServiceProvider, IBoxedEntityHook> _hookFactory;

        public TypeEntityHookFactory(Func<IServiceProvider, IBoxedEntityHook> hookFactory)
        {
            _hookFactory = hookFactory;
        }
        
        internal static TypeEntityHookFactory From<THook, TRequest, TEntity>()
            where TEntity : class
            where THook : EntityHook<TRequest, TEntity>
        {
            return new TypeEntityHookFactory(
                provider =>
                {
                    var instance = (EntityHook<TRequest, TEntity>)provider.ProvideInstance(typeof(THook));
                    return new FunctionEntityHook((request, entity, ct)
                        => instance.Run((TRequest)request, (TEntity)entity, ct));
                });
        }
        
        public IBoxedEntityHook Create(IServiceProvider provider) => _hookFactory(provider);
    }
}
