#!/usr/bin/env python3
import json, pathlib, sys
ROOT = pathlib.Path(__file__).resolve().parents[1]
SHEETS = ROOT / 'sheets'
errors=[]; warnings=[]; data={}
required = {
 'project.json':['id','workingTitle','version','hostGame','requiredGames','players','tagline','buildStatus','meltyStatus'],
 'weapons.json':['id','displayName','kind','damagePercent','baseKnockback','range','cooldown','projectiles','spread','moveSpeedMultiplier','pickupWeight','verified'],
 'cards.json':['id','displayName','description','rarity','movementSpeedMultiplier','jumpMultiplier','extraJumps','outgoingKnockbackMultiplier','incomingKnockbackMultiplier','attackCooldownMultiplier','weaponRangeMultiplier','verified'],
 'systems.json':['id','trigger','implementation','dependsOn','verified'],
 'hooks.json':['id','target','method','hookType','system','verified'],
}
for fname in required:
    p=SHEETS/fname
    if not p.exists(): errors.append(f'missing sheet: {fname}'); continue
    try: data[fname]=json.loads(p.read_text(encoding='utf-8'))
    except Exception as e: errors.append(f'{fname}: invalid JSON: {e}')

for fname, cols in required.items():
    if fname not in data: continue
    if fname=='project.json':
        row=data[fname]
        for c in cols:
            if c not in row or row[c] in ('',None,[]): errors.append(f'{fname}: unfilled cell {c}')
        continue
    rows=data[fname].get('rows')
    if not isinstance(rows,list) or not rows: errors.append(f'{fname}: rows missing/empty'); continue
    ids=set()
    for i,row in enumerate(rows):
        rid=row.get('id',f'row#{i}')
        if rid in ids: errors.append(f'{fname}: duplicate id {rid}')
        ids.add(rid)
        for c in cols:
            if c not in row or row[c] is None or row[c]=='': errors.append(f'{fname}:{rid}: unfilled cell {c}')
        if row.get('verified') is not True: errors.append(f'{fname}:{rid}: verified must be true before generation')

systems={r['id'] for r in data.get('systems.json',{}).get('rows',[]) if 'id' in r}
for row in data.get('systems.json',{}).get('rows',[]):
    for dep in row.get('dependsOn',[]):
        if dep not in systems: errors.append(f'systems.json:{row.get("id")}: unresolved dependsOn -> {dep}')
for row in data.get('hooks.json',{}).get('rows',[]):
    if row.get('system') not in systems: errors.append(f'hooks.json:{row.get("id")}: unresolved system -> {row.get("system")}')

project=data.get('project.json',{})
if project.get('players',{}).get('max')!=2: errors.append('project.json: first playable must have maxPlayers=2')
if project.get('hostGame')!='ROUNDS': errors.append('project.json: hostGame must be ROUNDS for this design')
if 'ROUNDS' not in project.get('requiredGames',[]): errors.append('project.json: ROUNDS must be required')
if 'Brawlhalla' in project.get('requiredGames',[]): warnings.append('Brawlhalla is marked required; only do this if runtime genuinely consumes the player-owned install.')

print('Brawl ROUNDS preflight')
print('======================')
if errors:
    print(f'FAIL: {len(errors)} issue(s)')
    for e in errors: print(' -',e)
else:
    rows=sum(len(data[f].get('rows',[])) for f in ['weapons.json','cards.json','systems.json','hooks.json'] if f in data)
    print(f'PASS: all required cells and references resolve ({rows} rows checked).')
for w in warnings: print('WARN:',w)
sys.exit(1 if errors else 0)
