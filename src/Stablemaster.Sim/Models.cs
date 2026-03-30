using System.Text.Json.Serialization;

namespace Stablemaster.Sim;

public sealed class Horse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public int Age { get; init; }
    public int Speed { get; set; }
    public int Stamina { get; set; }
    public int Acceleration { get; set; }
    public int Consistency { get; set; }
    public double Fitness { get; set; }
    public double Fatigue { get; set; }
    public double CurrentForm { get; set; }
    public double Morale { get; set; } = 60;
    public string InjuryStatus { get; set; } = "Healthy";
    public int RecoveryWeeksRemaining { get; set; }
    public string SurfacePreference { get; init; } = "Turf";
    public int DistancePreference { get; init; } = 1600;
    public string StableId { get; init; } = string.Empty;
}

public sealed class Staff
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Role { get; init; }
    public int Skill { get; init; }
    public int Morale { get; set; }
    public double Salary { get; init; }
    public string StableId { get; init; } = string.Empty;
}

public sealed class Stable
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public double CashBalance { get; set; }
    public List<string> HorseIds { get; init; } = [];
}

public sealed class Race
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string Date { get; init; } = string.Empty;
    public string Surface { get; init; } = "Turf";
    public int Distance { get; init; }
    public int ClassLevel { get; init; }
    public double Purse { get; init; }
    public int FieldSize { get; init; } = 8;
    public string? RacecourseId { get; init; }
}

public sealed class TrainingPlan
{
    public required string HorseId { get; init; }
    public required string Focus { get; init; }
    public double Intensity { get; init; }
}

public sealed class WeeklyUpdate
{
    public required string HorseId { get; init; }
    public required string HorseName { get; init; }
    public required string Summary { get; init; }
}

public sealed class GameData
{
    public List<Horse> Horses { get; init; } = [];
    public List<Staff> Staff { get; init; } = [];
    public List<Stable> Stables { get; init; } = [];
    public List<Race> Races { get; init; } = [];
}

public sealed class RaceResultEntry
{
    public required string HorseId { get; init; }
    public required string HorseName { get; init; }
    public required int Position { get; init; }
    public required double Score { get; init; }
    public required string Explanation { get; init; }
}

public sealed class RaceResult
{
    public required string RaceId { get; init; }
    public required string RaceName { get; init; }
    public required string Date { get; init; }
    public List<RaceResultEntry> Results { get; init; } = [];

    [JsonPropertyName("trackLength")]
    public double TrackLength { get; init; }
}
