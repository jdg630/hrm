#!/usr/bin/env python3
"""Turf Master - lightweight CLI management sim prototype.

Implements a playable subset of the Gold Master specification:
- SQLite persistence
- Manual Next Day processing
- Horse aging + sex transitions on Jan 1
- Fitness/condition recovery
- Simple race scheduling + declarations + race execution
- Basic handicapping updates
"""

from __future__ import annotations

import argparse
import random
import sqlite3
from dataclasses import dataclass
from datetime import date, datetime, timedelta
from pathlib import Path
from typing import Iterable

DB_PATH = Path("turf_master.db")
START_DATE = date(2025, 1, 1)


@dataclass
class Horse:
    id: int
    name: str
    birth_year: int
    sex: str
    quirk: int
    stride_length: int
    balance: int
    stamina: int
    speed: int
    condition: float
    fitness: float
    rating: int
    wins: int
    runs: int


def connect(db_path: Path) -> sqlite3.Connection:
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    return conn


def init_db(conn: sqlite3.Connection) -> None:
    conn.executescript(
        """
        CREATE TABLE IF NOT EXISTS meta (
            key TEXT PRIMARY KEY,
            value TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS horses (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT UNIQUE NOT NULL,
            birth_year INTEGER NOT NULL,
            sex TEXT NOT NULL,
            gelded INTEGER NOT NULL DEFAULT 0,
            quirk INTEGER NOT NULL,
            stride_length INTEGER NOT NULL,
            balance INTEGER NOT NULL,
            stamina INTEGER NOT NULL,
            speed INTEGER NOT NULL,
            condition REAL NOT NULL DEFAULT 100.0,
            fitness REAL NOT NULL DEFAULT 65.0,
            fragility INTEGER NOT NULL DEFAULT 50,
            surface_pref TEXT NOT NULL DEFAULT 'Turf',
            going_pref TEXT NOT NULL DEFAULT 'Good',
            rating INTEGER NOT NULL DEFAULT 50,
            wins INTEGER NOT NULL DEFAULT 0,
            runs INTEGER NOT NULL DEFAULT 0,
            retired INTEGER NOT NULL DEFAULT 0
        );

        CREATE TABLE IF NOT EXISTS races (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            race_date TEXT NOT NULL,
            name TEXT NOT NULL,
            distance_furlongs INTEGER NOT NULL,
            surface TEXT NOT NULL,
            going TEXT NOT NULL,
            race_class TEXT NOT NULL DEFAULT 'Open',
            age_restriction TEXT NOT NULL DEFAULT '3+',
            sex_restriction TEXT NOT NULL DEFAULT 'Any',
            declared INTEGER NOT NULL DEFAULT 0,
            completed INTEGER NOT NULL DEFAULT 0,
            winner_horse_id INTEGER,
            FOREIGN KEY (winner_horse_id) REFERENCES horses(id)
        );

        CREATE TABLE IF NOT EXISTS entries (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            race_id INTEGER NOT NULL,
            horse_id INTEGER NOT NULL,
            draw INTEGER,
            final_weight_lbs INTEGER,
            declared_date TEXT,
            result_pos INTEGER,
            speed_figure INTEGER,
            non_runner INTEGER NOT NULL DEFAULT 0,
            UNIQUE(race_id, horse_id),
            FOREIGN KEY (race_id) REFERENCES races(id) ON DELETE CASCADE,
            FOREIGN KEY (horse_id) REFERENCES horses(id)
        );

        CREATE TABLE IF NOT EXISTS ledger (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            event_date TEXT NOT NULL,
            category TEXT NOT NULL,
            amount REAL NOT NULL,
            note TEXT
        );
        """
    )

    if get_meta(conn, "current_date") is None:
        set_meta(conn, "current_date", START_DATE.isoformat())
    conn.commit()


def get_meta(conn: sqlite3.Connection, key: str) -> str | None:
    row = conn.execute("SELECT value FROM meta WHERE key = ?", (key,)).fetchone()
    return None if row is None else row["value"]


def set_meta(conn: sqlite3.Connection, key: str, value: str) -> None:
    conn.execute(
        """
        INSERT INTO meta(key, value) VALUES(?, ?)
        ON CONFLICT(key) DO UPDATE SET value = excluded.value
        """,
        (key, value),
    )


def current_date(conn: sqlite3.Connection) -> date:
    value = get_meta(conn, "current_date")
    if value is None:
        return START_DATE
    return datetime.strptime(value, "%Y-%m-%d").date()


def horse_age(on_date: date, birth_year: int) -> int:
    return on_date.year - birth_year


