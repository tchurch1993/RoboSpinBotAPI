namespace RoboSpinAPI.Interfaces;

public interface IChatCommand
{
    public void Execute(IEnumerable<string> args, string channel, string user);
    public string CommandName { get; }
    public string Description { get; }
}