import * as THREE from 'https://unpkg.com/three@0.161.0/build/three.module.js';

const STORAGE_KEY = 'stablemaster-web-save-v2';
const initialState = {
  week: 1,
  cash: 250000,
  horses: [
    { id: 'H-001', name: 'Iron Comet', speed: 78, stamina: 72, acceleration: 80, consistency: 69, fitness: 70, fatigue: 18, form: 64, morale: 66, injury: 0 },
    { id: 'H-002', name: 'Velvet Thistle', speed: 74, stamina: 83, acceleration: 68, consistency: 75, fitness: 73, fatigue: 16, form: 66, morale: 68, injury: 0 },
    { id: 'H-003', name: 'River Quill', speed: 82, stamina: 70, acceleration: 84, consistency: 63, fitness: 68, fatigue: 21, form: 61, morale: 63, injury: 0 },
    { id: 'H-004', name: 'North Banner', speed: 71, stamina: 79, acceleration: 67, consistency: 80, fitness: 75, fatigue: 14, form: 70, morale: 71, injury: 0 },
    { id: 'H-005', name: 'Amber Vow', speed: 76, stamina: 77, acceleration: 74, consistency: 72, fitness: 69, fatigue: 17, form: 65, morale: 64, injury: 0 },
    { id: 'H-006', name: 'Silver Harrier', speed: 79, stamina: 68, acceleration: 82, consistency: 67, fitness: 71, fatigue: 19, form: 63, morale: 65, injury: 0 }
  ],
  logs: []
};

let state = loadState();
let raceResults = [];
let playbackSpeed = 1;
let t = 0;
const rng = () => Math.random();

const el = {
  week: document.getElementById('week'), cash: document.getElementById('cash'), healthy: document.getElementById('healthy'), avgForm: document.getElementById('avgForm'),
  roster: document.getElementById('roster'), insights: document.getElementById('insights'), log: document.getElementById('log'), results: document.getElementById('results'), raceMeta: document.getElementById('raceMeta')
};

function clamp(v, min, max) { return Math.max(min, Math.min(max, v)); }
function saveState() { localStorage.setItem(STORAGE_KEY, JSON.stringify(state)); }
function loadState() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : structuredClone(initialState);
  } catch {
    return structuredClone(initialState);
  }
}

function statusPill(horse) {
  if (horse.injury > 0) return '<span class="pill bad">Injured</span>';
  if (horse.fatigue > 60) return '<span class="pill warn">Tired</span>';
  return '<span class="pill good">Ready</span>';
}

function renderDashboard() {
  const healthyCount = state.horses.filter(h => h.injury === 0).length;
  const avgForm = state.horses.reduce((a, h) => a + h.form, 0) / state.horses.length;
  el.week.textContent = String(state.week);
  el.cash.textContent = `$${Math.round(state.cash / 1000)}k`;
  el.healthy.textContent = String(healthyCount);
  el.avgForm.textContent = avgForm.toFixed(0);

  el.roster.innerHTML = state.horses.map(h => {
    const load = clamp(h.fatigue, 0, 100);
    return `<tr>
      <td>${h.name}</td>
      <td>${statusPill(h)}</td>
      <td>${h.form.toFixed(0)}</td>
      <td><div class="bar"><span style="width:${load}%"></span></div></td>
    </tr>`;
  }).join('');

  const best = [...state.horses].sort((a,b)=>b.form-a.form)[0];
  const mostTired = [...state.horses].sort((a,b)=>b.fatigue-a.fatigue)[0];
  el.insights.innerHTML = `
    <div>Top form: <strong>${best.name}</strong> (${best.form.toFixed(0)})</div>
    <div>Highest fatigue: <strong>${mostTired.name}</strong> (${mostTired.fatigue.toFixed(0)})</div>
    <div>Injured horses: <strong>${state.horses.filter(h => h.injury > 0).length}</strong></div>
  `;

  el.log.innerHTML = state.logs.slice(0, 80).map(x => `<div>${x}</div>`).join('');
  document.getElementById('runRaceBtn').disabled = healthyCount === 0;
  saveState();
}

function addLog(message) {
  state.logs.unshift(`W${state.week}: ${message}`);
  state.logs = state.logs.slice(0, 120);
}

function advanceOneWeek() {
  for (const horse of state.horses) {
    if (horse.injury > 0) {
      horse.injury -= 1;
      horse.fatigue = clamp(horse.fatigue - 8, 0, 100);
      horse.fitness = clamp(horse.fitness + 3, 0, 100);
      addLog(`${horse.name} continues recovery.`);
      continue;
    }

    const intensity = horse.fatigue > 55 ? 0.35 : 0.55 + rng() * 0.3;
    horse.fitness = clamp(horse.fitness + intensity * 4 - horse.fatigue * 0.08, 0, 100);
    horse.fatigue = clamp(horse.fatigue + intensity * (6.8 - horse.stamina * 0.03), 0, 100);
    horse.form = clamp(horse.form + (horse.fitness - horse.fatigue) * 0.04 + (rng() - 0.5) * 3, 0, 100);
    horse.morale = clamp(horse.morale + (horse.form - 50) * 0.02, 20, 100);

    const injuryRisk = clamp((horse.fatigue - 45) / 80 + (intensity - 0.5) * 0.4, 0.01, 0.35);
    if (rng() < injuryRisk) {
      horse.injury = 1 + Math.floor(rng() * 3);
      horse.morale = clamp(horse.morale - 8, 20, 100);
      addLog(`${horse.name} picked up a minor injury (${horse.injury} weeks).`);
    }
  }

  state.week += 1;
  renderDashboard();
}

