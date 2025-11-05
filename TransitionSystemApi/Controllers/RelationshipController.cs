using Microsoft.AspNetCore.Mvc;
using TransitionSystemApi.Services;

namespace TransitionSystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectionsController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public ConnectionsController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var connections = await _neo4jService.GetAllConnectionsAsync();
        return Ok(connections);
    }

    [HttpGet("{fromName}/{toName}")]
    public async Task<IActionResult> Get(string fromName, string toName)
    {
        var type = await _neo4jService.GetConnectionAsync(fromName, toName);
        if (type == null)
            return NotFound("Connection not found.");
        return Ok(new { From = fromName, To = toName, Type = type });
    }

    [HttpGet("of/{name}")]
    public async Task<IActionResult> GetConnectionsOfNode(string name)
    {
        var connections = await _neo4jService.GetConnectionsOfNodeAsync(name);
        return Ok(connections);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] string fromName, [FromQuery] string toName, [FromQuery] string type)
    {
        await _neo4jService.CreateConnectionAsync(fromName, toName, type);
        return Ok($"Connection {fromName} -[:{type}]-> {toName} created.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] string fromName, [FromQuery] string toName, [FromQuery] string newType)
    {
        var updated = await _neo4jService.UpdateConnectionAsync(fromName, toName, newType);
        return updated ? Ok("Connection updated.") : NotFound("Connection not found.");
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string fromName, [FromQuery] string toName)
    {
        var deleted = await _neo4jService.DeleteConnectionAsync(fromName, toName);
        return deleted ? Ok("Connection deleted.") : NotFound("Connection not found.");
    }
}