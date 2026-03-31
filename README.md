# Stablemaster Prototype

This repository now contains a first playable simulation slice based on `VISION.md`, including a browser-playable HTML management prototype:

- **C# simulation core** (`src/Stablemaster.Sim`)
- **JSON data definitions** (`data/*.json`, including staff and racecourse master data)
- **Relational persistence** (`runtime/stablemaster.db`, SQLite)
- **Three.js race viewer** (`race-viewer/index.html` + `viewer.js`)



## Browser game (HTML + modern UI + Three.js)

Serve the repository and open `/race-viewer/` to play the web prototype:

- management dashboard with tabs (Roster / Insights / Log)
- one-click weekly simulation updates plus multi-week advance
- race simulation + animated Three.js race viewer with playback speed controls
- browser save state persistence via localStorage

## Simulation aspects currently modeled

- Weekly training-plan generation per horse (`Speed`, `Stamina`, or `Recovery` focus).
- Fitness / fatigue / form / morale progression each week.
- Official Rating (OR) and core horse stats (speed/stamina/acceleration/consistency) now move up or down based on weekly form/training/fatigue trends.
- Injury rolls and multi-week recovery handling.
- Staff influence (trainer, jockey, vet) on training outcomes and race performance.
- Race explanations including base ability, condition, staff effect, suitability, and randomness.

## Real-world 2025/2026 calendar data

- `data/races.json` now contains an expanded race list (171 races) spanning the Road to the Kentucky Derby plus additional major UK/Ireland and international races, including an expanded UK/Ireland jumps program plus group/graded/listed race coverage plus premier handicaps for broader gameplay coverage.
- `data/race_calendar_sources.json` records the official source and normalization notes.
- `data/racecourses.json` contains expanded major UK/Ireland plus international racecourse metadata (US/UAE/France/Australia/Japan/Hong Kong) used as a relational lookup by race rows (`racecourseId`).

## Relational data model

The simulation now links game data with foreign-key relationships in SQLite:

- `stables`
- `staff` (`stable_id -> stables.id`)
- `horses` (`stable_id -> stables.id`)
- `racecourses`
- `races` (`racecourse_id -> racecourses.id` when present)
- `race_entries` (`race_id -> races.id`, `horse_id -> horses.id`, `stable_id -> stables.id`)
- `race_results` (`race_id -> races.id`, `horse_id -> horses.id`)

## Run simulation

```bash
dotnet run --project src/Stablemaster.Sim
```

This advances four weekly ticks, simulates one race, and writes:

- `race-viewer/race_result.json`
- `runtime/stablemaster.db`

## View race replay

From repository root, serve files with a local static server and open `/race-viewer/`.
For example:

```bash
python3 -m http.server 8080
```

Then open:

- <http://localhost:8080/race-viewer/>
