﻿using System.Threading;
using System.Threading.Tasks;
using UnstableSort.Crudless.Requests;

namespace UnstableSort.Crudless.Tests.Fakes
{
    public class FakeInjectable
    {
    }

    public class TestTypeRequestHook : IRequestHook<TestHooksRequest>
    {
        public TestTypeRequestHook(FakeInjectable injectable)
        {
            if (injectable == null)
                throw new System.Exception("Injection Failed");
        }

        public Task Run(TestHooksRequest request, CancellationToken token)
        {
            request.Items.ForEach(i => i.RequestHookMessage += "r4/");
            return Task.CompletedTask;
        }
    }

    public class TestTypeEntityHook : IEntityHook<TestHooksRequest, HookEntity>
    {
        public TestTypeEntityHook(FakeInjectable injectable)
        {
            if (injectable == null)
                throw new System.Exception("Injection Failed");
        }

        public Task Run(TestHooksRequest request, HookEntity entity, CancellationToken token)
        {
            entity.EntityHookMessage += "e4/";
            return Task.CompletedTask;
        }
    }

    public class TestTypeItemHook : IItemHook<TestHooksRequest, HookDto>
    {
        public TestTypeItemHook(FakeInjectable injectable)
        {
            if (injectable == null)
                throw new System.Exception("Injection Failed");
        }

        public Task<HookDto> Run(TestHooksRequest request, HookDto item, CancellationToken token)
        {
            item.ItemHookMessage += "i4/";
            return Task.FromResult(item);
        }
    }

    public class TestTypeResultHook : IResultHook<TestHooksRequest, string>
    {
        public TestTypeResultHook(FakeInjectable injectable)
        {
            if (injectable == null)
                throw new System.Exception("Injection Failed");
        }

        public Task<string> Run(TestHooksRequest request, string result, CancellationToken token)
        {
            return Task.FromResult(result + "t4/");
        }
    }

    public class TestInstanceRequestHook : IRequestHook<TestHooksRequest>
    {
        public Task Run(TestHooksRequest request, CancellationToken token)
        {
            request.Items.ForEach(i => i.RequestHookMessage += "r3/");
            return Task.CompletedTask;
        }
    }

    public class TestInstanceEntityHook : IEntityHook<TestHooksRequest, HookEntity>
    {
        public Task Run(TestHooksRequest request, HookEntity entity, CancellationToken token)
        {
            entity.EntityHookMessage += "e3/";
            return Task.CompletedTask;
        }
    }

    public class TestInstanceItemHook : IItemHook<TestHooksRequest, HookDto>
    {
        public Task<HookDto> Run(TestHooksRequest request, HookDto item, CancellationToken token)
        {
            item.ItemHookMessage += "i3/";
            return Task.FromResult(item);
        }
    }

    public class TestInstanceResultHook : IResultHook<TestHooksRequest, string>
    {
        public Task<string> Run(TestHooksRequest request, string result, CancellationToken token)
        {
            return Task.FromResult(result + "t3/");
        }
    }

    public class TestContravariantRequestHook : IRequestHook<ICrudlessRequest>
    {
        public Task Run(ICrudlessRequest request, CancellationToken token)
        {
            ((ITestHooksRequest)request).Items.ForEach(i => i.RequestHookMessage += "r5");
            return Task.CompletedTask;
        }
    }

    public class TestContravariantEntityHook : IEntityHook<ICrudlessRequest, IEntity>
    {
        public Task Run(ICrudlessRequest request, IEntity entity, CancellationToken token)
        {
            if (!(request is TestHooksRequest))
                throw new System.Exception("Contravariance Failed");

            ((HookEntity)entity).EntityHookMessage += "e5";
            return Task.CompletedTask;
        }
    }

    public class TestContravariantItemHook : IItemHook<ICrudlessRequest, HookDto>
    {
        public Task<HookDto> Run(ICrudlessRequest request, HookDto item, CancellationToken token)
        {
            if (!(request is TestHooksRequest))
                throw new System.Exception("Contravariance Failed");

            item.ItemHookMessage += "i5";
            return Task.FromResult(item);
        }
    }

    public class TestContravariantResultHook : IResultHook<ICrudlessRequest, string>
    {
        public Task<string> Run(ICrudlessRequest request, string result, CancellationToken token)
        {
            if (!(request is TestHooksRequest))
                throw new System.Exception("Contravariance Failed");

            return Task.FromResult(result + "t5");
        }
    }
}
