# Quick Reference: Logical Commit Grouping

## One-Minute Workflow

```bash
# 1. Stage interactively (one group at a time)
git add -p

# 2. Review what you're staging
git diff --cached

# 3. Commit with conventional format
git commit -m "feat(scope): subject"

# 4. Repeat for next group
git add -p
git diff --cached
git commit -m "fix(scope): subject"
```

---

## git add -p Reference

**Interactive staging lets you review hunks one-by-one.**

```
Stage this hunk [y,n,s,e,?,q]?
  y = yes, stage this hunk
  n = no, skip this hunk
  s = split (break hunk into smaller pieces)
  e = edit (manually edit the hunk, advanced)
  ? = help
  q = quit
```

---

## Commit Type Cheat Sheet

| Type | Use Case | Example |
|------|----------|---------|
| `feat` | New feature | `feat(auth): add token refresh` |
| `fix` | Bug fix | `fix(posts): validate title` |
| `refactor` | Code improvement (no behavior change) | `refactor(db): extract queries` |
| `docs` | Documentation only | `docs(api): update endpoint docs` |
| `test` | Tests only | `test(auth): add token tests` |
| `style` | Code formatting (no logic change) | `style: format code` |
| `perf` | Performance improvement | `perf(queries): optimize N+1 lookups` |
| `chore` | Build, config, tooling | `chore: upgrade dependencies` |

---

## Commit Format

```
<type>(<scope>): <subject (50 chars max, imperative)>

<body (optional, wrap at 72 chars)>
Explain WHY and WHAT, not HOW
Mention motivation and contrasts

<footer (optional)>
Fixes #123
Closes #456
```

---

## Common Scenarios

### Scenario: Fix + Feature in Same Branch

```bash
# Stage only fix hunks
git add -p                          # Answer 'y' only to fix hunks
git commit -m "fix(posts): validate title"

# Stage only feature hunks
git add -p                          # Answer 'y' only to feature hunks
git commit -m "feat(posts): add markdown"
```

### Scenario: Multiple Unrelated Files

```bash
# Stage files for first commit
git add path/to/file1.cs path/to/file2.cs
git commit -m "fix(auth): prevent expired tokens"

# Stage files for second commit
git add path/to/file3.cs path/to/file4.cs
git commit -m "feat(posts): add comments section"
```

### Scenario: Split a Large Hunk

```bash
git add -p
# When you see the large hunk:
Stage this hunk [y,n,s,e,?,q]? s
# Git splits it into smaller pieces, review each
```

### Scenario: Wrong File Staged

```bash
# Unstage it
git reset HEAD path/to/file.cs

# Stage only what you want
git add -p
git commit
```

---

## Before Pushing

```bash
# See your commits vs main
git log --oneline origin/main..HEAD

# See full details
git log -p origin/main..HEAD

# See commit count
git rev-list --count origin/main..HEAD
```

Expected: 1-5 focused commits, each with a clear, conventional message.

---

## Undo Mistakes

| Mistake | Fix |
|---------|-----|
| Staged wrong file | `git reset HEAD path/to/file` |
| Forgot a file in commit | `git add <file>` then `git commit --amend --no-edit` |
| Wrong commit message | `git commit --amend -m "new message"` |
| Committed before staging | `git reset --soft HEAD~1` then `git add -p` and recommit |

---

## Pro Tips

✅ **DO**:
- Stage one logical group at a time
- Review staged changes before committing
- Write commit messages in imperative mood ("Add feature" not "Added feature")
- Reference issue numbers in commit messages (`Fixes #123`)
- Commit frequently (don't wait for end of day)

❌ **DON'T**:
- Mix unrelated changes in one commit
- Force push to shared branches (`git push --force`)
- Leave commits with messages like "WIP" or "Fix stuff"
- Commit files you haven't tested
- Amend commits already pushed to shared branches

---

## Setup (One-Time)

```bash
# Configure commit template
git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt

# Optional: Create helpful aliases
git config alias.add-patch "add -p"
git config alias.staged "diff --cached"
git config alias.commits "log --oneline origin/main..HEAD"
```

---

**For detailed guidance**: See `.github/skills/logical-commit-grouping/SKILL.md`  
**For setup**: See `.github/skills/logical-commit-grouping/SETUP.md`
