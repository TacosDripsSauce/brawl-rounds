#!/usr/bin/env python3
import json, re, sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
errs=[]
counts={}
for name,folder,suffix in [
    ('weapons','Weapons','Weapon.g.cs'),('cards','Cards','Card.g.cs'),('systems','Systems','SystemRow.g.cs'),('hooks','Hooks','HookRow.g.cs')]:
    rows=json.loads((ROOT/'sheets'/f'{name}.json').read_text())['rows']
    counts[name]=len(rows)
    files=list((ROOT/'src'/'BrawlRounds'/'Generated'/folder).glob('*.g.cs'))
    if len(files)!=len(rows): errs.append(f'{name}: {len(rows)} rows but {len(files)} generated files')

# Project invariants that must stay true for the first release.
p=json.loads((ROOT/'sheets'/'project.json').read_text())
if p.get('hostGame')!='ROUNDS': errs.append('project.hostGame must be ROUNDS')
if p.get('requiredGames')!=['ROUNDS']: errs.append('v0.1 must not fake Brawlhalla as a required game')
if p.get('players',{}).get('max')!=2: errs.append('v0.1 max players must be 2')

# Publishing hygiene.
for f in ROOT.rglob('*'):
    if not f.is_file() or '.git' in f.parts: continue
    if f.suffix.lower() in {'.dll','.exe','.pdb'}: errs.append(f'proprietary/binary artifact committed: {f.relative_to(ROOT)}')
    try: text=f.read_text(errors='ignore')
    except Exception: continue
    if re.search(r'Bearer\s+[A-Za-z0-9_\-]{20,}',text): errs.append(f'possible bearer token in {f.relative_to(ROOT)}')
    marker = 'll' + 'c_'
    if marker in text: errs.append(f'possible Melty credential marker in {f.relative_to(ROOT)}')
    if 'https://github.com/' in text and 'github.com/rehan-remade' not in text and f.name=='Plugin.cs': errs.append('Plugin.cs contains placeholder GitHub credit URL')

if errs:
    print('STATIC CHECK FAIL')
    for e in errs: print(' -',e)
    sys.exit(1)
print('STATIC CHECK PASS')
print('Generated rows:', ', '.join(f'{k}={v}' for k,v in counts.items()))
print('No committed DLL/EXE/PDB or Melty credential marker found.')
