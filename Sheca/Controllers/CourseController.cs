using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
using Sheca.DTOs;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        public CourseController(IAuthService authService, IMapper mapper, ICourseService courseService)
        {
            _mapper=mapper;
            _courseService=courseService;
        }



        [HttpGet]
        [Produces(typeof(ApiResponse<IEnumerable<CourseDto>>))]
        public async Task<IActionResult> Get()
        {
            var courses = await _courseService.Get();

            var courseDtos = _mapper.Map<IEnumerable<Course>, IEnumerable<CourseDto>>(courses);
            return Ok(new ApiResponse<IEnumerable<CourseDto>>(courseDtos, "Get Courses successfully"));
        }

        [HttpPost]
        [Produces(typeof(ApiResponse<CourseDto>))]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto course)
        {
            var c = await _courseService.Create(course);
            return Ok(new ApiResponse<CourseDto>(_mapper.Map<CourseDto>(c), "Create course successfully"));
        }
        
        [HttpPut("{id}")]
        [Produces(typeof(ApiResponse<CourseDto>))]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto course)
        {
            var c = await _courseService.Update(id, course);
            return Ok(new ApiResponse<CourseDto>(_mapper.Map<CourseDto>(c), "Update course successfully"));
        }

    }
}