namespace RoboSpinAPI.Models;

public class RoboSpinDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string RoboSpinCollection { get; set; } = null!;
}