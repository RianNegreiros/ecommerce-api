using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DatabaseInitializer
{
    public static async Task Initialize(WebApplication app)
    {
      await DB.InitAsync("SearchDb", MongoClientSettings
        .FromConnectionString(app.Configuration.GetConnectionString("MongoDb")));

      await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

      var count = await DB.CountAsync<Item>();

      if (count == 0)
      {
        Console.WriteLine("Seeding database...");
        var itemData = await File.ReadAllTextAsync("Data/auctions.json");

        var options = new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true,
        };

        var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

        if (items != null)
        {
          await DB.SaveAsync(items);
        }
      }
    }
}