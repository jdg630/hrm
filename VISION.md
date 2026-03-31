# Horse Racing Management Sim — Vision Document (Draft)

## Working Title

Stablemaster: Horse Racing Manager

## 1. Vision Statement

Create a standalone PC horse racing management simulation for Steam that delivers the long-term strategic depth, data-driven decision-making, and career immersion of a top-tier management sim. The player builds a racing operation over many seasons through horse acquisition, development, staffing, race planning, finance, and breeding, with the goal of growing from a modest stable into an elite racing dynasty.

## 2. Product Goal

Deliver a commercially viable management sim with strong replayability, a clear player fantasy, and enough simulation depth to attract fans of Football Manager, motorsport manager games, tycoon games, and horse racing enthusiasts.

## 3. Player Fantasy

The player is not a jockey and not primarily a spectator. The player is the strategic force behind a racing stable:

* Spotting talent before rivals do
* Developing horses into winners
* Hiring the right trainer, jockeys, vets, and support staff
* Managing budgets, facilities, and owner expectations
* Entering the right races at the right time
* Building bloodlines and a lasting legacy

## 4. Core Design Pillars

### 4.1 Management Depth

The main interaction is decision-making, planning, evaluation, and optimization.

### 4.2 Living Racing Ecosystem

The world should feel dynamic, with rival stables, seasonal calendars, horse progression, injuries, bloodlines, and shifting reputations.

### 4.3 Long-Term Career Progression

The game must support multi-season play with compounding decisions and persistent consequences.

### 4.4 Data-Rich but Readable UI

Players need reports, form guides, development tracking, staff insights, and race analysis without being overwhelmed.

### 4.5 High Replayability

Different starts, regions, budgets, horse pools, and strategic paths should create varied campaigns.

## 5. Target Audience

### Primary Audience

* Fans of management sims such as Football Manager and Motorsport Manager
* Players who enjoy spreadsheets, optimization, scouting, and long-term progression

### Secondary Audience

* Horse racing fans
* Fans of tycoon and simulation games on Steam
* Players who enjoy sports strategy without requiring twitch gameplay

## 6. Platform and Market Position

### Platform

* PC first
* Standalone application
* Built for Steam release

### Positioning

A deep management-first sports sim in an under-served niche, combining the career obsession of Football Manager with the subject matter of horse racing.

## 7. Core Gameplay Loop

1. Review stable status, staff reports, finances, and horse condition
2. Set training, recovery, and development plans
3. Scout, buy, sell, lease, or breed horses
4. Choose race entries based on form, distance, surface, competition, and risk
5. Watch or simulate race results
6. Analyze outcomes, injuries, finances, and reputation changes
7. Upgrade facilities, adjust strategy, and plan the next phase of the season

## 8. Core Game Systems

### 8.1 Horses

* Attributes, hidden potential, temperament, preferences, form, fatigue, injury state
* Ageing and career arcs
* Surface, distance, and pace suitability
* Development through training and race exposure

### 8.2 Training and Conditioning

* Weekly schedules and intensity controls
* Fitness vs freshness tradeoffs
* Staff-driven recommendations
* Recovery, overtraining, peaking, and form cycles

### 8.3 Race Entry and Calendar Management

* Regional and seasonal race calendars
* Eligibility, class, distance, surface, prestige, and purse money
* Strategic placement and campaign planning

### 8.4 Staff and Stable Operations

* Trainers
* Assistant trainers
* Jockeys
* Vets
* Scouts
* Bloodstock agents
* Stable hands / operational support
* Staff specialization, salaries, morale, and quality

### 8.5 Finance and Facilities

* Cash flow, owner backing, salaries, race winnings, horse sales, breeding income
* Stable capacity and upgrades
* Medical, training, and breeding facility improvements

### 8.6 Breeding and Bloodlines

* Stallions, mares, bloodline traits, inherited tendencies, long-term breeding programs
* Economic and strategic value of offspring

### 8.7 Reputation and Prestige

* Stable reputation
* Staff attraction
* Better race invites and owner opportunities
* Media presence and legacy metrics

### 8.8 World Simulation

* Rival stables
* AI horse development and race placement
* Market movement for staff and horses
* Seasonal progression and historical records

## 9. Race Simulation Philosophy

The race simulation must feel credible, legible, and strategically connected to management decisions.

### Principles

* Outcomes should emerge from preparation, horse suitability, field strength, tactics, randomness, and current condition
* Players must understand why a result happened
* The simulation should generate believable narratives, not pure noise

### Presentation Options

