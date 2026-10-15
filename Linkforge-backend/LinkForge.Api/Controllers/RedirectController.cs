using LinkForge.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkForge.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController : ControllerBase
    {
        private readonly ILinkService _linkService;

        public RedirectController(ILinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode)
        {
            var originalUrl =
                await _linkService.GetOriginalUrlForRedirectAsync(shortCode);

            return Redirect(originalUrl);
        }
    }
}