function simulateRace() {
  const runners = state.horses.filter(h => h.injury === 0);
  if (!runners.length) { addLog('Race canceled: no healthy runners.'); return; }

  raceResults = runners.map(h => {
    const base = h.speed * 0.32 + h.stamina * 0.24 + h.acceleration * 0.16 + h.consistency * 0.14;
    const condition = h.fitness * 0.2 - h.fatigue * 0.18 + h.form * 0.2 + h.morale * 0.08;
    const randomness = (rng() - 0.5) * 14;
    return { id: h.id, name: h.name, score: base + condition + randomness, explanation: `base=${base.toFixed(1)} condition=${condition.toFixed(1)} rand=${randomness.toFixed(1)}` };
  }).sort((a,b)=>b.score-a.score).map((x,i)=>({ ...x, position:i+1 }));

  state.cash += 50000;
  el.raceMeta.textContent = `Week ${state.week} • Spring Cup Trial • 1600m`;
  el.results.innerHTML = raceResults.map(r => `<div class="result-row"><span>#${r.position} ${r.name}</span><span>${r.score.toFixed(1)}</span></div>`).join('');
  addLog(`${raceResults[0].name} won the race.`);
  buildRaceMeshes();
  renderDashboard();
}

function setTab(tab) {
  document.getElementById('panelRoster').style.display = tab === 'roster' ? '' : 'none';
  document.getElementById('panelInsights').style.display = tab === 'insights' ? '' : 'none';
  document.getElementById('panelLog').style.display = tab === 'log' ? '' : 'none';
  document.getElementById('tabRoster').classList.toggle('active', tab === 'roster');
  document.getElementById('tabInsights').classList.toggle('active', tab === 'insights');
  document.getElementById('tabLog').classList.toggle('active', tab === 'log');
}

const viewer = document.getElementById('viewer');
const scene = new THREE.Scene();
scene.background = new THREE.Color(0x0d1324);
const camera = new THREE.PerspectiveCamera(60, viewer.clientWidth / viewer.clientHeight, 0.1, 2000);
camera.position.set(0, 150, 230);
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(viewer.clientWidth, viewer.clientHeight);
viewer.appendChild(renderer.domElement);
scene.add(new THREE.HemisphereLight(0xffffff, 0x25304f, 0.85));
const sun = new THREE.DirectionalLight(0xffffff, 0.8); sun.position.set(150, 250, 120); scene.add(sun);
const ground = new THREE.Mesh(new THREE.PlaneGeometry(1000, 1000), new THREE.MeshStandardMaterial({ color: 0x1f5134, roughness: 0.95 })); ground.rotation.x = -Math.PI / 2; scene.add(ground);
const track = new THREE.Mesh(new THREE.RingGeometry(115, 160, 80), new THREE.MeshStandardMaterial({ color: 0x7d5a3a, side: THREE.DoubleSide })); track.rotation.x = -Math.PI / 2; scene.add(track);

const colors = [0xff5f5f, 0x60a5fa, 0x4ade80, 0xfbbf24, 0xc084fc, 0x22d3ee, 0xf472b6, 0xa3e635];
let horses3D = [];
function buildRaceMeshes() {
  horses3D.forEach(h => scene.remove(h.mesh)); horses3D = [];
  const source = raceResults.length ? raceResults : state.horses.map((h, i) => ({ ...h, position: i + 1 }));
  source.forEach((entry, i) => {
    const mesh = new THREE.Mesh(new THREE.BoxGeometry(7, 4, 3), new THREE.MeshStandardMaterial({ color: colors[i % colors.length] }));
    mesh.position.y = 2; scene.add(mesh);
    horses3D.push({ mesh, lane: i, speed: Math.max(0.3, 1.5 - (entry.position || i + 1) * 0.12) });
  });
}
function animate() {
  requestAnimationFrame(animate);
  t += 0.003 * playbackSpeed;
  horses3D.forEach(h => {
    const p = (t * h.speed + h.lane * 0.02) % 1;
    const ang = p * Math.PI * 2; const radius = 124 + h.lane * 4;
    h.mesh.position.x = Math.cos(ang) * radius; h.mesh.position.z = Math.sin(ang) * radius; h.mesh.rotation.y = -ang;
  });
  camera.lookAt(0, 0, 0);
  renderer.render(scene, camera);
}
window.addEventListener('resize', () => { camera.aspect = viewer.clientWidth / viewer.clientHeight; camera.updateProjectionMatrix(); renderer.setSize(viewer.clientWidth, viewer.clientHeight); });

// events
document.getElementById('advanceWeekBtn').addEventListener('click', advanceOneWeek);
document.getElementById('autoWeekBtn').addEventListener('click', () => { for (let i = 0; i < 4; i += 1) advanceOneWeek(); });
document.getElementById('runRaceBtn').addEventListener('click', simulateRace);
document.getElementById('resetBtn').addEventListener('click', () => {
  localStorage.removeItem(STORAGE_KEY);
  state = structuredClone(initialState);
  raceResults = [];
  el.results.innerHTML = '';
  el.raceMeta.textContent = 'No race run yet.';
  buildRaceMeshes();
  renderDashboard();
});
document.getElementById('tabRoster').addEventListener('click', () => setTab('roster'));
document.getElementById('tabInsights').addEventListener('click', () => setTab('insights'));
document.getElementById('tabLog').addEventListener('click', () => setTab('log'));

document.querySelectorAll('.speed button').forEach(btn => {
  btn.addEventListener('click', () => {
    playbackSpeed = Number(btn.dataset.speed || '1');
    document.querySelectorAll('.speed button').forEach(x => x.classList.remove('active'));
    btn.classList.add('active');
  });
});

renderDashboard();
buildRaceMeshes();
animate();
