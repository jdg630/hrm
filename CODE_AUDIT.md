# Stablemaster Prototype — Code Audit (March 30, 2026)

## Scope
- `src/Stablemaster.Sim/*`
- `race-viewer/*`
- `data/*.json`

## Checks run
- JavaScript syntax check: `node --check race-viewer/viewer.js`
- JSON structural validation for all data files via `python3 -m json.tool`
- Manual code path review for runtime exceptions in simulation and data loading.

## Issues found

### 1) Potential runtime exception in weekly simulation
**File:** `src/Stablemaster.Sim/SimulationEngine.cs`  
**Problem:** `plans.First(...)` would throw if a horse had no matching generated plan.  
**Impact:** Hard crash during weekly advance.

**Fix applied:**
- Replaced strict `First(...)` with `FirstOrDefault(...)` and fallback recovery plan.

### 2) Unclear failure mode when required data files are missing
**File:** `src/Stablemaster.Sim/DataLoader.cs`  
**Problem:** File reads assumed all JSON files existed; missing file errors were implicit and less actionable.  
**Impact:** Startup failure with less explicit diagnostics.

**Fix applied:**
- Added explicit existence checks and descriptive `FileNotFoundException` messages before deserialization.

## Remaining non-blocking risks
- `schema.sql` and inline schema in `RelationalStore` are duplicated and could drift over time.
- No automated unit/integration tests currently enforce simulation consistency.
- Browser simulation and C# simulation are separate implementations and may diverge behaviorally.

## Recommendation
Short-term:
1. Add test project for simulation formula regression and DB seed/persist integration.
2. Generate schema from a single source (or load `schema.sql` directly) to prevent drift.
3. Decide whether browser mode should consume C#-produced race outputs to reduce logic duplication.
