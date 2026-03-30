using System.Text.Json;

namespace Stablemaster.Sim;

public static class DataLoader
{
    public static GameData Load(string dataDir)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var horsesPath = Path.Combine(dataDir, "horses.json");
        var staffPath = Path.Combine(dataDir, "staff.json");
        var stablesPath = Path.Combine(dataDir, "stables.json");
        var racesPath = Path.Combine(dataDir, "races.json");

        EnsureExists(horsesPath);
        EnsureExists(staffPath);
        EnsureExists(stablesPath);
        EnsureExists(racesPath);

        var horses = JsonSerializer.Deserialize<List<Horse>>(File.ReadAllText(horsesPath), options) ?? [];
        var staff = JsonSerializer.Deserialize<List<Staff>>(File.ReadAllText(staffPath), options) ?? [];
        var stables = JsonSerializer.Deserialize<List<Stable>>(File.ReadAllText(stablesPath), options) ?? [];
        var races = JsonSerializer.Deserialize<List<Race>>(File.ReadAllText(racesPath), options) ?? [];

        return new GameData
        {
            Horses = horses,
            Staff = staff,
            Stables = stables,
            Races = races
        };
    }

    private static void EnsureExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Required data file is missing: {path}");
        }
    }
}
