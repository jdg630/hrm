using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Stablemaster.Sim;

public sealed class Racecourse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string Region { get; init; }
    public List<string> Disciplines { get; init; } = [];
    public List<string> Surfaces { get; init; } = [];
    public List<string> NotableRaces { get; init; } = [];
}

public static class RelationalStore
{
    public static void Initialize(string dbPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        using var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();

        Execute(conn, "PRAGMA foreign_keys = ON;");

        var schema = conn.CreateCommand();
        schema.CommandText = @"
            CREATE TABLE IF NOT EXISTS stables (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                cash_balance REAL NOT NULL
            );

            CREATE TABLE IF NOT EXISTS racecourses (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                country TEXT NOT NULL,
                region TEXT NOT NULL,
                disciplines_json TEXT NOT NULL,
                surfaces_json TEXT NOT NULL,
                notable_races_json TEXT NOT NULL
            );


            CREATE TABLE IF NOT EXISTS staff (
                id TEXT PRIMARY KEY,
                stable_id TEXT NOT NULL,
                name TEXT NOT NULL,
                role TEXT NOT NULL,
                skill INTEGER NOT NULL,
                morale INTEGER NOT NULL,
                salary REAL NOT NULL,
                FOREIGN KEY(stable_id) REFERENCES stables(id)
            );

            CREATE TABLE IF NOT EXISTS horses (
                id TEXT PRIMARY KEY,
                stable_id TEXT NOT NULL,
                name TEXT NOT NULL,
                age INTEGER NOT NULL,
                speed INTEGER NOT NULL,
                stamina INTEGER NOT NULL,
                acceleration INTEGER NOT NULL,
                consistency INTEGER NOT NULL,
                fitness REAL NOT NULL,
                fatigue REAL NOT NULL,
                current_form REAL NOT NULL,
                surface_preference TEXT NOT NULL,
                distance_preference INTEGER NOT NULL,
                FOREIGN KEY(stable_id) REFERENCES stables(id)
            );

            CREATE TABLE IF NOT EXISTS races (
                id TEXT PRIMARY KEY,
                racecourse_id TEXT,
                name TEXT NOT NULL,
                race_date TEXT NOT NULL,
                surface TEXT NOT NULL,
                distance INTEGER NOT NULL,
                class_level INTEGER NOT NULL,
                purse REAL NOT NULL,
                field_size INTEGER NOT NULL,
                FOREIGN KEY(racecourse_id) REFERENCES racecourses(id)
            );

            CREATE TABLE IF NOT EXISTS race_entries (
                race_id TEXT NOT NULL,
                horse_id TEXT NOT NULL,
                stable_id TEXT NOT NULL,
                PRIMARY KEY (race_id, horse_id),
                FOREIGN KEY(race_id) REFERENCES races(id),
                FOREIGN KEY(horse_id) REFERENCES horses(id),
                FOREIGN KEY(stable_id) REFERENCES stables(id)
            );

            CREATE TABLE IF NOT EXISTS race_results (
                race_id TEXT NOT NULL,
                horse_id TEXT NOT NULL,
                finish_position INTEGER NOT NULL,
                performance_score REAL NOT NULL,
                explanation TEXT NOT NULL,
                PRIMARY KEY (race_id, horse_id),
                FOREIGN KEY(race_id) REFERENCES races(id),
                FOREIGN KEY(horse_id) REFERENCES horses(id)
            );";
        schema.ExecuteNonQuery();
    }

    public static void SeedBaseData(string dbPath, GameData data, string racecoursesPath)
    {
        var racecourses = JsonSerializer.Deserialize<List<Racecourse>>(File.ReadAllText(racecoursesPath)) ?? [];

        using var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();
        using var tx = conn.BeginTransaction();

        Execute(conn, "DELETE FROM race_results;");
        Execute(conn, "DELETE FROM race_entries;");
        Execute(conn, "DELETE FROM races;");
        Execute(conn, "DELETE FROM horses;");
        Execute(conn, "DELETE FROM staff;");
        Execute(conn, "DELETE FROM racecourses;");
        Execute(conn, "DELETE FROM stables;");

        foreach (var stable in data.Stables)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO stables (id, name, cash_balance) VALUES ($id,$name,$cash);";
            cmd.Parameters.AddWithValue("$id", stable.Id);
            cmd.Parameters.AddWithValue("$name", stable.Name);
            cmd.Parameters.AddWithValue("$cash", stable.CashBalance);
            cmd.ExecuteNonQuery();
        }

