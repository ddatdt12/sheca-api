using AutoMapper;
using Sheca.Dtos;
using Sheca.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<User, UserDTO>();
        CreateMap<UserDTO, User>();
        //CreateMap<CreatePostDto, Post>()
        //    .ForMember(p => p.Tags, options => options.MapFrom(p => string.Join(';', p.Tags).Replace(" ","")));
    }
}