def normalized_sex(sex: str, age: int, gelded: int) -> str:
    if gelded:
        return "g"
    if sex.lower() in {"colt", "stallion", "horse", "h", "c"}:
        return "c" if age <= 4 else "h"
    if sex.lower() in {"filly", "mare", "f", "m"}:
        return "f" if age <= 4 else "m"
    return sex


def add_horse(conn: sqlite3.Connection, name: str, birth_year: int, sex: str) -> None:
    rng = random.Random(f"{name}-{birth_year}-{sex}")
    quirk = rng.randint(1, 20)
    stride = rng.randint(45, 95)
    balance = rng.randint(45, 95)
    stamina = rng.randint(45, 95)
    speed = rng.randint(45, 95)

    conn.execute(
        """
        INSERT INTO horses(
            name, birth_year, sex, quirk, stride_length, balance, stamina, speed
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """,
        (name, birth_year, sex, quirk, stride, balance, stamina, speed),
    )
    conn.commit()


def all_horses(conn: sqlite3.Connection) -> Iterable[Horse]:
    rows = conn.execute(
        """
        SELECT id, name, birth_year, sex, quirk, stride_length, balance, stamina, speed,
               condition, fitness, rating, wins, runs
        FROM horses
        WHERE retired = 0
        ORDER BY rating DESC, name ASC
        """
    ).fetchall()
    for r in rows:
        yield Horse(**dict(r))


def schedule_race(
    conn: sqlite3.Connection,
    name: str,
    race_date: date,
    distance_furlongs: int,
    surface: str,
    going: str,
    race_class: str,
    age_restriction: str,
    sex_restriction: str,
) -> None:
    conn.execute(
        """
        INSERT INTO races(
            race_date, name, distance_furlongs, surface, going, race_class,
            age_restriction, sex_restriction
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """,
        (
            race_date.isoformat(),
            name,
            distance_furlongs,
            surface,
            going,
            race_class,
            age_restriction,
            sex_restriction,
        ),
    )
    conn.commit()


def eligible(horse: sqlite3.Row, race: sqlite3.Row, today: date) -> bool:
    age = horse_age(today, horse["birth_year"])
    sex = normalized_sex(horse["sex"], age, horse["gelded"])

    if race["race_class"] == "Classic" and sex == "g":
        return False

    sr = race["sex_restriction"].lower()
    if sr == "fillies" and sex not in {"f", "m"}:
        return False
    if sr == "colts" and sex not in {"c", "h", "g"}:
        return False

    ar = race["age_restriction"]
    if ar.endswith("+"):
        min_age = int(ar[:-1])
        if age < min_age:
            return False
    elif "-" in ar:
        lo, hi = ar.split("-", maxsplit=1)
        if not (int(lo) <= age <= int(hi)):
            return False
    else:
        if age != int(ar):
            return False

    if horse["condition"] < 55:
        return False

    return True


def declare_races(conn: sqlite3.Connection, today: date) -> None:
    # 48h declaration window
    target_date = today + timedelta(days=2)
    races = conn.execute(
        "SELECT * FROM races WHERE race_date = ? AND declared = 0",
        (target_date.isoformat(),),
    ).fetchall()

    horses = conn.execute("SELECT * FROM horses WHERE retired = 0").fetchall()

    for race in races:
        candidates = [h for h in horses if eligible(h, race, today)]
        # higher-rated horses more likely to enter
        scored = sorted(candidates, key=lambda x: (x["rating"], x["fitness"]), reverse=True)
        field = scored[: min(12, len(scored))]

        random.shuffle(field)
        for i, h in enumerate(field, start=1):
            base_weight = 126
            sex_allowance = 3 if normalized_sex(h["sex"], horse_age(today, h["birth_year"]), h["gelded"]) in {"f", "m"} else 0
            final_weight = base_weight - sex_allowance
            conn.execute(
                """
                INSERT OR IGNORE INTO entries(race_id, horse_id, draw, final_weight_lbs, declared_date)
                VALUES (?, ?, ?, ?, ?)
                """,
                (race["id"], h["id"], i, final_weight, today.isoformat()),
            )

        conn.execute("UPDATE races SET declared = 1 WHERE id = ?", (race["id"],))