* Fully simulated result screen
* Simplified 2D or 2.5D race visualization
* Full 3D race presentation later if justified by budget and team strength

## 10. UX Principles

* Information hierarchy first
* Low friction between screens
* Fast navigation for long careers
* Tooltips and advisor support for onboarding
* Clear cause-and-effect after every key decision

## 11. Art and Presentation Direction

### Recommended Initial Direction

* Clean professional management UI
* Race-day presentation that is readable and exciting, but cost-controlled
* Horses represented clearly in portraits, cards, and race visualization

### Recommended Scope-Control Choice

Deprioritize photorealistic 3D horse animation in the first commercial version unless the team already has strong animation and racing game expertise.

## 12. Audio Direction

* Calm management ambience in menus
* Elevated race-day music and commentary cues
* Functional but not overproduced first-release audio scope

## 13. Content Strategy

### Launch Scope Candidate

* One main career mode
* One or a small number of regions / circuits
* A strong set of races with enough variety to support long-term play
* Deep systems over broad geography

### Expansion Potential

* More countries and circuits
* Deeper breeding systems
* Syndicates and ownership groups
* Media and narrative events
* Steward decisions, scandals, regulation systems
* More race presentation fidelity

## 14. MVP Definition

A commercially testable MVP should include:

* Playable long-term career mode
* Functional horse progression system
* Training and fatigue management
* Staff hiring and operational management
* Race calendar and entry decisions
* Credible race simulation and result breakdown
* Basic economy and facility upgrades
* Save/load persistence
* Usable management UI with reports and filters

## 15. Non-Goals for Initial Version

* Real-time manual riding gameplay
* Large open-world stable exploration
* Ultra-realistic broadcast-grade 3D racing visuals
* Fully global racing ecosystem at launch
* Excessive narrative scripting that dilutes simulation depth

## 16. Production Risks

* Overbuilding visual race presentation before the management loop is proven
* Attempting full realism in breeding, training, and race rules too early
* Making results feel random instead of earned
* UI complexity overwhelming new players
* Niche theme limiting market reach if onboarding is poor

## 17. Recommended Product Approach (Provisional)

### Best Product Approach

Start with a management-first sim where the primary value is planning, data interpretation, and long-term progression. Race presentation should support clarity and excitement, but not dominate scope.

### Best Scope Approach

Build a vertical slice around one full season loop, then expand depth before breadth. Prove the fun of:

* horse development
* race placement
* race simulation
* financial progression
* stable growth

### Best Commercial Approach

Position the game as the definitive horse racing management sim rather than a broad horse game. Focus marketing on strategy, dynasty-building, statistics, and the satisfaction of turning overlooked horses into champions.

## 18. Recommended Technical Approach (Provisional)

### Core Recommendation

Use a simulation-first architecture with data-driven systems and a UI-centric client. Treat racing as an outcome of interconnected systems, not as a handcrafted animation problem.

### Key Principles

* Data-driven horse, staff, race, and world definitions
* Deterministic or near-deterministic simulation with controlled randomness
* Strong telemetry and balancing tools for tuning
* Tooling for rapid iteration of attributes, formulas, and AI behavior
* UI performance and modifiability prioritized early

### Suggested Development Sequence

1. Economy + horse data model
2. Training and progression systems
3. Race entry logic and AI stable behavior
4. Race simulation engine with explanation layers
5. Management UI and reports
6. Save/load and career continuity
7. Presentation polish

## 19. Success Criteria

* Players understand the value of their decisions
* Careers remain compelling across many seasons
* Winning feels earned through planning, not luck
* The UI supports deep play without excessive friction
* The game can be expanded post-launch without rewriting core systems

## 20. Locked Starting Direction

To begin development without further discovery, the project will proceed with these default decisions:

* Single-player first
* Management-first gameplay
* Fictional racing world inspired by real horse racing structures
* Fast simulation with optional race viewing
* Deep stable management as the core loop
* Breeding included in the architecture but deferred behind the core stable loop
* PC-first development for eventual Steam release

## 21. Selected Development Approach

### Product Direction

Build the first playable version around the fantasy of running a stable over a season. The player’s primary activities are:

* reviewing horse condition and form
* assigning training focus
* selecting race entries
* managing staff and budgets
* responding to results, injuries, and progression

### Scope Direction

The first playable should cover one circuit, one career mode, one stable, and one full season loop. Depth is prioritized over breadth.

### Presentation Direction

Race presentation is a core product feature. The first commercial target should support watched races in 3D, with a camera system that can present the field clearly while remaining cost-controlled. Full broadcast realism is not required at the start, but the viewing experience must feel like a meaningful part of the game rather than a placeholder.

