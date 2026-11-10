"""Validate public archive contents, manifest identities, hashes and privacy boundaries."""
from pathlib import Path
import hashlib,json,re,zipfile,sys
root=Path(__file__).resolve().parents[1]
mods=json.loads((root/'modules.json').read_text(encoding='utf8'))
assert len(mods)==7
ids=[]
for m in mods:
    manifest=json.loads((root/'src'/m['slug']/'plugin.json').read_text(encoding='utf8'))
    ids.append(manifest['ID'])
    assert manifest['ExecuteFileName']==m['slug']+'.dll'
    source=(root/'src'/m['slug']/'Main.cs').read_text(encoding='utf-8-sig')
    assert manifest['ID'] in source
    assert 'QueryTextDisplay=query.RawQuery' in source.replace(' ','')
assert len(set(ids))==7
for folder in ['src','scripts','docs']:
    for p in (root/folder).rglob('*'):
        if p.is_file() and p.suffix in {'.cs','.ps1','.md','.ini','.json'}:
            text=p.read_text(encoding='utf-8-sig')
            assert not re.search(r'(?i)([A-Z]:\\Users\\(?!Public)[^\\\s]+|[A-Z]:\\Documents\\Tuya|wbSeedToken|accountSnapshot|gh[pousr]_[A-Za-z0-9]{20,})',text),str(p)
release=Path(sys.argv[1]).resolve() if len(sys.argv)>1 else root/'release'
sums=release/'SHA256SUMS.txt'
if sums.exists():
    for line in sums.read_text(encoding='utf8').splitlines():
        expected,name=line.split(None,1);p=release/name.strip()
        assert hashlib.sha256(p.read_bytes()).hexdigest()==expected,p.name
    zips=list(release.glob('*.zip'));assert len(zips)==7
    for p in zips:
        with zipfile.ZipFile(p) as z:
            names=z.namelist()
            assert not any(x in n.lower() for n in names for x in ['draft-backups','actions.log','status.txt','before-','settings.json','wox.plugin.dll','sogou','clipboard'])
            dlls=[n for n in names if n.endswith('.dll')];assert len(dlls)==1
            manifest_name=next(n for n in names if n.endswith('/plugin.json'))
            manifest=json.loads(z.read(manifest_name));prefix=manifest_name.rsplit('/',1)[0]
            assert prefix+'/'+manifest['ExecuteFileName'] in names
            ini=z.read(prefix+'/settings.ini').decode('utf-8-sig');assert re.search(r'^AppPath=\r?$',ini,re.M)
            assert {'Install.ps1','Uninstall.ps1','LICENSE','README.md','README.zh-CN.md'}.issubset(names)
    print('PASS: seven independent archives, DLL manifests, SHA-256 checksums and privacy allowlist.')
else:
    print('PASS: seven source manifests and portable source checks; build packages for archive checks.')
