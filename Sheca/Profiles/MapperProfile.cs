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
        CreateMap<UpdateEventDto, Course>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        //Event
        CreateMap<Event, EventDto>()
            .ForMember(e => e.RecurringType.Value, opt => opt.MapFrom(src => src.RecurringInterval != null ? src.RecurringType.RecurringUnit.ToString() : null))
            .ForMember(e => e.RecurringInterval, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Value : 0))
            .ForMember(e => e.RecurringDetails, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Details : null));
        CreateMap<CreateEventDto, Event>().ForMember(e => e.RecurringUnit, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.RecurringUnit.ToString() : null))
            .ForMember(e => e.RecurringInterval, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Value : 0))
            .ForMember(e => e.RecurringDetails, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Details : null));
        CreateMap<UpdateEventDto, Event>()
            .ForMember(e => e.CloneEventId, item => item.Ignore()).ForMember(e => e.Id, item => item.Ignore())
            .ForMember(e => e.RecurringUnit, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.RecurringUnit.ToString() : null))
            .ForMember(e => e.RecurringInterval, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Value : 0))
            .ForMember(e => e.RecurringDetails, opt => opt.MapFrom(src => src.RecurringType != null ? src.RecurringType.Details : null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<RegisterUserDto, User>();
        CreateMap<LoginUserDto, User>();
    }
}