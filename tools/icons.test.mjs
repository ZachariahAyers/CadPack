// Every icon the ribbon names is embedded, and every embedded icon is used.
// Material icons are named by Materials.cs from the layer name; this test only counts them.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readdirSync, readFileSync } from 'node:fs';

const src = new URL('../src/CadPack/', import.meta.url);
const read = (path) => readFileSync(new URL(path, src), 'utf8');

const toolIcons = [...read('Ribbon/Tools.cs').matchAll(/"([\w-]+\.png)"/g)].map((m) => m[1]);
const materialNames = [...read('Materials.cs').matchAll(/new\("([^"]+)", \d+\)/g)].map((m) => m[1]);
const files = readdirSync(new URL('Icons/', src));
const materialFiles = files.filter((f) => f.startsWith('material-'));

test('the tables were parsed', () => {
  assert.ok(toolIcons.length > 80, `found ${toolIcons.length} tool icons`);
  assert.equal(materialNames.length, 51);
});

test('every icon the ribbon names exists', () => {
  assert.deepEqual(toolIcons.filter((f) => !files.includes(f)), []);
});

test('every material layer has one icon', () => {
  assert.equal(materialFiles.length, materialNames.length);
});

test('every other icon file is used', () => {
  const used = new Set(toolIcons);
  assert.deepEqual(files.filter((f) => !f.startsWith('material-') && !used.has(f)), []);
});
