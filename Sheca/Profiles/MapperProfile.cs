using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);

        //CreateMap<User, UserDto>();
        //CreateMap<UserDto, User>();
        //CreateMap<CreatePostDto, Post>()
        //    .ForMember(p => p.Tags, options => options.MapFrom(p => string.Join(';', p.Tags).Replace(" ","")));
    }
}