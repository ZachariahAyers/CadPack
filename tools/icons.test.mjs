// Every icon the ribbon names is embedded, and every embedded icon is used.
// Materials.cs derives material icon names from layer names; materialIcon mirrors that rule.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readdirSync, readFileSync } from 'node:fs';

const src = new URL('../src/CadPack/', import.meta.url);
const read = (path) => readFileSync(new URL(path, src), 'utf8');

export const materialIcon = (name) => `material-${name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-+|-+$/g, '')}.png`;

const toolIcons = [...read('Ribbon/Tools.cs').matchAll(/"([\w-]+\.png)"/g)].map((m) => m[1]);
const materialNames = [...read('Materials.cs').matchAll(/new\("([^"]+)", \d+\)/g)].map((m) => m[1]);
const files = readdirSync(new URL('Icons/', src));

test('the tables were parsed', () => {
  assert.ok(toolIcons.length > 80, `found ${toolIcons.length} tool icons`);
  assert.equal(materialNames.length, 51);
});

test('every icon the ribbon names exists', () => {
  const missing = [...toolIcons, ...materialNames.map(materialIcon)].filter((f) => !files.includes(f));
  assert.deepEqual(missing, []);
});

test('every icon file is used', () => {
  const used = new Set([...toolIcons, ...materialNames.map(materialIcon)]);
  assert.deepEqual(files.filter((f) => !used.has(f)), []);
});

test('material icon names follow the slug rule', () => {
  assert.equal(materialIcon('PVC Edge - xxx (.02) .5MM'), 'material-pvc-edge-xxx-02-5mm.png');
  assert.equal(materialIcon('MDF - Raw (1.0)'), 'material-mdf-raw-1-0.png');
});
