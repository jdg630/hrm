# Stablemaster — What Still Needs Work

This document captures the highest-priority gaps from the current prototype and what to build next.

## 1) Core Simulation Depth (Highest Priority)

## 1.1 Horse lifecycle is still too shallow
- Add aging curves by age band (2yo/3yo/older peak/decline).
- Split injuries into severity tiers (minor/moderate/major) with probabilistic rehab outcomes.
- Add persistent trait system (temperament, gate behavior, closing kick, wet-track affinity).
- Add hidden potential progression and visible scouting uncertainty.

## 1.2 Training system needs player agency
- Replace auto-plan-only flow with player-configurable weekly schedules.
- Add explicit training modules (speed work, stamina work, gate drills, easy canter).
- Add peaking windows and race-specific prep plans.
- Add overtraining and burnout behavior that persists over multiple weeks.

## 1.3 Race simulation should model pace dynamics
- Add pace map (leader/stalker/closer roles) and sectionals.
- Include draw/bias, going/track condition, and weather effects.
- Add tactical decisions (front-run, hold-up, mid-pack) and tactical execution variance.
- Generate richer explanation output (sectional breakdown + decisive factors).

## 2) Management Gameplay Loop

## 2.1 Staffing is under-utilized
- Add contract length, role-specific responsibilities, and poaching.
- Add staff development and trait system.
- Add morale drivers (results, pay, workload, facilities).

## 2.2 Finance model is minimal
- Add monthly burn and recurring operational expenses.
- Add sponsorship/owner injections, transfer/claiming market, and horse sale valuation model.
- Add prize-money splits by placing and race class.

## 2.3 Missing market/scouting gameplay
- Implement auction and private purchase flow.
- Build scouting reports with confidence/uncertainty and regional specialization.
- Add AI stable bidding behavior.

## 3) Race Calendar & Content

## 3.1 Calendar is static data only
- Validate and refresh race dates per season at data-update checkpoints.
- Add eligibility rules per race (age, sex, class rating, region).
- Add conflict handling (simultaneous races requiring roster split decisions).

## 3.2 Need data governance
- Add versioned data pipeline for race/racecourse JSON updates.
- Add JSON schema validation + CI checks for data integrity.
- Add source attribution metadata per imported race block.

## 4) Persistence, Architecture, and Tooling

## 4.1 Save/load and career continuity
- Add explicit save-slot system and migration strategy for schema updates.
- Persist weekly inbox/events and historical records.
- Add deterministic replay seed capture for race debugging.

## 4.2 Simulation/service boundaries
- Split domain/sim/application layers into clearer service boundaries.
- Add repository abstraction instead of direct SQL in orchestration paths.
- Add import/export command layer for headless balancing runs.

## 4.3 Test coverage (critical gap)
- Unit tests for simulation formulas and injury/training edge cases.
- Integration tests for DB seeding + race persistence.
- Snapshot tests for race explanation output and ranking determinism with seed.

## 5) Web UI / UX Quality

## 5.1 Current UI is prototype-only
- Introduce route-based app structure (Dashboard, Horses, Calendar, Staff, Finance, Race Day).
- Add reusable design system tokens/components.
- Add mobile responsiveness constraints and accessibility checks.

## 5.2 Race viewer fidelity
- Replace box proxies with horse placeholders + lane offsets and camera rails.
- Add replay controls (pause, speed x1/x2/x4, follow horse, rewind).
- Add timeline panel with sectional markers.

## 6) Productization

## 6.1 Packaging and distribution
- Define clear split between desktop sim runtime and web viewer runtime.
- Add build scripts for reproducible local/dev builds.
- Add telemetry hooks for balancing (opt-in for development builds).

## 6.2 Content and onboarding
- Add tutorialized first 4 weeks with advisor prompts.
- Add difficulty presets and scenario starts (small budget / big stable / youth academy).
- Add glossary and contextual help for horse-racing-specific terms.

---

## Recommended next milestone (4–6 weeks)
1. Player-driven training planner + improved injury model.
2. Race eligibility + pace-model v2 with explanation panel.
3. Finance v1 (monthly burn + purse splits + staff wages).
4. Unit/integration test harness + JSON schema validation.
5. UI navigation split into dedicated management screens.
