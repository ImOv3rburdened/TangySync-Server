using MareSynchronos.API.Dto.Files;
using MareSynchronosServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MareSynchronosServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ManifestController : ControllerBase
    {
        private readonly ManifestService _manifests;
        public ManifestController(ManifestService manifests) { _manifests = manifests; }

        [HttpPost]
        public async Task<ActionResult<ManifestDto>> Upsert([FromBody] ManifestDto dto)
        {
            var owner = User?.Identity?.Name ?? "unknown";
            var result = await _manifests.UpsertAsync(dto, owner);
            return Ok(result);
        }

        [HttpGet("{hash}")]
        public async Task<ActionResult<ManifestDto>> Get(string hash)
        {
            var result = await _manifests.GetAsync(hash);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
