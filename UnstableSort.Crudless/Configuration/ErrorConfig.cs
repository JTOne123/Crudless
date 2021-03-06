﻿using System;
using System.Collections.Generic;
using UnstableSort.Crudless.Errors;

namespace UnstableSort.Crudless.Configuration
{
    public class ErrorConfig
    {
        private Func<IErrorHandler> _requestErrorHandlerFactory;

        private readonly Dictionary<Type, Func<IErrorHandler>> _errorHandlerFactoryCache
            = new Dictionary<Type, Func<IErrorHandler>>();

        private readonly Dictionary<Type, Func<IErrorHandler>> _errorHandlerFactories
            = new Dictionary<Type, Func<IErrorHandler>>();

        public bool FailedToFindInGetIsError { get; set; } = true;

        public bool FailedToFindInGetAllIsError { get; set; }

        public bool FailedToFindInFindIsError { get; set; } = true;

        public bool FailedToFindInUpdateIsError { get; set; } = true;

        public bool FailedToFindInDeleteIsError { get; set; }

        public IErrorHandler GetErrorHandlerFor<TEntity>()
            where TEntity : class
        {
            if (_errorHandlerFactoryCache.TryGetValue(typeof(TEntity), out var cachedFactory))
                return cachedFactory?.Invoke();

            foreach (var tEntity in typeof(TEntity).BuildTypeHierarchyUp())
            {
                if (_errorHandlerFactories.TryGetValue(tEntity, out var factory))
                {
                    _errorHandlerFactoryCache[typeof(TEntity)] = factory;
                    return factory();
                }
            }

            _errorHandlerFactoryCache[typeof(TEntity)] = _requestErrorHandlerFactory;

            return _requestErrorHandlerFactory?.Invoke();
        }

        internal void SetErrorHandler(Func<IErrorHandler> factory)
        {
            _requestErrorHandlerFactory = factory;
        }

        internal void SetErrorHandlerFor(Type tEntity, Func<IErrorHandler> factory)
        {
            _errorHandlerFactories[tEntity] = factory;
        }
    }
}