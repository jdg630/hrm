using Stablemaster.Sim;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var dataDir = Path.Combine(root, "data");
var outputDir = Path.Combine(root, "race-viewer");
var dbDir = Path.Combine(root, "runtime");
var dbPath = Path.Combine(dbDir, "stablemaster.db");

Directory.CreateDirectory(outputDir);
Directory.CreateDirectory(dbDir);

var data = DataLoader.Load(dataDir);
var engine = new SimulationEngine(seed: 42);

RelationalStore.Initialize(dbPath);
RelationalStore.SeedBaseData(dbPath, data, Path.Combine(dataDir, "racecourses.json"));

Console.WriteLine("Stablemaster Season Prototype");
Console.WriteLine("============================");

for (var week = 1; week <= 4; week++)
{
    var plans = engine.BuildWeeklyPlans(data.Horses);
    var updates = engine.AdvanceWeek(data, plans);
    Console.WriteLine($"Week {week}: {updates.Count} horse updates generated.");
    foreach (var update in updates.Take(2))
    {
        Console.WriteLine($"  - {update.HorseName}: {update.Summary}");
    }
}

var playerStable = data.Stables.First();
var entrants = data.Horses.Where(h => h.StableId == playerStable.Id).Take(6).ToList();
var race = data.Races.OrderBy(r => r.Date).First();

var result = engine.SimulateRace(race, entrants, data.Staff.Where(s => s.StableId == playerStable.Id));
SimulationEngine.WriteRaceResult(result, Path.Combine(outputDir, "race_result.json"));
RelationalStore.PersistRace(dbPath, race, playerStable.Id, result.Results);

if (result.Results.Count > 0)
{
    var winner = result.Results[0];
    playerStable.CashBalance += race.Purse * 0.5;
    Console.WriteLine($"Race simulated: {race.Name}");
    Console.WriteLine($"Winner: {winner.HorseName} (score {winner.Score:F2})");
    Console.WriteLine($"Updated stable cash: ${playerStable.CashBalance:N0}");
}
else
{
    Console.WriteLine($"Race simulated: {race.Name}, but no eligible runners (injury/inactive). ");
}

Console.WriteLine($"Race result exported to race-viewer/race_result.json");
Console.WriteLine($"Relational DB updated at runtime/stablemaster.db");
