PRAGMA foreign_keys = ON;

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
);
