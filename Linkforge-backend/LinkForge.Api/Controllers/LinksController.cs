using LinkForge.Application.DTOs.Links;
using LinkForge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinksController : ControllerBase
    {
        private readonly ILinkService _linkService;

        public LinksController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortLink([FromBody] CreateShortLinkRequest request)
        {
            var response = await _linkService.CreateShortLinkAsync(request);

            return Ok(response);
        }
    }
}
