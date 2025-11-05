using Neo4j.Driver;

namespace TransitionSystemApi.Services;

public class Neo4jService
{
    private readonly IDriver _driver;

    public Neo4jService(IDriver driver)
    {
        _driver = driver;
    }

    // ======================
    // PERSON CRUD
    // ======================

    public async Task CreateNodeAsync(string name)
    {
        await using var session = _driver.AsyncSession();
        await session.RunAsync(@"
            CREATE (n:Person { name: $name })
        ", new { name });
    }

    public async Task<string?> GetNodeAsync(string name)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (n:Person { name: $name })
            RETURN n.name AS name
        ", new { name });

        var record = await result.SingleAsync();
        return record?["name"].As<string>();
    }

    public async Task<IEnumerable<string>> GetAllNodesAsync()
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (n:Person)
            RETURN n.name AS name
        ");

        var nodes = new List<string>();
        await foreach (var record in result)
            nodes.Add(record["name"].As<string>());

        return nodes;
    }

    public async Task<bool> UpdateNodeAsync(string oldName, string newName)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (n:Person { name: $oldName })
            SET n.name = $newName
            RETURN COUNT(n) AS updated
        ", new { oldName, newName });

        var record = await result.SingleAsync();
        return record["updated"].As<long>() > 0;
    }

    public async Task<bool> DeleteNodeAsync(string name)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (n:Person { name: $name })
            DETACH DELETE n
            RETURN COUNT(n) AS deleted
        ", new { name });

        var record = await result.SingleAsync();
        return record["deleted"].As<long>() > 0;
    }

    // ======================
    // CONNECTION CRUD
    // ======================

    public async Task CreateConnectionAsync(string fromName, string toName, string relationType)
    {
        await using var session = _driver.AsyncSession();
        await session.RunAsync(@"
            MATCH (a:Person { name: $fromName })
            MATCH (b:Person { name: $toName })
            MERGE (a)-[r:RELATION { type: $relationType }]->(b)
        ", new { fromName, toName, relationType });
    }

    public async Task<string?> GetConnectionAsync(string fromName, string toName)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (a:Person { name: $fromName })-[r:RELATION]->(b:Person { name: $toName })
            RETURN r.type AS type
        ", new { fromName, toName });

        var record = await result.SingleAsync();
        return record?["type"].As<string>();
    }

    public async Task<IEnumerable<(string From, string To, string Type)>> GetAllConnectionsAsync()
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (a:Person)-[r:RELATION]->(b:Person)
            RETURN a.name AS from, b.name AS to, r.type AS type
        ");

        var connections = new List<(string, string, string)>();
        await foreach (var record in result)
        {
            connections.Add((
                record["from"].As<string>(),
                record["to"].As<string>(),
                record["type"].As<string>()
            ));
        }

        return connections;
    }

    public async Task<IEnumerable<(string From, string To, string Type)>> GetConnectionsOfNodeAsync(string name)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (a:Person { name: $name })-[r:RELATION]->(b:Person)
            RETURN a.name AS from, b.name AS to, r.type AS type
        ", new { name });

        var connections = new List<(string, string, string)>();
        await foreach (var record in result)
        {
            connections.Add((
                record["from"].As<string>(),
                record["to"].As<string>(),
                record["type"].As<string>()
            ));
        }

        return connections;
    }

    public async Task<bool> UpdateConnectionAsync(string fromName, string toName, string newType)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (a:Person { name: $fromName })-[r:RELATION]->(b:Person { name: $toName })
            SET r.type = $newType
            RETURN COUNT(r) AS updated
        ", new { fromName, toName, newType });

        var record = await result.SingleAsync();
        return record["updated"].As<long>() > 0;
    }

    public async Task<bool> DeleteConnectionAsync(string fromName, string toName)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.RunAsync(@"
            MATCH (a:Person { name: $fromName })-[r:RELATION]->(b:Person { name: $toName })
            DELETE r
            RETURN COUNT(r) AS deleted
        ", new { fromName, toName });

        var record = await result.SingleAsync();
        return record["deleted"].As<long>() > 0;
    }

    // ======================
    // UTILITS
    // ======================

    public async Task<bool> ClearAllAsync()
    {
        await using var session = _driver.AsyncSession();

        await session.RunAsync("MATCH (n) DETACH DELETE n");

        return true;
    }
}