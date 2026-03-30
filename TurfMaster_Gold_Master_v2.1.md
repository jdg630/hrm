🏇 PROJECT: TURF MASTER (Technical Blueprint v2.1)
Document Status: Gold Master | Sections: 1–171

01. Core Architecture & Time
Simulation Engine: SQLite3 Relational Database for all persistent data.

The Calendar Year: 52 Weeks (12 Months, 4 Weeks/Month). 2025/2026

The Birthday Logic: All horses age on January 1st (Northern Hemisphere).

Sex Transitions (Males): 2yo-4yo = Colt (c); 5yo+ = Stallion/Horse (h).

Sex Transitions (Females): 2yo-4yo = Filly (f); 5yo+ = Mare (m).

The Gelding (g): Permanent status for castrated males. Logic: IF sex == 'Gelding' THEN is_sterile = TRUE.

Regional Logic: Separate databases for UK, US, HK, AUS, UAE.

Currency: GBP for UK. EUR for Ireland/France. USD for USA. Yen for Japan. 

Time-Step: Manual "Next Day" processing.

Auto-Save: Commit to DB on Race Finish and Day Advance.

02. Physical Genetics & Attributes
DNA Inheritance: 70% Parents / 20% Ancestry / 10% Mutation.

Stride Length: (1-100) Longer stride = +Speed on Flat/Galloping tracks (Newmarket).

Balance: (1-100) Higher balance = -Speed Penalty on Hills/Cambers (Epsom/Goodwood).

Stamina Floor/Ceiling: Min/Max distance range (e.g., 5f to 12f).

Heart (Guts): Chance to maintain top speed for 100m after stamina is 0.

Surface Pref: Turf, Dirt, All-Weather.

Going Pref: Firm, Good, Soft, Heavy (Turf) / Fast, Slow (Dirt).

Maturity Rate: Determines if a horse peaks at age 2, 3, or 4.

Degradation: Performance stats decay at age 6 (Stallions) or 7 (Geldings).

Weight Carrying: Height (hands) and Strength determine speed loss under heavy jockey weights.

03. The Psychology (Quirks & Temperament)
The Quirk Stat: (1-20) 1 = Professional, 20 = Rogue.

Hanging: Probability to drift left/right under pressure.

Loading: Risk of being 'Stunned' or withdrawn at the starting stalls.

Bolting: Risk of burning 15% stamina before the race starts.

Gelding Benefit: Gelding a Colt/Stallion reduces Quirk by 5-10.

Daily Variance: Small random +/- to stats based on "Morning Mood."

Travel Stress: Condition loss based on distance from home stable to track.

Stable Happiness: Faster fitness recovery if staff/facilities are high-tier.

Injury Logic: 'Fragility' stat checked against 'Going' (Hard ground = higher risk).

Recovery Speed: Rate at which 'Condition' returns to 100%.

04. Racing Physics & Topography
Elevation Nodes: XYZ coordinates from Blender heightmaps.

Uphill Gradient: 1.2x stamina drain on slopes > 3%.

Downhill Gradient: Increases stumbling risk (Balance check).

The Draw: Stall assignment logic relative to the first turn.

Kickback: Reduces 'Focus' of horses behind the lead on Dirt/AW.

Wind/Drafting: 5% stamina save when running directly behind another horse.

Fighting the Bit: 'Aggressive' horses burn extra stamina if restrained.

Collision: Ray-casting to prevent horses clipping through each other.

Camber Tilt: Lateral force pushing horses toward the rail on turns.

Turn of Foot: Acceleration curve from 80% to 100% velocity.

05. Bloodstock & The Breeding Barn
Main Category: [Bloodstock].

Sub-Menu A: [Breeding Barn] (Owned Broodmares).

Sub-Menu B: [Stallion Register] (Global Sire Database).

AEI (Average Earnings Index): Progeny earnings vs average.

CI (Comparable Index): Quality of mares covered by the sire.

SW% (Stakes Winner %): % of foals winning Group/Listed races.

AWD (Average Winning Distance): Displays sire's stamina influence.

Black Type Stats: Count of G1, G2, G3, and Listed wins for horse and progeny.

Nicking Score: Algorithm comparing sire/dam bloodline success.

Stud Fees: Auto-calculated: (G1_Wins * 5000) + (AEI * 2000).

06. The UI & Dashboard
Dashboard Widget A: Stable Snapshot (Quick-view of active runners).

Dashboard Widget B: Interactive Calendar (Displays next 14 days).