        foreach (var racecourse in racecourses)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO racecourses (id,name,country,region,disciplines_json,surfaces_json,notable_races_json)
                                VALUES ($id,$name,$country,$region,$disciplines,$surfaces,$notable);";
            cmd.Parameters.AddWithValue("$id", racecourse.Id);
            cmd.Parameters.AddWithValue("$name", racecourse.Name);
            cmd.Parameters.AddWithValue("$country", racecourse.Country);
            cmd.Parameters.AddWithValue("$region", racecourse.Region);
            cmd.Parameters.AddWithValue("$disciplines", JsonSerializer.Serialize(racecourse.Disciplines));
            cmd.Parameters.AddWithValue("$surfaces", JsonSerializer.Serialize(racecourse.Surfaces));
            cmd.Parameters.AddWithValue("$notable", JsonSerializer.Serialize(racecourse.NotableRaces));
            cmd.ExecuteNonQuery();
        }


        foreach (var member in data.Staff)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO staff (id, stable_id, name, role, skill, morale, salary)
                                VALUES ($id,$stableId,$name,$role,$skill,$morale,$salary);";
            cmd.Parameters.AddWithValue("$id", member.Id);
            cmd.Parameters.AddWithValue("$stableId", member.StableId);
            cmd.Parameters.AddWithValue("$name", member.Name);
            cmd.Parameters.AddWithValue("$role", member.Role);
            cmd.Parameters.AddWithValue("$skill", member.Skill);
            cmd.Parameters.AddWithValue("$morale", member.Morale);
            cmd.Parameters.AddWithValue("$salary", member.Salary);
            cmd.ExecuteNonQuery();
        }

        foreach (var horse in data.Horses)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO horses
                (id, stable_id, name, age, speed, stamina, acceleration, consistency, fitness, fatigue, current_form, surface_preference, distance_preference)
                VALUES ($id,$stableId,$name,$age,$speed,$stamina,$acceleration,$consistency,$fitness,$fatigue,$form,$surface,$distancePref);";
            cmd.Parameters.AddWithValue("$id", horse.Id);
            cmd.Parameters.AddWithValue("$stableId", horse.StableId);
            cmd.Parameters.AddWithValue("$name", horse.Name);
            cmd.Parameters.AddWithValue("$age", horse.Age);
            cmd.Parameters.AddWithValue("$speed", horse.Speed);
            cmd.Parameters.AddWithValue("$stamina", horse.Stamina);
            cmd.Parameters.AddWithValue("$acceleration", horse.Acceleration);
            cmd.Parameters.AddWithValue("$consistency", horse.Consistency);
            cmd.Parameters.AddWithValue("$fitness", horse.Fitness);
            cmd.Parameters.AddWithValue("$fatigue", horse.Fatigue);
            cmd.Parameters.AddWithValue("$form", horse.CurrentForm);
            cmd.Parameters.AddWithValue("$surface", horse.SurfacePreference);
            cmd.Parameters.AddWithValue("$distancePref", horse.DistancePreference);
            cmd.ExecuteNonQuery();
        }

        foreach (var race in data.Races)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO races (id, racecourse_id, name, race_date, surface, distance, class_level, purse, field_size)
                VALUES ($id,$racecourseId,$name,$date,$surface,$distance,$class,$purse,$fieldSize);";
            cmd.Parameters.AddWithValue("$id", race.Id);
            cmd.Parameters.AddWithValue("$racecourseId", (object?)race.RacecourseId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$name", race.Name);
            cmd.Parameters.AddWithValue("$date", race.Date);
            cmd.Parameters.AddWithValue("$surface", race.Surface);
            cmd.Parameters.AddWithValue("$distance", race.Distance);
            cmd.Parameters.AddWithValue("$class", race.ClassLevel);
            cmd.Parameters.AddWithValue("$purse", race.Purse);
            cmd.Parameters.AddWithValue("$fieldSize", race.FieldSize);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    public static void PersistRace(string dbPath, Race race, string stableId, IReadOnlyList<RaceResultEntry> resultEntries)
    {
        using var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();
        using var tx = conn.BeginTransaction();

        foreach (var entry in resultEntries)
        {
            using var insertEntry = conn.CreateCommand();
            insertEntry.CommandText = "INSERT OR IGNORE INTO race_entries (race_id, horse_id, stable_id) VALUES ($raceId,$horseId,$stableId);";
            insertEntry.Parameters.AddWithValue("$raceId", race.Id);
            insertEntry.Parameters.AddWithValue("$horseId", entry.HorseId);
            insertEntry.Parameters.AddWithValue("$stableId", stableId);
            insertEntry.ExecuteNonQuery();

            using var resultCmd = conn.CreateCommand();
            resultCmd.CommandText = @"INSERT OR REPLACE INTO race_results (race_id, horse_id, finish_position, performance_score, explanation)
                                      VALUES ($raceId,$horseId,$position,$score,$explanation);";
            resultCmd.Parameters.AddWithValue("$raceId", race.Id);
            resultCmd.Parameters.AddWithValue("$horseId", entry.HorseId);
            resultCmd.Parameters.AddWithValue("$position", entry.Position);
            resultCmd.Parameters.AddWithValue("$score", entry.Score);
            resultCmd.Parameters.AddWithValue("$explanation", entry.Explanation);
            resultCmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private static void Execute(SqliteConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}
