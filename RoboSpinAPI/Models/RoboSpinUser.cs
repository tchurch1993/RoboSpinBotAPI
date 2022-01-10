using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RoboSpinAPI.Models;

public class RoboSpinUser
{
    [BsonId]
    public string? Id { get; init; }

    [BsonElement("Name")]
    public string Name { get; init; } = null!;

    public BotSettings BotSettings { get; set; } = new BotSettings();

}

public class BotSettings
{
    public string? TwitchBotUsername { get; set; }
    public string? TwitchBotPassword { get; set; }
    public string? TwitchBotPrefix { get; set; }
    public string? ApiSecret { get; set; }
    public string? ApiKey { get; set; }
    public string? AccessToken { get; set; }
    public string? AccessSecret { get; set; }

    public bool IsValid()
    {
        //check if all the fields are filled
        return !string.IsNullOrEmpty(TwitchBotUsername) && !string.IsNullOrEmpty(TwitchBotPassword) && !string.IsNullOrEmpty(TwitchBotPrefix) && !string.IsNullOrEmpty(ApiSecret) && !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(AccessSecret);
    }
}