using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RoboSpinAPI.Models;

namespace RoboSpinAPI.Services;

public class RoboSpinUsersService
{
    private readonly IMongoCollection<RoboSpinUser> _usersCollection;
    
    public RoboSpinUsersService(IOptions<RoboSpinDatabaseSettings> settings)
    {
        MongoClient client = new MongoClient(settings.Value.ConnectionString);
        IMongoDatabase? database = client.GetDatabase(settings.Value.DatabaseName);

        _usersCollection = database.GetCollection<RoboSpinUser>(settings.Value.RoboSpinCollection);
    }
    
    public async Task<List<RoboSpinUser>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<RoboSpinUser?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(RoboSpinUser newRoboSpinUser) =>
        await _usersCollection.InsertOneAsync(newRoboSpinUser);

    public async Task UpdateAsync(string? id, RoboSpinUser updatedRoboSpinUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedRoboSpinUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

}