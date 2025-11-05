🧩 Transition System API
========================

`![Graph Example](graph-example.jpeg)`

A C# (.NET 8) project that models and explores state transitions between processes using Neo4j as a graph database.  
The project automatically builds a transition graph by exploring states, evaluating conditions, and applying expressions dynamically.

------------------------------------------------------------

🚀 Overview
------------

The TransitionSystemApi simulates multiple parallel processes that change states according to defined conditions and expressions.  
Each process is represented by:
- A program counter (PC) like n0, t0, c0  
- A condition (e.g., i < 2, i > 0)  
- An expression that updates variables (e.g., i++, i--, i=0)

The transitions between states are stored and visualized in Neo4j, allowing you to observe how the system evolves over time.

------------------------------------------------------------

🧠 Example Concept
------------------

Two processes p0 and p1 run in parallel:

p0:  
n0 while true  
s0 wait i < 2  
c0 i++

p1:  
n1 while true  
s1 wait i > 0  
c1 i--

They interact with a shared variable i, and their transitions are represented as graph nodes and edges.

------------------------------------------------------------

⚙️ Architecture
---------------

Main Components:

- TransitionService — Core service that explores all possible transitions and interacts with Neo4j  
- Neo4jService — Handles connection, node, and relationship creation in Neo4j  
- State — Represents a program state with process counters, conditions, and expressions  
- Transition — Represents a connection (edge) between two states

------------------------------------------------------------

🧩 How It Works
----------------

1. Define transitions — specify condition → expression pairs (e.g., "<2" : "++")  
2. Set initial states — define initial integer values for variable(s)  
3. Run exploration — the service recursively explores all reachable states  
4. Neo4j Integration — each unique state becomes a node, and each transition becomes an edge

------------------------------------------------------------

🧰 Code Example
---------------

public async Task TransitionAsync()  
{  
    Dictionary<string, string> transitions = new()  
    {  
        { "<2", "++" },  
        { ">0", "--" }  
    };  

    List<int> initialStates = [0, 1];  

    await Explore(transitions, initialStates);  
}

Exploration Logic:  
- DoCondition() dynamically evaluates conditions (like i < 2) using DataTable.Compute()  
- DoExpression() updates the shared variable (like i++)  
- NextPc() determines the next process counter (n, t, or c state)  
- Explore() recursively traverses the system and writes nodes/edges to Neo4j

------------------------------------------------------------

🔗 Graph Representation (Neo4j)
-------------------------------

Each state is stored as a node:
(1,n0,n1)

Each transition is stored as a relationship:
(1,n0,n1) -[:P0]-> (2,t0,n1)

------------------------------------------------------------

🧮 Example Output
-----------------

After running TransitionAsync(), the graph might look like this:

(0,n0,n1)  
 ├── P0 → (1,c0,n1)  
 └── P1 → (0,n0,t1)

This visualizes how processes interact and evolve over time.


`![Graph Example](graph-example-1.jpeg)`

------------------------------------------------------------

🧑‍💻 Project Structure
-----------------------

TransitionSystemApi/  
│  
├── Models/  
│   ├── State.cs          — Represents each state node  
│   └── Transition.cs     — Represents transitions between states  
│  
├── Services/  
│   ├── TransitionService.cs  — Main logic and Neo4j interaction  
│   └── Neo4jService.cs       — Handles Neo4j CRUD  
│  
└── Program.cs (if exists)

------------------------------------------------------------

🛠️ Technologies Used
---------------------

- .NET 8  
- C#  
- Neo4j.Driver  
- System.Data.DataTable  
- Async/Await for asynchronous graph operations

------------------------------------------------------------

⚡ Future Improvements
-----------------------

- Add variable tracking for multiple shared variables  
- Extend Neo4j visualization with process-specific colors  
- Add unit tests for condition and expression parsing  
- Create a frontend (React + Neo4j visualization)

------------------------------------------------------------

📚 Usage
--------

1. Configure your Neo4j connection in Neo4jService.  
2. Run TransitionAsync() from a controller or test environment.  
3. View results in Neo4j Browser (http://localhost:7474).

------------------------------------------------------------