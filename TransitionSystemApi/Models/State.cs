namespace TransitionSystemApi.Models;

class State : IEquatable<State>
{
    public int Turn { get; }

    // 1 - n1
    // 2 - n2
    // 3 - n3
    public Dictionary<string, string> PCs { get; } = new Dictionary<string, string>();

    // 1 - > i
    // 2 - < i 
    // 3 - == i
    // 4 - <= i 
    // 5 - >= i
    // 6 - != i
    public Dictionary<string, string> Conditions { get; } = new Dictionary<string, string>();

    // 1 - --
    // 2 - ++ 
    // 3 - += i
    // 4 - -= i 
    // 5 - = i
    public Dictionary<string, string> Expressions = new Dictionary<string, string>();

    public State(int turn, Dictionary<string, string> pCs, Dictionary<string, string> coditions, Dictionary<string, string> expressions)
    {
        Turn = turn;
        PCs = pCs;
        Conditions = coditions;
        Expressions = expressions;
    }

    public override string ToString() => $"({Turn},{string.Join(',', PCs.Values)})";

    public override bool Equals(object obj) => Equals(obj as State);

    public bool Equals(State other) =>
        other != null && Turn == other.Turn && string.Join(',', PCs.Values) == string.Join(',', other.PCs.Values);

    public override int GetHashCode() => HashCode.Combine(Turn, ",", string.Join(',', PCs));
}