# Turf Master (Prototype)

A lightweight command-line horse racing management prototype generated from the Gold Master specification.

## Features implemented

- SQLite3 persistence and auto-commit style day progression.
- Manual **Next Day** processing pipeline.
- Horse roster with core attributes (speed, stamina, stride, balance, quirk, condition, fitness, OR).
- Jan 1 birthday/sex-transition logic and permanent gelding identity.
- Race calendar, 48-hour declarations, draw/weight snapshots.
- Classic restriction example (no geldings in Classic races).
- Race simulation with attribute + preference + quirk effects.
- Post-race basic handicapping and prize ledger entries.

## Quickstart

```bash
python3 turf_master.py init
python3 turf_master.py seed
python3 turf_master.py horses
python3 turf_master.py races
python3 turf_master.py next-day
```

Run `next-day` repeatedly to progress time and execute declarations/races.

## Additional commands

```bash
python3 turf_master.py add-horse "Storm Banner" 2023 c
python3 turf_master.py add-race 2025-01-12 "Winter Trial" 8 Turf Good --class Open --age 3+ --sex Any
```
