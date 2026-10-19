import { test } from 'node:test';
import assert from 'node:assert/strict';
import { mkdtempSync, mkdirSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { checkLisp, checkDcl, checkDim, referencedFiles, checkTree } from './check-lisp.mjs';

const messages = (errors) => errors.map((e) => `${e.line}: ${e.message}`);

test('balanced lisp passes', () => {
  assert.deepEqual(checkLisp('(defun c:x (/ a)\n  (setq a 1)\n  (princ))\n'), []);
});

test('extra close paren is reported on its line', () => {
  assert.deepEqual(messages(checkLisp('(princ)\n(princ))\n')), ["2: unexpected ')'"]);
});

test('unclosed paren is reported where it opened', () => {
  assert.deepEqual(messages(checkLisp('(princ)\n(defun c:x ()\n  (princ)\n')), ["2: unclosed '(' (paren)"]);
});

test('parens inside strings and comments are ignored', () => {
  const text = '(princ "(")\n; ) stray\n(princ "a \\" )") ;| ( |;\n';
  assert.deepEqual(checkLisp(text), []);
});

test('multi-line block comment keeps line numbers right', () => {
  assert.deepEqual(messages(checkLisp(';| one\ntwo |;\n)\n')), ["3: unexpected ')'"]);
});

test('unterminated string is reported', () => {
  assert.deepEqual(messages(checkLisp('(princ "oops)\n')), ['1: unterminated string', "1: unclosed '(' (paren)"]);
});

test('dcl braces balance outside comments and strings', () => {
  assert.deepEqual(checkDcl('a : dialog { label = "}"; // }\n /* { */ ok_only; }\n'), []);
  assert.deepEqual(messages(checkDcl('a : dialog {\n  : row {\n}\n')), ["1: unclosed '{' (brace)"]);
});

test('dim rows must each be one list', () => {
  assert.deepEqual(checkDim('("W44x285" 44.02 1.025)\n\n("W40" 1 2)\n'), []);
  assert.deepEqual(messages(checkDim('("W44x285" 44.02\n("a" 1) ("b" 2)\n"W40" 1\n("a)" 1)\n')), [
    "1: unclosed '(' (paren)",
    '2: row is not a single list',
    '3: row is not a single list',
  ]);
});

test('referenced support files are found by literal name', () => {
  const text = '(open (findfile "stl_w.dim") "r") (load_dialog "WELD.DCL") (load_dialog (findfile "stl.dcl"))';
  assert.deepEqual(referencedFiles(text).sort(), ['WELD.DCL', 'stl.dcl', 'stl_w.dim']);
});

test('tree check reports a missing support file', () => {
  const root = mkdtempSync(join(tmpdir(), 'cadpack-lisp-'));
  mkdirSync(join(root, 'support'));
  writeFileSync(join(root, 'a.lsp'), '(load_dialog "a.dcl")\n(open (findfile "MISSING.dim") "r")\n');
  writeFileSync(join(root, 'support', 'A.DCL'), 'a : dialog { ok_only; }\n');
  assert.deepEqual(checkTree(root).map((r) => r.message), ['support file not found: MISSING.dim']);
});

test('the shipped lisp tree is clean', () => {
  assert.deepEqual(checkTree(new URL('../lisp', import.meta.url).pathname), []);
});
