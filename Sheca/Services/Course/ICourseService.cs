using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public interface ICourseService
    {
        public Task<IEnumerable<Course>> Get();
        public Task<Course> Create(CreateCourseDto c);
        public Task<Course> Update(int id, UpdateCourseDto ev);
    }
}
