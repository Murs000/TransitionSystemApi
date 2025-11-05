using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;
using TransitionSystemApi.Models;

namespace TransitionSystemApi.Services;

/// p0 
/// n0 while true
/// s0 whait i<2
/// c0 i++
/// 
/// p1 
/// n1 while true
/// s1 whait i>0
/// c1 i--

public class TransitionService
{
    private readonly Neo4jService _service;
    public TransitionService(Neo4jService service)
    {
        _service = service;
    }
    
    private HashSet<State> visited = new HashSet<State>();
    private HashSet<Transition> visitedConnection = new HashSet<Transition>();

    public async Task TransitionAsync()
    {
        Dictionary<string, string> transitions = new Dictionary<string, string>
        {
            { "=1", "=0" },
            { "=0", "=1" }
        };

        List<int> initialStates = [0, 1];

        await Explore(transitions, initialStates);
    }

    public async Task TransitionAsync(Dictionary<string, string> transitions, List<int> initialStates)
    {
        await Explore(transitions, initialStates);
    }
    
    // Transition 
    // ex:
    // <2 --- ++
    // >0 --- -- 
    // ==2 --- =0
    private async Task Explore(Dictionary<string, string> transitions, List<int> initialStates)
    {
        //ClearAll
        await _service.ClearAllAsync();

        Dictionary<string, string> pCs = new Dictionary<string, string>();
        Dictionary<string, string> conditions = new Dictionary<string, string>();
        Dictionary<string, string> expressions = new Dictionary<string, string>();
        int i = 0;

        foreach (var transition in transitions.Keys)
        {
            pCs.Add($"{i}", $"n{i}");
            conditions.Add($"{i}", transition);
            expressions.Add($"{i}", transitions[transition]);
            i++;
        }

        // Explore initial turn

        foreach (var state in initialStates)
        {
            State initialstate = new State(state, pCs, conditions, expressions);

            if (!visited.Contains(initialstate))
            {
                await _service.CreateNodeAsync(initialstate.ToString());
            }
            
            visited.Add(initialstate);

            await Explore(initialstate);
        }
    }
    private async Task Explore(State state)
    {
        foreach (var key in state.PCs.Keys)
        {
            await Explore(state, key);
        }
    }

    private async Task Explore(State state, string pC)
    {
        (string nextPc, int nextTurn) = NextPc(pC, state);

        State nextState = state;

        Dictionary<string, string> newPCs = new Dictionary<string, string>();

        foreach (var key in state.PCs.Keys)
        {
            if (key == pC)
            {
                newPCs.Add(key, nextPc);
            }
            else
            {
                newPCs.Add(key, state.PCs[key]);
            }
        }
        nextState = new State(nextTurn, newPCs, state.Conditions, state.Expressions);

        if (!visited.Contains(nextState))
        {
            await _service.CreateNodeAsync(nextState.ToString());
            visited.Add(nextState);
        }

        Transition transition = new Transition(state, nextState, $"P{pC}");

        if (!visitedConnection.Contains(transition))
        {
            await _service.CreateConnectionAsync(state.ToString(), nextState.ToString(), $"P{pC}");
            visitedConnection.Add(transition);

            await Explore(nextState);
        }
    }

    private (string nextPc, int nextTurn) NextPc(string pc, State state)
    {
        string nextPc = string.Empty;
        int nextTurn = state.Turn;

        string currState = state.PCs[pc];

        if ($"n{pc}" == currState)
        {
            nextPc = $"t{pc}";
        }
        else if ($"t{pc}" == currState)
        {
            if (DoCondition(pc, state))
            {
                nextPc = $"c{pc}";
            }
            else
            {
                nextPc = currState;
            }
        }
        else if ($"c{pc}" == currState)
        {
            nextTurn = DoExpression(pc, state);
            nextPc = $"n{pc}";
        }

        return (nextPc, nextTurn);
    }

    private bool DoCondition(string pC, State state)
    {
        int i = state.Turn;
        string expression = $"{i}" + state.Conditions[pC]; // dynamically create expression

        return (bool)new DataTable().Compute(expression, "");
    }
    private int DoExpression(string pC, State state)
    {
        int i = state.Turn;
        string expression = state.Expressions[pC];
        if (expression.Contains('-'))
        {
            int element = Int32.Parse(expression.Substring(expression.IndexOf('-') +1 , 1));

            i -= element;
        }
        else if (expression.Contains('+'))
        {
            int element = Int32.Parse(expression.Substring(expression.IndexOf('+') + 1, 1));

            i += element;
        }
        else if (expression.Contains(':'))
        {
            int element = Int32.Parse(expression.Substring(expression.IndexOf(':') + 1, 1));

            i /= element;
        }
        else if (expression.Contains('*'))
        {
            int element = Int32.Parse(expression.Substring(expression.IndexOf('*') + 1, 1));

            i *= element;
        }
        else if (expression.Contains('='))
        {

            int element = Int32.Parse(expression.Substring(expression.IndexOf('=') + 1, 1));

            i = element;
        }
        
        return i;
    }

}