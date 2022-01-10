namespace RoboSpinAPI.Managers;

public class RoboSpinConfigurationManager
{

    //create singleton
    private static RoboSpinConfigurationManager _instance;

    private static RoboSpinConfigurationManager Instance => _instance ??= new RoboSpinConfigurationManager();
    
    private readonly IConfigurationRoot _configs;

    private RoboSpinConfigurationManager()
    {
        
        this._configs = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>()
            .Build();
    }
    
    public static string GetConfigurationValue(string key)
    {
        return Instance._configs[key];
    }
}