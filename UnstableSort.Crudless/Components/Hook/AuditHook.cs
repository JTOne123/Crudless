﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IServiceProvider = UnstableSort.Crudless.Common.ServiceProvider.IServiceProvider;

namespace UnstableSort.Crudless
{
    public interface IBoxedAuditHook
    {
        Task Run(object request, object oldEntity, object newEntity, CancellationToken ct = default(CancellationToken));
    }

    public interface IAuditHookFactory
    {
        IBoxedAuditHook Create(IServiceProvider provider);
    }

    public class FunctionAuditHook
       : IBoxedAuditHook
    {
        private readonly Func<object, object, object, CancellationToken, Task> _hookFunc;

        public FunctionAuditHook(Func<object, object, object, CancellationToken, Task> hookFunc)
        {
            _hookFunc = hookFunc;
        }

        public Task Run(object request, object oldEntity, object newEntity, CancellationToken ct = default(CancellationToken))
            => _hookFunc(request, oldEntity, newEntity, ct);
    }

    public class FunctionAuditHookFactory : IAuditHookFactory
    {
        private readonly IBoxedAuditHook _hook;

        private FunctionAuditHookFactory(Func<object, object, object, CancellationToken, Task> hook)
        {
            _hook = new FunctionAuditHook(hook);
        }

        internal static FunctionAuditHookFactory From<TRequest, TEntity>(
            Func<TRequest, TEntity, TEntity, CancellationToken, Task> hook)
            where TEntity : class
        {
            return new FunctionAuditHookFactory(
                (request, oldEntity, newEntity, ct) => hook((TRequest)request, (TEntity)oldEntity, (TEntity)newEntity, ct));
        }

        internal static FunctionAuditHookFactory From<TRequest, TEntity>(
            Action<TRequest, TEntity, TEntity> hook)
            where TEntity : class
        {
            return new FunctionAuditHookFactory(
                (request, oldEntity, newEntity, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return Task.FromCanceled(ct);

                    hook((TRequest)request, (TEntity)oldEntity, (TEntity)newEntity);

                    return Task.CompletedTask;
                });
        }

        public IBoxedAuditHook Create(IServiceProvider provider) => _hook;
    }

    public class InstanceAuditHookFactory : IAuditHookFactory
    {
        private readonly object _instance;
        private IBoxedAuditHook _adaptedInstance;

        private InstanceAuditHookFactory(object instance, IBoxedAuditHook adaptedInstance)
        {
            _instance = instance;
            _adaptedInstance = adaptedInstance;
        }

        internal static InstanceAuditHookFactory From<TRequest, TEntity>(
            IAuditHook<TRequest, TEntity> hook)
            where TEntity : class
        {
            return new InstanceAuditHookFactory(
                hook,
                new FunctionAuditHook((request, oldEntity, newEntity, ct)
                    => hook.Run((TRequest)request, (TEntity)oldEntity, (TEntity)newEntity, ct)));
        }

        public IBoxedAuditHook Create(IServiceProvider provider) => _adaptedInstance;
    }

    public class TypeAuditHookFactory : IAuditHookFactory
    {
        private Func<IServiceProvider, IBoxedAuditHook> _hookFactory;

        public TypeAuditHookFactory(Func<IServiceProvider, IBoxedAuditHook> hookFactory)
        {
            _hookFactory = hookFactory;
        }
        
        internal static TypeAuditHookFactory From<THook, TRequest, TEntity>()
            where TEntity : class
            where THook : IAuditHook<TRequest, TEntity>
        {
            return new TypeAuditHookFactory(
                provider =>
                {
                    var instance = (IAuditHook<TRequest, TEntity>)provider.ProvideInstance(typeof(THook));
                    return new FunctionAuditHook((request, oldEntity, newEntity, ct)
                        => instance.Run((TRequest)request, (TEntity)oldEntity, (TEntity)newEntity, ct));
                });
        }

        public IBoxedAuditHook Create(IServiceProvider provider) => _hookFactory(provider);
    }
}
