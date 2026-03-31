using System.Text.Json;

namespace Stablemaster.Sim;

public sealed class SimulationEngine
{
    private readonly Random _rng;

    public SimulationEngine(int? seed = null)
    {
        _rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public List<TrainingPlan> BuildWeeklyPlans(IEnumerable<Horse> horses)
    {
        return horses.Select(h =>
        {
            var focus = h.Fatigue > 55 ? "Recovery" : h.Speed < h.Stamina ? "Speed" : "Stamina";
            var intensity = h.Fatigue > 55 ? 0.35 : 0.55 + _rng.NextDouble() * 0.3;
            return new TrainingPlan { HorseId = h.Id, Focus = focus, Intensity = intensity };
        }).ToList();
    }

    public List<WeeklyUpdate> AdvanceWeek(GameData data, IReadOnlyList<TrainingPlan> plans)
    {
        var trainerBonus = GetStaffBonus(data.Staff, "Trainer");
        var vetBonus = GetStaffBonus(data.Staff, "Vet");
        var updates = new List<WeeklyUpdate>();

        foreach (var horse in data.Horses)
        {
            if (horse.RecoveryWeeksRemaining > 0)
            {
                horse.RecoveryWeeksRemaining -= 1;
                horse.Fatigue = Math.Max(0, horse.Fatigue - 10 - vetBonus * 0.06);
                horse.Fitness = Math.Min(100, horse.Fitness + 3);
                if (horse.RecoveryWeeksRemaining == 0)
                {
                    horse.InjuryStatus = "Healthy";
                }

                horse.OfficialRating = Math.Max(45, horse.OfficialRating - 1);

                updates.Add(new WeeklyUpdate
                {
                    HorseId = horse.Id,
                    HorseName = horse.Name,
                    Summary = $"Recovering ({horse.RecoveryWeeksRemaining}w left); fatigue now {horse.Fatigue:F1}."
                });
                continue;
            }

            var plan = plans.FirstOrDefault(p => p.HorseId == horse.Id)
                       ?? new TrainingPlan { HorseId = horse.Id, Focus = "Recovery", Intensity = 0.35 };
            var trainingGain = plan.Intensity * (3.5 + trainerBonus * 0.05);
            var fatigueGain = plan.Intensity * (6.5 - horse.Stamina * 0.03);

            if (plan.Focus == "Recovery")
            {
                horse.Fatigue = Math.Max(0, horse.Fatigue - 7 - vetBonus * 0.06);
                horse.Fitness = Math.Min(100, horse.Fitness + 2 + trainerBonus * 0.03);
            }
            else
            {
                horse.Fitness = Math.Clamp(horse.Fitness + trainingGain - horse.Fatigue * 0.08, 0, 100);
                horse.Fatigue = Math.Clamp(horse.Fatigue + fatigueGain, 0, 100);
            }

            horse.CurrentForm = Math.Clamp(horse.CurrentForm + (horse.Fitness - horse.Fatigue) * 0.04 + (_rng.NextDouble() - 0.5) * 2.5, 0, 100);
            horse.Morale = Math.Clamp(horse.Morale + (horse.CurrentForm - 50) * 0.02, 20, 100);

            // Dynamic stat and official rating movement (can increase or decrease each week)
            var statTrend = (horse.CurrentForm - 50) / 100.0 + (_rng.NextDouble() - 0.5) * 0.2 - horse.Fatigue / 250.0;
            horse.Speed = (int)Math.Clamp(horse.Speed + Math.Round(statTrend), 40, 99);
            horse.Stamina = (int)Math.Clamp(horse.Stamina + Math.Round(statTrend * 0.8), 40, 99);
            horse.Acceleration = (int)Math.Clamp(horse.Acceleration + Math.Round(statTrend * 0.9), 40, 99);
            horse.Consistency = (int)Math.Clamp(horse.Consistency + Math.Round(statTrend * 0.7), 40, 99);

            var ratingDelta = (int)Math.Round((horse.CurrentForm - 50) / 25.0 + (horse.Fitness - horse.Fatigue) / 60.0);
            horse.OfficialRating = (int)Math.Clamp(horse.OfficialRating + ratingDelta, 45, 130);

            var injuryRisk = Math.Clamp((horse.Fatigue - 45) / 80.0 + (plan.Intensity - 0.5) * 0.4 - vetBonus * 0.002, 0.01, 0.35);
            if (_rng.NextDouble() < injuryRisk)
            {
                horse.InjuryStatus = "Minor Injury";
                horse.RecoveryWeeksRemaining = 1 + _rng.Next(0, 3);
                horse.Morale = Math.Max(25, horse.Morale - 8);
                updates.Add(new WeeklyUpdate
                {
                    HorseId = horse.Id,
                    HorseName = horse.Name,
                    Summary = $"Injured ({horse.InjuryStatus}) - out for {horse.RecoveryWeeksRemaining} weeks."
                });
                continue;
            }

            updates.Add(new WeeklyUpdate
            {
                HorseId = horse.Id,
                HorseName = horse.Name,
                Summary = $"{plan.Focus} @ {plan.Intensity:F2}; OR {horse.OfficialRating}, fit {horse.Fitness:F1}, fatigue {horse.Fatigue:F1}, form {horse.CurrentForm:F1}, SPD {horse.Speed}."
            });
        }

        return updates;
    }

    public RaceResult SimulateRace(Race race, IEnumerable<Horse> entries, IEnumerable<Staff> staff)
    {
        var trainerBonus = GetStaffBonus(staff, "Trainer") * 0.08;
        var jockeyBonus = GetStaffBonus(staff, "Jockey") * 0.09;

        var scored = entries
            .Where(h => h.RecoveryWeeksRemaining == 0)
            .Select(h =>
            {
                var surfaceSuitability = h.SurfacePreference.Equals(race.Surface, StringComparison.OrdinalIgnoreCase) ? 8.0 : -4.0;
                var distancePenalty = Math.Abs(h.DistancePreference - race.Distance) / 200.0;
                var baseScore = h.Speed * 0.32 + h.Stamina * 0.24 + h.Acceleration * 0.16 + h.Consistency * 0.14;
                var conditionScore = h.Fitness * 0.2 - h.Fatigue * 0.18 + h.CurrentForm * 0.2 + h.Morale * 0.08 + h.OfficialRating * 0.12;
                var classPenalty = race.ClassLevel * 1.5;
                var randomness = (_rng.NextDouble() - 0.5) * 14;
                var score = baseScore + conditionScore + surfaceSuitability - distancePenalty - classPenalty + randomness + trainerBonus + jockeyBonus;

                return new
                {
                    Horse = h,
                    Score = score,
                    Explanation = $"base={baseScore:F1}, condition+OR={conditionScore:F1}, staff={trainerBonus + jockeyBonus:F1}, surface={surfaceSuitability:F1}, distancePenalty={distancePenalty:F1}, randomness={randomness:F1}"
                };
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        var results = scored.Select((item, idx) => new RaceResultEntry
        {
            HorseId = item.Horse.Id,
            HorseName = item.Horse.Name,
            Position = idx + 1,
            Score = Math.Round(item.Score, 2),
            Explanation = item.Explanation
        }).ToList();

        return new RaceResult
        {
            RaceId = race.Id,
            RaceName = race.Name,
            Date = race.Date,
            Results = results,
            TrackLength = race.Distance
        };
    }

    private static int GetStaffBonus(IEnumerable<Staff> staff, string role)
        => staff.Where(s => string.Equals(s.Role, role, StringComparison.OrdinalIgnoreCase)).Select(s => s.Skill).DefaultIfEmpty(50).Max();

    public static void WriteRaceResult(RaceResult result, string outputFile)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(outputFile, JsonSerializer.Serialize(result, options));
    }
}
