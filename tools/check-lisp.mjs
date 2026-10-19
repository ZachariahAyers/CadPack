#!/usr/bin/env node
// Structural check for the AutoLISP routines and their support files.
// AutoCAD only offers a paren checker inside the Visual LISP IDE, so CI runs this instead.
//   .lsp  balanced parentheses outside strings and comments
//   .dcl  balanced braces outside strings and comments
//   .dim  one complete list per line (the routines read each line with `read`)
//   every file a routine opens by name with findfile or load_dialog exists in support/
import { readdirSync, readFileSync } from 'node:fs';
import { join, extname } from 'node:path';
import { fileURLToPath } from 'node:url';

// Walks text and calls onChar for each character that is code, not string or comment.
function walk(text, { line, block, onChar }) {
  const errors = [];
  let ln = 1;
  for (let i = 0; i < text.length; i++) {
    const c = text[i];
    if (c === '\n') { ln++; continue; }
    if (text.startsWith(block[0], i)) {
      const end = text.indexOf(block[1], i + block[0].length);
      if (end < 0) { errors.push({ line: ln, message: 'unterminated block comment' }); break; }
      ln += countNewlines(text, i, end);
      i = end + block[1].length - 1;
      continue;
    }
    if (text.startsWith(line, i)) {
      const end = text.indexOf('\n', i);
      if (end < 0) break;
      i = end - 1;
      continue;
    }
    if (c === '"') {
      const start = ln;
      let j = i + 1;
      for (; j < text.length && text[j] !== '"'; j++) {
        if (text[j] === '\\') j++;
        else if (text[j] === '\n') ln++;
      }
      if (j >= text.length) { errors.push({ line: start, message: 'unterminated string' }); break; }
      i = j;
      continue;
    }
    const err = onChar(c, ln);
    if (err) errors.push(err);
  }
  return errors;
}

function countNewlines(text, from, to) {
  let n = 0;
  for (let i = from; i < to; i++) if (text[i] === '\n') n++;
  return n;
}

function balance(text, syntax, open, close, name) {
  const stack = [];
  const errors = walk(text, {
    ...syntax,
    onChar(c, ln) {
      if (c === open) stack.push(ln);
      else if (c === close && stack.pop() === undefined) return { line: ln, message: `unexpected '${close}'` };
      return null;
    },
  });
  for (const ln of stack) errors.push({ line: ln, message: `unclosed '${open}' (${name})` });
  return errors;
}

const LISP = { line: ';', block: [';|', '|;'] };
const DCL = { line: '//', block: ['/*', '*/'] };

export const checkLisp = (text) => balance(text, LISP, '(', ')', 'paren');
export const checkDcl = (text) => balance(text, DCL, '{', '}', 'brace');

export function checkDim(text) {
  const errors = [];
  text.split('\n').forEach((raw, i) => {
    const row = raw.trim();
    if (!row) return;
    const rowErrors = checkLisp(row);
    for (const e of rowErrors) errors.push({ line: i + 1, message: e.message });
    if (!rowErrors.length && firstListEnd(row) !== row.length - 1) {
      errors.push({ line: i + 1, message: 'row is not a single list' });
    }
  });
  return errors;
}

// Index of the paren that closes the list opening at index 0, or -1.
function firstListEnd(row) {
  if (row[0] !== '(') return -1;
  const code = row.replace(/"(?:\\.|[^"\\])*"/g, (s) => ' '.repeat(s.length));
  let depth = 0;
  for (let i = 0; i < code.length; i++) {
    if (code[i] === '(') depth++;
    else if (code[i] === ')' && --depth === 0) return i;
  }
  return -1;
}

// File names a routine opens by literal name.
export function referencedFiles(text) {
  const names = new Set();
  for (const m of text.matchAll(/\((?:findfile|load_dialog)\s+"([^"]+)"/gi)) names.add(m[1]);
  return [...names];
}

export function checkTree(root) {
  const supportDir = join(root, 'support');
  const support = readdirSync(supportDir);
  const supportLower = new Set(support.map((f) => f.toLowerCase()));
  const results = [];
  const report = (file, errors) => errors.forEach((e) => results.push({ file, ...e }));

  for (const f of readdirSync(root).filter((f) => extname(f).toLowerCase() === '.lsp')) {
    const text = readFileSync(join(root, f), 'utf8');
    report(join(root, f), checkLisp(text));
    for (const name of referencedFiles(text)) {
      if (!supportLower.has(name.toLowerCase())) {
        results.push({ file: join(root, f), line: 0, message: `support file not found: ${name}` });
      }
    }
  }
  for (const f of support) {
    const path = join(supportDir, f);
    const ext = extname(f).toLowerCase();
    if (ext === '.dcl') report(path, checkDcl(readFileSync(path, 'utf8')));
    if (ext === '.dim') report(path, checkDim(readFileSync(path, 'utf8')));
  }
  return results;
}

if (process.argv[1] === fileURLToPath(import.meta.url)) {
  const root = process.argv[2] ?? 'lisp';
  const results = checkTree(root);
  for (const r of results) console.error(`${r.file}:${r.line}: ${r.message}`);
  if (results.length) process.exit(1);
  console.log(`lisp check passed: ${root}`);
}
