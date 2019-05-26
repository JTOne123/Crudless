using System;

namespace UnstableSort.Crudless.Mediator.Hangfire
{
    public interface IBackgroundJobMediator
    {
        void Enqueue<TRequest, TResult>(TRequest command)
            where TRequest : IRequest<TResult>;

        void Schedule<TRequest, TResult>(TRequest command, TimeSpan delay)
            where TRequest : IRequest<TResult>;

        void Schedule<TRequest, TResult>(string name, TRequest command, string cron)
            where TRequest : IRequest<TResult>;
    }
}