Dashboard Widget C: Finance Chart (Monthly P&L visualization).

Distance Format Logic: Render furlongs as standard UK text (e.g., 10f = '1m 2f').

Navigation: Sidebar [Home], [Stable], [Racecourse], [Bloodstock], [Office].

Contextual Popups: Hover over stats like AEI to see definitions.

Process Button: 'Next Day' triggers world-state update.

The Ledger: Categorized spending (Vet, Entry, Training, Stud).

Equipment Toggle: Visual icons for Blinkers/Hoods in Stable UI.

Global Search: Find any horse by name or ID.

07. UK Calendar & Classic Restrictions
Classic Restriction: NO Geldings in Guineas, Derby, Oaks, St Leger.

2000 Guineas: 1m | 3yo Colts & Fillies.

1000 Guineas: 1m | 3yo Fillies Only.

The Derby: 1m 4f | 3yo Colts & Fillies.

The Oaks: 1m 4f | 3yo Fillies Only.

The St Leger: 1m 6f | 3yo Colts & Fillies.

Sex Allowance: Fillies/Mares carry 3lbs less in 'Open' races.

Open G1 Races: (Eclipse, Juddmonte, etc.) Allow Stallions, Mares, Geldings.

Maiden Rules: Horse is ineligible once total_wins > 0.


08. Validation & Gelding
The Gelding Warning: Modal confirmation: "Permanent action. Lost Stud Value."

Gender Locking: 'Geld' action disabled for females.

Weight-for-Age (WFA): 3yos carry less than 4yos based on  BHA scale.

Eligibility Check: Validate_Entry(Horse, Race) -> Boolean logic.

The Stallion Trigger: Retirement moves 'Stallion' from Stable to Stud.

Final Validation: Check Age, Sex, and Rating before entry commitment.

09. Training & Gallops
The Gallop (Work-In): Private trial sub-menu in Stable Hub.

Setup: Select Subject, Lead Horse (Schooler), Distance, and Intensity.

Intensity Risk: Strong work = +Fitness, +Fatigue, 2% Injury Risk.

Jockey Narrative Engine: 20+ authentic phrases based on performance vs. schooler.

Speed Figures: Hidden numerical rating (0-120) for private tracking.

Condition Penalty: Strong gallops reduce 'Condition' by 15%.

10. Economy & Handicapping
Public Auction: Real-time bidding ticker for Yearlings and HIT Sales.

Automatic Handicapper: Post-race script. Winner rises 4-10pts; losers drop 1-3pts.

Jockey Entities: Traits like 'Ice Man' (Stamina) or 'The Barker' (Aggression).

Year-End Awards: Champion Sire, Horse of the Year, Leading Owner prestige.

Veterinary Suite: Treatments for Wind Ops, Heat in Leg, and Stress Fractures.

Rival AI Stables: AI trainers with specific specializations.

Legacy Export: Export retired legends to new save files.

11. Jockey & Declaration Logic
Apprentice Claim Restriction: No claims allowed in G1, G2, G3, or Listed races.

Dual-Code Wins: Separate win counters for Flat and Jump racing.

Flat Claim Scale: 7lb (0-19 wins), 5lb (20-49), 3lb (50-94), 0lb (95+).

Jump Claim Scale: 7lb (0-19 wins), 5lb (20-39), 3lb (40-74), 0lb (75+).

48-Hour Declaration Window: Point where final field, draw, and weights are frozen.

Declaration Table: Stores "Snapshot" data to prevent mid-week stat changes.

Balloting: Horses with lowest OR are removed if race exceeds Safety Limit.

The 3-Day Delay: Jockey claim is fixed at declaration time regardless of interim wins.

Stall Draw: Random assignment conducted at the 48hr window.

Non-Runner Logic: Horses withdrawn after declaration incur a fine.

Weight Calculation Engine: Final Weight = (Base - Sex - Apprentice + Penalty).

Unrated Rule: Horses receive an OR only after winning or 3 career runs.

12. Technical Stack & Data Architecture
Primary Logic Language: Python 3.11+. (NumPy/Pandas integration).

Data Storage (Persistence): SQLite3. Relational database for all "stateful" data.

Configuration & Rulesets: JSON. For WFA scales, Calendars, and Trait definitions.

Architecture Pattern: Model-View-Controller (MVC).

The "Next Day" Script: Automated batch processing for Age/Birthday checks, Recovery, AI Entries, Declarations, Race Execution, Handicapping, and DB Commits.
