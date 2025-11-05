using Microsoft.AspNetCore.Mvc;
using TransitionSystemApi.Services;

namespace TransitionSystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransitionController : ControllerBase
{
    private readonly TransitionService _service;

    public TransitionController(TransitionService service)
    {
        _service = service;
    }

    [HttpGet("Create")]
    public async Task<IActionResult> GetConnections()
    {
        await _service.TransitionAsync();
        return Redirect("http://localhost:7474/browser/");
    }

    [HttpPost("Create")]
    public async Task<IActionResult> PostConnections([FromBody] Dictionary<string, string> parameters, [FromQuery] List<int> initialStates)
    {
        await _service.TransitionAsync(parameters, initialStates);
        return Redirect("http://localhost:7474/browser/");
    }
}