namespace TransitionSystemApi.Models;

class Transition : IEquatable<Transition>
{
    public State From { get; }
    public State To { get; }
    public string Process { get; }

    public Transition(State from, State to, string process)
    {
        From = from;
        To = to;
        Process = process;
    }

    public override string ToString() => $"{From.ToString()}, {To.ToString()}, {Process}";

    public override bool Equals(object obj) => Equals(obj as Transition);

    public bool Equals(Transition other) =>
        other != null && From.ToString() == other.From.ToString() && From.ToString() == other.From.ToString() && Process == other.Process;

    public override int GetHashCode() => HashCode.Combine(From.ToString(), To.ToString(), Process);
}