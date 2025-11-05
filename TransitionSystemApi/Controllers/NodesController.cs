using Microsoft.AspNetCore.Mvc;
using TransitionSystemApi.Services;

namespace TransitionSystemApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NodesController : ControllerBase
{
    private readonly Neo4jService _neo4jService;

    public NodesController(Neo4jService neo4jService)
    {
        _neo4jService = neo4jService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var nodes = await _neo4jService.GetAllNodesAsync();
        return Ok(nodes);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name)
    {
        var node = await _neo4jService.GetNodeAsync(name);
        if (node == null)
            return NotFound($"Node '{name}' not found.");
        return Ok(node);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string name)
    {
        await _neo4jService.CreateNodeAsync(name);
        return Ok($"Node '{name}' created.");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] string oldName, [FromQuery] string newName)
    {
        var updated = await _neo4jService.UpdateNodeAsync(oldName, newName);
        return updated ? Ok("Updated successfully.") : NotFound("Node not found.");
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var deleted = await _neo4jService.DeleteNodeAsync(name);
        return deleted ? Ok("Deleted successfully.") : NotFound("Node not found.");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteGraph()
    {
        var deleted = await _neo4jService.ClearAllAsync();
        return deleted ? Ok("Deleted successfully.") : NotFound("Graph not found.");
    }
}