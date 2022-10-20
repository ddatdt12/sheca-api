using Sheca.Dtos;
using Sheca.Models;

namespace Sheca.Services
{
    public interface ICourseService
    {
        public Task<IEnumerable<Course>> Get(Guid userId);
        public Task<Course> Create(CreateCourseDto c, Guid userId);
        public Task<Course> Update(int id, UpdateCourseDto ev, Guid userId);
        public Task Delete(int id, Guid userId);
    }
}