## 22. Recommended Engine and Technical Stack

### Engine Choice

**Unreal Engine 5** is the recommended starting engine for this project.

### Why

* Better fit for a management sim that also needs visually convincing watched races
* Strong built-in tooling for animation, rigs, cameras, sequences, and cinematic presentation
* Better long-term ceiling for race-day spectacle and polished replay viewing
* Strong desktop publishing path for a premium PC title

### Core Stack

* Engine: Unreal Engine 5
* Language: Blueprints for fast UI and gameplay iteration, with C++ for simulation-heavy or performance-critical systems
* Data: Data Tables / JSON-backed data import for horses, staff, races, and tuning values
* UI: UMG for management screens
* Build target: Windows first
* Steam integration: add after the core loop is stable

### Development Rule

The simulation layer should remain largely engine-agnostic in structure so race viewing can improve without rewriting core management systems.

## 23. Simulation-First Architecture

### Core Principle

The game should be built as a set of plain simulation systems that do not depend on presentation layers.

### Layers

1. **Domain Layer**

   * horse
   * staff
   * race
   * stable
   * facility
   * finance
   * schedule
2. **Simulation Layer**

   * training progression
   * fatigue and recovery
   * injury rolls
   * AI stable decisions
   * race outcome simulation
   * economy updates
3. **Application Layer**

   * game state orchestration
   * turn/week advancement
   * save/load
   * notification generation
4. **Presentation Layer**

   * dashboard
   * horse roster
   * race calendar
   * staff screens
   * race result screens
   * optional race viewer

## 24. Initial Data Model

### Horse

* id
* name
* age
* sex
* attributes

  * speed
  * stamina
  * acceleration
  * consistency
  * temperament
  * recovery
  * durability
* hidden_potential
* current_form
* fitness
* fatigue
* morale
* injury_status
* preferences

  * distance_band
  * surface
  * pace_style
* owner_id
* stable_id
* market_value
* career_record

### Staff

* id
* name
* role
* skill_profile
* salary
* morale
* traits
* stable_id

### Stable

* id
* name
* cash_balance
* reputation
* facilities
* horse_ids
* staff_ids
* owner_expectation

### Race

* id
* name
* date
* surface
* distance
* class_level
* purse
* field_size
* entry_rules

## 25. Core Time Structure

The game will progress primarily in **weekly management ticks** with race-day events inside the week.

### Weekly Cycle

1. training assignment
2. condition update
3. scouting and market updates
4. race entry decisions
5. race execution
6. financial settlement
7. inbox/report generation

This keeps pacing readable and scalable.

## 26. First Playable Milestone

### Goal

Prove the central management loop in a narrow but complete slice.

### Required Features

* New game start
* One player stable
* Horse roster with core stats
* Weekly training assignment
* Fitness, fatigue, and form updates
* Small race calendar
* Race entry flow
* Basic race simulation
* Race results and winnings
* Stable cash tracking
* Save/load

### Definition of Success

A player can complete a short season, make meaningful decisions each week, and understand why their horses improved or declined.

## 27. Race Simulation v1

### Inputs

* horse base attributes
* current fitness
* fatigue
* recent form
* suitability to distance and surface
* race class difficulty
* controlled randomness

### Output

* finishing order
* performance score
* explanation text
* condition changes after race

### Important Rule

The player must always receive a breakdown of why the horse performed as it did.

## 28. UX Starting Screens

1. main dashboard
2. stable roster
3. horse detail
4. weekly training screen
5. race calendar
6. race entry screen
7. race result screen
8. finances screen

## 29. Build Sequence

### Phase 1

Project shell, navigation, core data structures, save framework

### Phase 2

Horse roster, horse detail, weekly tick progression

### Phase 3

Training system, fitness/fatigue/form logic

### Phase 4

Race calendar, race entry, race simulation, result explanations

### Phase 5

3D race prototype with placeholder horses, track spline movement, camera system, watched race playback

### Phase 6

Finance loop, staff, facilities, balancing

### Phase 7

Visual polish, animation refinement, replay presentation, Steam integration, demo packaging

## 30. Immediate Working Backlog

* Set final project name later
* Implement core enums and data definitions
* Implement horse entity and stable entity
* Implement game state manager
* Implement weekly advance flow
* Implement dashboard screen
* Implement horse roster screen
* Implement training assignment screen
* Implement race calendar screen
* Implement first-pass race simulator

## 31. Next Iteration Rule

From this point onward, feature additions should be integrated into this foundation rather than redefining the game at a high level.
