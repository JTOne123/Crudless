﻿using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace UnstableSort.Crudless.Tests.Fakes
{
    public interface IEntity
    {
        int Id { get; set; }

        bool IsDeleted { get; set; }
    }
    
    public class Entity : IEntity
    {
        [Key, Required]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    public class User : Entity
    {
        public string Name { get; set; }
    }
    
    public class UserDto
    {
        public string Name { get; set; }
    }
    
    public class UserGetDto : UserDto
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserGetInlineDto : UserDto
    {
        public int Id { get; set; }
    }

    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            CreateMap<User, User>();

            CreateMap<UserDto, User>()
                .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<User, UserGetDto>();

            CreateMap<User, UserGetInlineDto>();
        }
    }

    public class UserClaim : Entity
    {
        public int UserId { get; set; }

        public string Claim { get; set; }

        public string Value { get; set; }
    }

    public class UserClaimDto
    {
        public int UserId { get; set; }

        public string Claim { get; set; }

        public string Value { get; set; }
    }

    public class UserClaimGetDto : UserClaimDto
    {
        public int Id { get; set; }
    }

    public class UserClaimProfiles : Profile
    {
        public UserClaimProfiles()
        {
            CreateMap<UserClaimDto, UserClaim>();
            CreateMap<UserClaim, UserClaimGetDto>();
        }
    }

    public class Site : Entity
    {
        [Required]
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class SiteDto
    {
        public Guid Guid { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class SiteGetDto : SiteDto
    {
        public int Id { get; set; }
    }

    public class SiteProfiles : Profile
    {
        public SiteProfiles()
        {
            CreateMap<SiteDto, Site>()
                .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<Site, SiteGetDto>();
        }
    }

    public class NonEntity
    {
        [Key, Required]
        public int Id { get; set; }
    }

    public class CompositeKeyEntity
    {
        [Key, Required]
        public int IntPart { get; set; }

        [Key, Required]
        public Guid GuidPart { get; set; }

        public string Name { get; set; }
    }

    public interface IHookEntity
    {
        string RequestHookMessage { get; set; }

        string EntityHookMessage { get; set; }

        string ItemHookMessage { get; set; }
    }

    public class HookAutoMapperProfile : Profile
    {
        public HookAutoMapperProfile()
        {
            CreateMap<HookDto, HookEntity>().ReverseMap();
        }
    }

    public class HookEntity : Entity, IHookEntity
    {
        public string RequestHookMessage { get; set; }

        public string EntityHookMessage { get; set; }

        public string ItemHookMessage { get; set; }
    }
}
