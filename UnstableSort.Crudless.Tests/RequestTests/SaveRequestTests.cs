﻿using AutoMapper;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using UnstableSort.Crudless.Configuration;
using UnstableSort.Crudless.Requests;
using UnstableSort.Crudless.Tests.Fakes;

namespace UnstableSort.Crudless.Tests.RequestTests
{
    [TestFixture]
    public class SaveRequestTests : BaseUnitTest
    {
        [Test]
        public async Task Handle_SaveWithoutResponse_CreatesUser()
        {
            var request = new SaveUserWithoutResponseRequest
            {
                User = new UserDto { Name = "TestUser" }
            };

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());

            var user = Context.Set<User>().FirstOrDefault();
            Assert.IsNotNull(user);
            Assert.AreEqual("TestUser", user.Name);
        }

        [Test]
        public async Task Handle_SaveWithResponse_CreatesUser()
        {
            var request = new SaveUserWithResponseRequest
            {
                Name = "TestUser"
            };

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("TestUser", response.Result.Name);
        }

        [Test]
        public async Task Handle_SaveExistingWithoutResponse_UpdatesUser()
        {
            var existing = new User { Name = "TestUser" };
            Context.Add(existing);

            await Context.SaveChangesAsync();

            var request = new SaveUserWithoutResponseRequest
            { 
                Id = existing.Id,
                User = new UserDto { Name = "NewUser" }
            };

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());

            var user = Context.Set<User>().FirstOrDefault();
            Assert.IsNotNull(user);
            Assert.AreEqual("NewUser", user.Name);
        }

        [Test]
        public async Task Handle_SaveExistingWithResponse_UpdatesUser()
        {
            var existing = new User { Name = "TestUser" };
            Context.Add(existing);

            await Context.SaveChangesAsync();

            var request = new SaveUserWithResponseRequest
            {
                Name = existing.Name
            };

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("TestUser", response.Result.Name);
        }

        [Test]
        public async Task Handle_DefaultSaveWithoutResponseRequest_CreatesUser()
        {
            var request = new SaveRequest<User, UserGetDto>(new UserGetDto
            {
                Name = "NewUser"
            });

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());
            var user = Context.Set<User>().First();
            Assert.IsNotNull(user);
            Assert.AreNotEqual(0, user.Id);
            Assert.AreEqual("NewUser", user.Name);
        }

        [Test]
        public async Task Handle_DefaultSaveWithResponseRequest_CreatesUser()
        {
            var request = new SaveRequest<User, UserGetDto, UserGetDto>(new UserGetDto
            {
                Name = "NewUser"
            });

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.AreEqual(1, Context.Set<User>().Count());
            Assert.IsNotNull(response.Result);
            Assert.AreNotEqual(0, response.Result.Id);
            Assert.AreEqual("NewUser", response.Result.Name);
        }
        
        [Test]
        public async Task Handle_SaveByIdRequest_UpdatesUser()
        {
            var existing = new User { Name = "TestUser" };
            Context.Add(existing);

            await Context.SaveChangesAsync();

            var request = new SaveByIdRequest<User, UserDto, UserGetDto>(
                existing.Id,
                new UserDto { Name = "NewUser" });

            var response = await Mediator.HandleAsync(request);

            Assert.IsFalse(response.HasErrors);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(1, Context.Set<User>().Count());
            Assert.AreEqual(existing.Id, response.Result.Id);
            Assert.AreEqual("NewUser", response.Result.Name);
            Assert.AreEqual("NewUser", Context.Set<User>().First().Name);
        }
    }
    
    public class SaveUserWithResponseRequest : UserDto, ISaveRequest<User, UserGetDto>
    { }
    
    public class SaveUserWithoutResponseRequest : ISaveRequest<User>
    {
        public int Id { get; set; }

        public UserDto User { get; set; }
    }
    
    public class SaveUserWithoutResponseProfile 
        : RequestProfile<SaveUserWithoutResponseRequest>
    {
        public SaveUserWithoutResponseProfile()
        {
            ForEntity<User>()
                .SelectWith(builder => builder.Single(r => r.Id, e => e.Id))
                .CreateEntityWith(request => Mapper.Map<User>(request.User))
                .UpdateEntityWith((request, entity) => Mapper.Map(request.User, entity));
        }
    }

    public class SaveUserWithResponseProfile 
        : RequestProfile<SaveUserWithResponseRequest>
    {
        public SaveUserWithResponseProfile()
        {
            ForEntity<User>()
                .SelectWith(builder => builder.Single("Name"));
        }
    }

    public class DefaultSaveWithoutResponseRequestProfile
        : RequestProfile<SaveRequest<User, UserGetDto>>
    {
        public DefaultSaveWithoutResponseRequestProfile()
        {
            ForEntity<User>()
                .SelectWith(builder => builder.Single(r => e => r.Item.Id == e.Id));
        }
    }

    public class DefaultSaveWithResponseRequestProfile
        : RequestProfile<SaveRequest<User, UserGetDto, UserGetDto>>
    {
        public DefaultSaveWithResponseRequestProfile()
        {
            ForEntity<User>()
                .SelectWith(builder => builder.Single(request => entity => request.Item.Id == entity.Id));
        }
    }
}