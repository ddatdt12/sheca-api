using AutoMapper;
using Sheca.Dtos;
using Sheca.Models;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);

        //Course
        CreateMap<CourseDto, Course>();
        CreateMap<Course, CourseDto>();
        CreateMap<CreateCourseDto, Course>();
        CreateMap<UpdateEventDto, Course>();

        //Event
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>();


        CreateMap<User, UserDTO>();
        CreateMap<UserDTO, User>();
        //CreateMap<CreatePostDto, Post>()
        //    .ForMember(p => p.Tags, options => options.MapFrom(p => string.Join(';', p.Tags).Replace(" ","")));

    }
}