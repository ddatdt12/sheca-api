using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sheca.Dtos;
using Sheca.DTOs;
using Sheca.Models;
using Sheca.Services;

namespace Sheca.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
        public EventController(ILogger<EventController> logger, IAuthService authService, IEventService eventService, IMapper mapper)
        {
            _logger = logger;
            _eventService=eventService;
            _mapper=mapper;
        }



        [HttpGet]
        [Produces(typeof(ApiResponse<IEnumerable<EventDto>>))]
        public async Task<IActionResult> Get()
        {
            var events = await _eventService.Get();

            var eventDtos = _mapper.Map<IEnumerable<Event>, IEnumerable<EventDto>>(events);
            return Ok(new ApiResponse<IEnumerable<EventDto>>(eventDtos, "Get Events successfully"));
        }

        [HttpPost]
        [Produces(typeof(ApiResponse<EventDto>))]
        public IActionResult CreateEvent([FromBody] CreateEventDto newE)
        {
            return Ok();
        }

    }
}