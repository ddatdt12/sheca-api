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
        CreateMap<Course, CourseDto>().ForMember(c => c.DayOfWeeks, opt => opt.MapFrom(source => source.DayOfWeeks.Split(';', StringSplitOptions.None).Select(d => (DayOfWeek)int.Parse(d))));
        CreateMap<CreateCourseDto, Course>().ForMember(c => c.DayOfWeeks, opt => opt.MapFrom(source => string.Join(";", source.DayOfWeeks.Select(d => (int)d))));
        CreateMap<UpdateEventDto, Course>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        //Event
        _ = CreateMap<Event, EventDto>().ForMember(e => e.RecurringDetails,
        opt => opt.MapFrom(cE => cE.RecurringDetails != null ? cE.RecurringDetails.Split(';', StringSplitOptions.None).Select(d => (DayOfWeek)int.Parse(d)).ToList() : null));

        CreateMap<CreateEventDto, Event>().ForMember(e => e.RecurringDetails,
        opt => opt.MapFrom(cE => cE.RecurringDetails != null ? string.Join(";", cE.RecurringDetails.Select(rD => (int)rD)) : null));
        CreateMap<UpdateEventDto, Event>()
        .ForMember(e => e.CloneEventId, item => item.Ignore()).ForMember(e => e.Id, item => item.Ignore())
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<RegisterUserDto, User>();
        CreateMap<LoginUserDto, User>();
    }
}