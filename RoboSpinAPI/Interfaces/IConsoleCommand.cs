#nullable enable
namespace RoboSpinAPI.Interfaces;

public interface IConsoleCommand : IComparable<IConsoleCommand>
{
    public void Execute(IEnumerable<string> args);
    public string CommandName { get; }
    public string Description { get; }

    //didnt know you can do this in an interface. should i have done it? who the hell knows, but it gets the job done.
    int IComparable<IConsoleCommand>.CompareTo(IConsoleCommand? other)
    {
        return string.Compare(CommandName, other?.CommandName, StringComparison.Ordinal);
    }
}