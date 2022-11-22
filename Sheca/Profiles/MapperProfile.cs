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
        CreateMap<Course, CourseDto>()
        .ForMember(c => c.DayOfWeeks, opt => opt.MapFrom(source => source.GetDayOfWeeks()))
        .ForMember(c => c.DayOffs, opt => opt.MapFrom(source => source.GetOffDaysList()));
        CreateMap<CreateCourseDto, Course>().ForMember(c => c.DayOfWeeks, opt => opt.MapFrom(source => string.Join(";", source.DayOfWeeks.Select(d => (int)d))));
        CreateMap<UpdateCourseDto, Course>()
            .ForMember(c => c.StartDate, opt => opt.MapFrom((source, des, soureMember) => source.StartDate != null ? source.StartDate : des.StartDate))
            .ForMember(c => c.EndDate, opt => opt.MapFrom((source, des, soureMember) => source.EndDate != null ? source.EndDate : des.EndDate))
            .ForMember(c => c.EndType, opt => opt.MapFrom((source, des, soureMember) => source.EndDate != null ? source.UpdateEndType: des.EndType))
            .ForMember(c => c.DayOfWeeks, opt => opt.MapFrom(source => source.DayOfWeeks != null ? string.Join(";", source.DayOfWeeks.Select(d => (int)d)) : null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        //Event
        _ = CreateMap<Event, EventDto>().ForMember(e => e.RecurringDetails,
        opt => opt.MapFrom(cE => cE.RecurringDetails != null ? cE.RecurringDetails.Split(';', StringSplitOptions.None).Select(d => (DayOfWeek)int.Parse(d)).ToList() : null));

        CreateMap<CreateEventDto, Event>().ForMember(e => e.RecurringDetails,
        opt => opt.MapFrom(cE => cE.RecurringDetails != null ? string.Join(";", cE.RecurringDetails.Select(rD => (int)rD)) : null));
        CreateMap<UpdateEventDto, Event>()
        .ForMember(e => e.RecurringDetails,
        opt => opt.MapFrom(cE => cE.RecurringDetails != null ? string.Join(";", cE.RecurringDetails.Select(rD => (int)rD)) : null))
        .ForMember(e => e.CloneEventId, item => item.Ignore()).ForMember(e => e.Id, item => item.Ignore())
        .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<RegisterUserDto, User>();
        CreateMap<LoginUserDto, User>();
    }
}