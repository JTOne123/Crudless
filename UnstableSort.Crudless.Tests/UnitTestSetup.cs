﻿using System;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using UnstableSort.Crudless.EntityFrameworkExtensions;
using UnstableSort.Crudless.Tests.Fakes;
using UnstableSort.Crudless.Tests.Utilities;

namespace UnstableSort.Crudless.Tests
{
    [SetUpFixture]
    public class UnitTestSetUp
    {
        public static Container Container { get; private set; }

        [OneTimeSetUp]
        public static void UnitTestOneTimeSetUp()
        {
            var assemblies = new[] { typeof(UnitTestSetUp).Assembly };
            Container = new Container();

            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            ConfigureDatabase(Container);
            ConfigureAutoMapper(Container, assemblies);
            ConfigureFluentValidation(Container, assemblies);
            
            // NOTE: License removed from repository

            //if (!LicenseManager.ValidateLicense(out var licenseErrorMessage))
            //{
            //    throw new Exception(licenseErrorMessage);
            //}
            
            Crudless.CreateInitializer(Container, assemblies)
                .ValidateAllRequests(false)
                .UseFluentValidation()
                .UseEntityFrameworkExtensions(BulkExtensions.Create | BulkExtensions.Update)
                .AddInitializer(new SoftDeleteInitializer())
                .Initialize();
            
            Container.Verify();
        }

        public static void ConfigureDatabase(Container container)
        {
            container.Register<DbContext>(() =>
            {
                var options = new DbContextOptionsBuilder<FakeDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

                return new FakeDbContext(options);
            }, 
            Lifestyle.Scoped);
        }

        public static void ConfigureAutoMapper(Container container, Assembly[] assemblies)
        {
            Mapper.Reset();
            Mapper.Initialize(config =>
            {
                config.AddMaps(assemblies);
            });
        }

        public static void ConfigureFluentValidation(Container container, Assembly[] assemblies)
        {
            container.Register(typeof(IValidator<>), assemblies);

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }
    }
}