def race_score(h: sqlite3.Row, race: sqlite3.Row) -> float:
    # blend speed/stamina + preferences + condition + quirk randomness
    score = 0.0
    score += h["speed"] * 0.45
    score += h["stamina"] * 0.25
    score += h["stride_length"] * 0.15
    score += h["balance"] * 0.1
    score += h["fitness"] * 0.15
    score += h["condition"] * 0.15

    if h["surface_pref"].lower() == race["surface"].lower():
        score += 6
    if h["going_pref"].lower() == race["going"].lower():
        score += 6

    quirk_noise = random.uniform(-h["quirk"], h["quirk"]) * 0.8
    race_noise = random.uniform(-8, 8)
    score += quirk_noise + race_noise

    # distance fit: closer to middle stamina profile better
    stamina_center = max(5, min(16, int(h["stamina"] / 8)))
    score -= abs(stamina_center - race["distance_furlongs"]) * 1.8

    return score


def handicap_delta(pos: int, field_size: int) -> int:
    if pos == 1:
        return random.randint(4, 10)
    if pos <= max(2, field_size // 3):
        return random.randint(1, 2)
    return -random.randint(1, 3)


def run_races(conn: sqlite3.Connection, today: date) -> list[str]:
    races = conn.execute(
        "SELECT * FROM races WHERE race_date = ? AND completed = 0 AND declared = 1",
        (today.isoformat(),),
    ).fetchall()
    recaps: list[str] = []

    for race in races:
        entries = conn.execute(
            """
            SELECT e.id AS entry_id, e.horse_id, h.*
            FROM entries e
            JOIN horses h ON h.id = e.horse_id
            WHERE e.race_id = ? AND e.non_runner = 0
            """,
            (race["id"],),
        ).fetchall()

        if len(entries) < 2:
            conn.execute("UPDATE races SET completed = 1 WHERE id = ?", (race["id"],))
            recaps.append(f"{race['name']}: void (insufficient runners)")
            continue

        ranked = sorted(entries, key=lambda h: race_score(h, race), reverse=True)

        for pos, h in enumerate(ranked, start=1):
            sf = max(20, min(120, int(70 + race_score(h, race) / 2)))
            conn.execute(
                "UPDATE entries SET result_pos = ?, speed_figure = ? WHERE id = ?",
                (pos, sf, h["entry_id"]),
            )

            delta = handicap_delta(pos, len(ranked))
            conn.execute(
                """
                UPDATE horses
                SET rating = MAX(1, rating + ?),
                    runs = runs + 1,
                    wins = wins + CASE WHEN ? = 1 THEN 1 ELSE 0 END,
                    condition = MAX(35, condition - ?),
                    fitness = MIN(100, fitness + 2)
                WHERE id = ?
                """,
                (delta, pos, random.randint(8, 16), h["horse_id"]),
            )

        winner = ranked[0]
        conn.execute(
            "UPDATE races SET completed = 1, winner_horse_id = ? WHERE id = ?",
            (winner["horse_id"], race["id"]),
        )
        conn.execute(
            "INSERT INTO ledger(event_date, category, amount, note) VALUES (?, 'Prize', ?, ?)",
            (today.isoformat(), 10000.0, f"Win: {race['name']}"),
        )
        recaps.append(f"{race['name']}: won by {winner['name']}")

    return recaps


def apply_new_year_aging(conn: sqlite3.Connection, today: date) -> None:
    if today.month != 1 or today.day != 1:
        return

    horses = conn.execute("SELECT id, birth_year, sex, gelded FROM horses WHERE retired = 0").fetchall()
    for h in horses:
        age = horse_age(today, h["birth_year"])
        new_sex = normalized_sex(h["sex"], age, h["gelded"])
        conn.execute("UPDATE horses SET sex = ? WHERE id = ?", (new_sex, h["id"]))


def recover_horses(conn: sqlite3.Connection) -> None:
    conn.execute(
        """
        UPDATE horses
        SET condition = MIN(100, condition + (4 + (100 - quirk) * 0.03)),
            fitness = MAX(55, fitness - 0.2)
        WHERE retired = 0
        """
    )


def next_day(conn: sqlite3.Connection) -> list[str]:
    today = current_date(conn)
    recaps: list[str] = []

    apply_new_year_aging(conn, today)
    recover_horses(conn)
    declare_races(conn, today)
    recaps.extend(run_races(conn, today))

    tomorrow = today + timedelta(days=1)
    set_meta(conn, "current_date", tomorrow.isoformat())
    conn.commit()

    if not recaps:
        recaps.append("No races today.")
    return recaps


def seed_if_empty(conn: sqlite3.Connection) -> None:
    count = conn.execute("SELECT COUNT(*) AS n FROM horses").fetchone()["n"]
    if count > 0:
        return

    starters = [
        ("Emerald Comet", 2022, "c"),
        ("Velvet Line", 2022, "f"),
        ("Iron Regent", 2021, "h"),
        ("Harbor Light", 2023, "c"),
        ("Winter Bell", 2021, "m"),
        ("Dirt Anthem", 2022, "g"),
    ]
    for name, by, sex in starters:
        add_horse(conn, name, by, sex)

    today = current_date(conn)
    schedule_race(conn, "Newmarket Spring Trial", today + timedelta(days=2), 8, "Turf", "Good", "Open", "3+", "Any")
    schedule_race(conn, "Epsom Classic Trial", today + timedelta(days=5), 12, "Turf", "Soft", "Classic", "3", "Any")
    conn.commit()


def print_horses(conn: sqlite3.Connection) -> None:
    today = current_date(conn)
    print(f"Stable roster on {today.isoformat()}:")
    print("ID  Name               Age Sex OR  Cond Fit  W-R")
    print("--  -----------------  --- --- --- ---- ---- ----")
    for h in all_horses(conn):
        age = horse_age(today, h.birth_year)
        print(
            f"{h.id:2}  {h.name[:17]:17}  {age:>3} {h.sex:>3} {h.rating:>3}"
            f" {h.condition:>4.1f} {h.fitness:>4.1f} {h.wins:>2}-{h.runs:<2}"
        )


def print_races(conn: sqlite3.Connection) -> None:
    rows = conn.execute(
        """
        SELECT r.id, r.race_date, r.name, r.distance_furlongs, r.surface, r.going,
               r.declared, r.completed, COUNT(e.id) AS entries
        FROM races r
        LEFT JOIN entries e ON e.race_id = r.id
        GROUP BY r.id
        ORDER BY r.race_date, r.id
        """
    ).fetchall()
    print("ID  Date        Race                         Dist Surface Going Decl Done Field")
    for r in rows:
        print(
            f"{r['id']:2}  {r['race_date']}  {r['name'][:28]:28} {r['distance_furlongs']:>4}f"
            f" {r['surface'][:6]:6} {r['going'][:6]:6}"
            f" {r['declared']:>4} {r['completed']:>4} {r['entries']:>5}"
        )


def main() -> None:
    parser = argparse.ArgumentParser(description="Turf Master CLI prototype")
    parser.add_argument("--db", default=str(DB_PATH), help="Path to SQLite DB")

    sub = parser.add_subparsers(dest="cmd", required=True)

    sub.add_parser("init", help="Initialize game database")
    sub.add_parser("seed", help="Seed starter horses and races if empty")
    sub.add_parser("horses", help="List horses")
    sub.add_parser("races", help="List races")
    sub.add_parser("next-day", help="Process one day")

    add_h = sub.add_parser("add-horse", help="Add a horse")
    add_h.add_argument("name")
    add_h.add_argument("birth_year", type=int)
    add_h.add_argument("sex", choices=["c", "h", "f", "m", "g"])

    add_r = sub.add_parser("add-race", help="Schedule race")
    add_r.add_argument("date", help="YYYY-MM-DD")
    add_r.add_argument("name")
    add_r.add_argument("distance_furlongs", type=int)
    add_r.add_argument("surface", choices=["Turf", "Dirt", "All-Weather"])
    add_r.add_argument("going")
    add_r.add_argument("--class", dest="race_class", default="Open")
    add_r.add_argument("--age", dest="age", default="3+")
    add_r.add_argument("--sex", dest="sex", default="Any")

    args = parser.parse_args()

    conn = connect(Path(args.db))
    init_db(conn)

    if args.cmd == "init":
        print(f"Initialized DB at {args.db}; current date = {current_date(conn)}")
    elif args.cmd == "seed":
        seed_if_empty(conn)
        print("Seed data created (or already present).")
    elif args.cmd == "horses":
        print_horses(conn)
    elif args.cmd == "races":
        print_races(conn)
    elif args.cmd == "next-day":
        today = current_date(conn)
        recaps = next_day(conn)
        print(f"Processed day: {today.isoformat()}")
        for recap in recaps:
            print(f"- {recap}")
        print(f"New date: {current_date(conn).isoformat()}")
    elif args.cmd == "add-horse":
        add_horse(conn, args.name, args.birth_year, args.sex)
        print(f"Added horse: {args.name}")
    elif args.cmd == "add-race":
        race_date = datetime.strptime(args.date, "%Y-%m-%d").date()
        schedule_race(
            conn,
            args.name,
            race_date,
            args.distance_furlongs,
            args.surface,
            args.going,
            args.race_class,
            args.age,
            args.sex,
        )
        print(f"Scheduled race '{args.name}' on {race_date}")


if __name__ == "__main__":
    main()
