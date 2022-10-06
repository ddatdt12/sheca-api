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
        CreateMap<UpdateEventDto, Event>().ForMember(e => e.CloneEventId, item => item.Ignore()).ForMember(e => e.Id, item => item.Ignore()).ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); ;


        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<RegisterUserDto, User>();
        CreateMap<LoginUserDto, User>();
    }
}