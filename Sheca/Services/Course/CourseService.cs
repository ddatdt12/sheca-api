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
            try
            {
                if (!c.EndDate.HasValue && !c.NumOfLessons.HasValue)
                {
                    throw new ApiException("Ngày kết thúc hoặc số tiết phải được cung cấp", 400);
                }

                course.UserId = userId;
                await _context.Courses.AddAsync(course);

                int learnDay = 0;
                if (c.EndDate.HasValue)
                {
                    var totalDays = (c.EndDate - c.StartDate).Value.TotalDays;
                    learnDay = (int)Math.Round(totalDays / 7) + 1;
                    course.NumOfLessons = learnDay * c.NumOfLessonsPerDay * c.DayOfWeeks.Count;
                }
                else if (c.NumOfLessons.HasValue)
                {
                    learnDay = (int)Math.Round((c.NumOfLessons * 1.0 / (c.NumOfLessonsPerDay * c.DayOfWeeks.Count)).Value) + 1;
                    course.EndDate = c.StartDate.AddDays((learnDay - 1) * 7);
                }

                await _context.SaveChangesAsync();
                return course;
            }
            catch (Exception)
            {
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

            try
            {
                _mapper.Map(upCourse, course);
                if (upCourse.NumOfLessonsPerDay.HasValue || upCourse.StartDate.HasValue || upCourse.EndDate.HasValue || upCourse.NumOfLessons.HasValue)
                {
                    int learnDay = 0;
                    int dayOfWeeks = upCourse.DayOfWeeks != null ? upCourse.DayOfWeeks.Count : course.DayOfWeeks.Split(';').ToList().Count;
                    if (upCourse.EndDate.HasValue)
                    {
                        var totalDays = (course.EndDate - course.StartDate).TotalDays;
                        learnDay = (int)Math.Round(totalDays / 7) + 1;

                        course.NumOfLessons = learnDay * course.NumOfLessonsPerDay  * dayOfWeeks;
                    }
                    else if (upCourse.NumOfLessons.HasValue)
                    {
                        learnDay = (int)Math.Round(course.NumOfLessons * 1.0 / (course.NumOfLessonsPerDay * dayOfWeeks)) + 1;
                        course.EndDate = course.StartDate.AddDays((learnDay - 1) * 7);
                    }
                }
                await _context.SaveChangesAsync();
                return course;
            }
            catch (Exception)
            {
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
            try
            {
                _context.Entry(course).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
