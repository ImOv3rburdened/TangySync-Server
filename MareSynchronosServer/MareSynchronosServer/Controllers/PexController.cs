using MareSynchronosServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MareSynchronosServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PexController : ControllerBase
    {
        private readonly PeerExchangeService _svc;
        public PexController(PeerExchangeService svc) { _svc = svc; }

        public record PexRegisterDto(string FileHash, int Port, string Area);
        public record PexPeerDto(string Ip, int Port, string Area, string UserUid);

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PexRegisterDto dto)
        {
            string uid = User?.Identity?.Name ?? "unknown";
            string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            await _svc.RegisterAsync(dto.FileHash, uid, ip, dto.Port, dto.Area ?? "");
            return Ok();
        }

        [HttpPost("unregister")]
        public async Task<IActionResult> Unregister([FromBody] PexRegisterDto dto)
        {
            string uid = User?.Identity?.Name ?? "unknown";
            await _svc.UnregisterAsync(dto.FileHash, uid);
            return Ok();
        }

        [HttpGet("{fileHash}")]
        public async Task<ActionResult<IEnumerable<PexPeerDto>>> GetPeers(string fileHash, [FromQuery] string? area = null, [FromQuery] int take = 50)
        {
            string uid = User?.Identity?.Name ?? "unknown";
            var peers = await _svc.GetPeersAsync(fileHash, uid, area, take);
            return Ok(peers.Select(p => new PexPeerDto(p.Ip, p.Port, p.Area, p.UserUid)));
        }
    }
}
