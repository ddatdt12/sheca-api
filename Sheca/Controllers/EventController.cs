using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Attributes;
using Sheca.Dtos;
using Sheca.DTOs;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Protect]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
        public EventController(ILogger<EventController> logger, IAuthService authService, IEventService eventService, IMapper mapper)
        {
            _logger = logger;
            _eventService = eventService;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces(typeof(ApiResponse<IEnumerable<EventDto>>))]
        public async Task<IActionResult> Get([FromQuery] FilterEvent filter)
        {
            var userId = HttpContext.Items["UserId"] as string;
            var events = await _eventService.Get(userId!, filter);

            var eventDtos = _mapper.Map<IEnumerable<Event>, IEnumerable<EventDto>>(events);
            return Ok(new ApiResponse<IEnumerable<EventDto>>(eventDtos, "Get Events successfully"));
        }

        [HttpPost]
        [Produces(typeof(ApiResponse<EventDto>))]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto newE)
        {
            var userId = HttpContext.Items["UserId"] as string;
            var @event = await _eventService.Create(newE, userId!);
            return Ok(new ApiResponse<EventDto>(_mapper.Map<EventDto>(@event), "Create event successfully"));
        }

        //[HttpPost("update")]
        //[Produces(typeof(ApiResponse<EventDto>))]
        //public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventDto newE)
        //{
        //    var userId = HttpContext.Items["UserId"] as string;
        //    var @event = await _eventService.Create(newE, userId!);
        //    return Ok(new ApiResponse<EventDto>(_mapper.Map<EventDto>(@event), "Create event successfully"));
        //}

        [HttpDelete("{id}")]
        [Produces(typeof(ApiResponse<EventDto>))]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var userId = HttpContext.Items["UserId"] as string;
            await _eventService.Delete(id, userId!);
            return NoContent();
        }

    }
}