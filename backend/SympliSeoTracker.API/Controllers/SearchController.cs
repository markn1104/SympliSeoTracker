using MediatR;
using Microsoft.AspNetCore.Mvc;
using SympliSeoTracker.Domain.Models;

namespace SympliSeoTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;       
        public SearchController(IMediator mediator)
        {
            _mediator = mediator;           
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}