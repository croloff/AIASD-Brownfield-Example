# Logical Commit Grouping Skill

**Organize and commit changes in focused, logical groups for clean Git history.**

## What Is This?

A comprehensive skill for using Git's interactive staging (`git add -p`) to organize changes into focused, conventional commits. This enables:

- 🎯 **Focused commits** — Each commit addresses one concern (bug fix, feature, refactor)
- 📖 **Clean history** — Easy to understand what changed and why
- 🔍 **Better reviews** — PRs are easier to review with organized commits
- 🐛 **Easier debugging** — `git bisect` works better with focused commits
- 📝 **Conventional format** — Follows conventional commit standards

## Files in This Skill

| File | Purpose |
|------|---------|
| **SKILL.md** | Complete guide with step-by-step workflow, scenarios, and techniques |
| **QUICK-REFERENCE.md** | One-page cheat sheet for quick lookup |
| **SETUP.md** | Initial setup, team configuration, and troubleshooting |
| **assets/commit-template.txt** | Git commit message template with guidance |

## Quick Start

### 1. Setup (One-Time)

```bash
# Configure the commit template
git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt
```

### 2. Organize Your Changes

```bash
# See what changed
git status
git diff --stat

# Stage one logical group interactively
git add -p

# Review what you're committing
git diff --cached

# Commit with conventional message
git commit -m "feat(scope): description"

# Repeat for next group
git add -p
git commit -m "fix(scope): description"
```

### 3. Verify Before Pushing

```bash
# See your commits
git log --oneline origin/main..HEAD

# Looks good? Push to PR
git push origin feature-branch
```

## When to Use This Skill

✅ Use when:
- You have mixed changes (bug fixes + features) that should be separate commits
- You're preparing a PR with multiple logical commits
- You want to organize commits following conventional commit format
- You want to improve code review experience with focused commits
- You need to split a large changeset into reviewable pieces

❌ Don't use when:
- You just need quick, local commits (single logical change)
- You're fixing a typo (one small, focused change)
- You're in a hurry (but it saves time long-term!)

## Key Concept: `git add -p` (Interactive Staging)

**`git add -p`** lets you review Git hunks one-by-one and choose which to stage:

```bash
git add -p
# Git shows each hunk of changes
Stage this hunk [y,n,s,e,?,q]?
  y = stage this hunk
  n = skip this hunk
  s = split hunk (break into smaller pieces)
  e = edit manually (advanced)
  q = quit
```

This is the **core technique** that lets you separate unrelated changes into focused commits.

## Commit Format

Follow conventional commits:

```
<type>(<scope>): <subject>

<optional body explaining WHY>

<optional footer with issue references>
```

**Types**: `feat`, `fix`, `refactor`, `docs`, `test`, `style`, `perf`, `chore`

**Example**:

```
feat(auth): add JWT token refresh mechanism

Previously, tokens never refreshed. Users had to re-login when tokens expired.
Now, clients can refresh tokens without re-authenticating.

Implements endpoint POST /auth/refresh-token
Updates JWT configuration in appsettings.json

Fixes #123
```

## Common Workflows

### Clean Up Mixed Changes

```bash
git add -p          # Pick hunks for fix
git commit -m "fix(posts): validate title"

git add -p          # Pick hunks for feature
git commit -m "feat(posts): add markdown"
```

### Organize by Type (Conventional)

```bash
# Commit 1: Bug fixes
git add -p
git commit -m "fix(auth): prevent expired token acceptance"

# Commit 2: Features
git add -p
git commit -m "feat(auth): add token refresh endpoint"

# Commit 3: Refactoring
git add -p
git commit -m "refactor(auth): extract token validation"

# Commit 4: Tests
git add tests/
git commit -m "test(auth): add comprehensive coverage"

# Commit 5: Documentation
git add docs/
git commit -m "docs(auth): update API guide"
```

### Split Large Hunks

```bash
git add -p
# When you see a hunk that mixes concerns:
Stage this hunk [y,n,s,e,?,q]? s
# Git splits it into smaller pieces
```

## Resources

- **Read first**: [QUICK-REFERENCE.md](./QUICK-REFERENCE.md) — One-page cheat sheet
- **Deep dive**: [SKILL.md](./SKILL.md) — Complete workflow guide with scenarios
- **Setup help**: [SETUP.md](./SETUP.md) — Configuration and troubleshooting
- **Template**: [assets/commit-template.txt](./assets/commit-template.txt) — Commit message template

## Examples

### ❌ Before (Messy history)

```
* a1b2c3d WIP
* d4e5f6g fixed stuff
* h8i9j0k added things
```

Reviewer can't understand what changed or why. Hard to debug.

### ✅ After (Clean history)

```
* fix(posts): validate title before save
* feat(posts): add markdown editor support
* refactor(service): extract validation logic
* test(service): add markdown parser tests
* docs(api): update POST endpoint documentation
```

Each commit is focused and reviewable. Easy to find when a bug was introduced.

## Team Adoption

To encourage team-wide adoption:

1. **Copy template locally**: `git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt`
2. **Share QUICK-REFERENCE.md** in team Slack or docs
3. **Model it in PRs** — reviewers appreciate focused commits
4. **Celebrate clean history** — mention it in code reviews

## Troubleshooting

**Q: I staged the wrong hunk?**
```bash
git reset
git add -p  # Stage again, carefully
```

**Q: I committed but forgot a file?**
```bash
git add <forgotten-file>
git commit --amend --no-edit
```

**Q: I mixed unrelated changes in one commit?**
```bash
git reset --soft HEAD~1        # Undo, keep changes
git add -p                     # Stage first group
git commit -m "feat: ..."      # Commit
git add -p                     # Stage next group
git commit -m "fix: ..."       # Commit
```

For more troubleshooting: See [SETUP.md](./SETUP.md)

## See Also

- [Git Expert Agent](.github/agents/git-expert.agent.md) — Ask for workflow design, merge strategies, conflict resolution
- [Conventional Commits](https://www.conventionalcommits.org/) — Standard commit format
- [Git Interactive Staging Docs](https://git-scm.com/book/en/v2/Git-Tools-Interactive-Staging)

---

**Skill Version**: 1.0.0  
**Location**: `.github/skills/logical-commit-grouping/`  
**Created**: 2026-05-20  
**Maintained by**: Team
