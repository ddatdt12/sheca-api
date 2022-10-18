using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos;
using Sheca.Error;
using Sheca.Models;

namespace Sheca.Services
{
    public class CourseService : ICourseService
    {
        public DataContext _context { get; }
        private readonly IMapper _mapper;
        public CourseService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Course>> Get(Guid userId)
        {
            return await _context.Courses.Where(c => c.UserId == userId).ToListAsync();
        }
        public async Task<Course> Create(CreateCourseDto c, Guid userId)
        {
            Course course = _mapper.Map<Course>(c);
            var transaction = _context.Database.BeginTransaction();
            try
            {
                if (!c.EndDate.HasValue && !c.NumOfLessons.HasValue)
                {
                    throw new ApiException("Ngày kết thúc hoặc số tiết phải được cung cấp", 400);
                }

                course.UserId = userId;
                await _context.Courses.AddAsync(course);

                int learnDay = 0;
                List<Event> evs = new List<Event>();
                if (c.EndDate.HasValue)
                {
                    var totalDays = (c.EndDate - c.StartDate).Value.TotalDays;
                    learnDay = (int)Math.Round(totalDays / 7) + 1;
                    course.NumOfLessons = learnDay * c.NumOfLessonsPerDay;
                }
                else if (c.NumOfLessons.HasValue)
                {
                    learnDay = (int)Math.Round((c.NumOfLessons * 1.0 / c.NumOfLessonsPerDay).Value) + 1;
                    course.EndDate = c.StartDate.AddDays((learnDay - 1) * 7);
                }

                await _context.SaveChangesAsync();

                for (int i = 0; i < learnDay; i++)
                {
                    var startTime = c.StartDate.AddDays(i * 7).AddSeconds(c.StartTime);
                    var endTime = c.StartDate.AddDays(i * 7).AddSeconds(c.EndTime);
                    evs.Add(new Event
                    {
                        NotiBeforeTime = c.NotiBeforeTime,
                        ColorCode = c.ColorCode,
                        CourseId = course.Id,
                        Title = c.Title,
                        Description = c.Description,
                        StartTime = startTime,
                        EndTime = endTime,
                        UserId = userId
                    });
                }

                await _context.BulkInsertAsync(evs);
                await transaction.CommitAsync();
                return course;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        async Task<Course> ICourseService.Update(int id, UpdateCourseDto upCourse, Guid userId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course is null || course.UserId != userId)
            {
                throw new ApiException("Course not found", 404);
            }

            var transaction = _context.Database.BeginTransaction();
            try
            {
                _mapper.Map(upCourse, course);
                if (upCourse.NumOfLessonsPerDay.HasValue || upCourse.StartDate.HasValue || upCourse.EndDate.HasValue || upCourse.NumOfLessons.HasValue)
                {
                    int learnDay = 0;
                    if (upCourse.EndDate.HasValue)
                    {
                        var totalDays = (course.EndDate - course.StartDate).TotalDays;
                        learnDay = (int)Math.Round(totalDays / 7) + 1;
                        course.NumOfLessons = learnDay * course.NumOfLessonsPerDay;
                    }
                    else if (upCourse.NumOfLessons.HasValue)
                    {
                        learnDay = (int)Math.Round((course.NumOfLessons * 1.0 / course.NumOfLessonsPerDay)) + 1;
                        course.EndDate = course.StartDate.AddDays((learnDay - 1) * 7);
                    }

                    for (int i = 0; i < learnDay; i++)
                    {
                        var startTime = course.StartDate.AddDays(i * 7).AddSeconds(course.StartTime);
                        var endTime = course.StartDate.AddDays(i * 7).AddSeconds(course.EndTime);
                        _context.Events.Add(new Event
                        {
                            NotiBeforeTime = course.NotiBeforeTime,
                            ColorCode = course.ColorCode,
                            CourseId = course.Id,
                            Title = course.Title,
                            Description = course.Description,
                            StartTime = startTime,
                            EndTime = endTime,
                        });
                    }
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"DELETE  FROM Course WHERE CourseId  = {course.Id}");
                }
                await _context.BulkSaveChangesAsync();
                await transaction.CommitAsync();
                return course;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(int id, Guid userId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course is null || course.UserId != userId)
            {
                throw new ApiException("Course not found", 404);
            }
            var transaction = _context.Database.BeginTransaction();
            try
            {

                await _context.Database.ExecuteSqlInterpolatedAsync(
                            $"DELETE  FROM Course WHERE CourseId  = {course.Id}");
                _context.Entry(course).State = EntityState.Deleted;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}
