---
ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "github-copilot"
chat_id: "create-logical-commit-skill-20260520"
prompt: |
  Create a comprehensive skill for committing changes in logical groups with interactive staging guidance and conventional commit support.
started: "2026-05-20T00:00:00Z"
ended: "2026-05-20T00:20:00Z"
task_durations:
  - task: "workflow design"
    duration: "00:05:00"
  - task: "skill authoring"
    duration: "00:12:00"
  - task: "asset creation"
    duration: "00:03:00"
total_duration: "00:20:00"
ai_log: "ai-logs/2026/05/20/create-logical-commit-skill-20260520/conversation.md"
source: "copilot-chat"
---

name: logical-commit-grouping
description: "Skill: Organize and commit changes in logical groups. Use when: preparing PRs with clean history, separating mixed concerns into focused commits, using interactive staging (git add -p), following conventional commits, or preparing a codebase for review."
created: "2026-05-20"
version: "1.0.0"
location: ".github/skills/logical-commit-grouping/"
scope: "workspace"
keywords:
  - git
  - commits
  - staging
  - interactive staging
  - conventional commits
  - PR preparation
  - code organization
author: "github-copilot"
---

# Logical Commit Grouping Skill

**Purpose**: Guide you through organizing and committing changes in focused, logical groups using interactive staging and conventional commit practices.

**When to Use This Skill**:
- You have mixed, unrelated changes that should be separate commits
- You want to clean up your commit history before opening a PR
- You need to organize commits following conventional commit format
- You're preparing code for review and want focused, reviewable commits
- You have WIP (work-in-progress) changes mixed with completed work

---

## Quick Start

1. **Analyze Your Changes**
   ```bash
   git status
   git diff --stat
   ```

2. **Identify Logical Groups**
   - Bug fixes (related to one issue)
   - Features (new functionality for one epic)
   - Refactoring (isolated code improvements)
   - Documentation (docs-only changes)
   - Tests (test-only changes)
   - Configuration (config/infrastructure changes)

3. **Stage Interactively**
   ```bash
   git add -p
   ```
   Review hunks one by one, selecting which to stage for this commit.

4. **Commit with Convention**
   ```bash
   git commit -m "feat(auth): add JWT token refresh logic"
   ```

5. **Repeat** for each logical group until all changes are committed.

---

## Workflow: Step-by-Step

### Step 1: Assess Your Changes

Before staging, understand what you're working with:

```bash
# See unstaged changes summary
git status

# See all changed files with stats
git diff --stat

# See changed files grouped by type
git diff --name-status

# See specific change details
git diff path/to/file.cs
```

**Analysis Task**:
- How many files changed?
- Are changes scattered across unrelated modules?
- Can you group them by feature/bug/refactor?

### Step 2: Define Logical Groups

Organize changes into commits. Each commit should:
- **Be focused**: Address one concern (one feature, one bug fix, one refactor)
- **Be self-contained**: Should not depend on subsequent commits
- **Be reviewable**: A reviewer can understand the change without other commits
- **Have a clear message**: Explain the "why" and "what"

**Example Groupings**:

| Commit 1 | Commit 2 | Commit 3 |
|----------|----------|----------|
| **feat(auth)**: Add JWT refresh | **refactor(db)**: Simplify queries | **docs**: Update API readme |
| src/JwtService.cs | src/UserContext.cs | docs/api.md |
| src/Controllers/AuthController.cs | src/Repository/UserRepository.cs | |
| tests/JwtServiceTest.cs | tests/RepositoryTest.cs | |

### Step 3: Stage Interactively (git add -p)

This is the **key technique** for logical grouping:

```bash
git add -p
```

You'll see hunks of changes one by one. For each hunk, respond:
- `y` — Stage this hunk
- `n` — Skip this hunk (don't stage yet)
- `s` — Split hunk (if too large; break into smaller pieces)
- `e` — Edit hunk manually (advanced: hand-pick lines)
- `q` — Quit (stop reviewing hunks)

**Example Interactive Session**:

```
Stage this hunk [y,n,s,e,?,q]? y
Stage this hunk [y,n,s,e,?,q]? n
Stage this hunk [y,n,s,e,?,q]? s
Split into 2 hunks.
Stage this hunk [y,n,s,e,?,q]? y
Stage this hunk [y,n,s,e,?,q]? n
```

**Tips**:
- Use `s` (split) to break large hunks that mix concerns
- Use `e` (edit) only for advanced users—dangerous if you break syntax
- If a hunk is too mixed, you can skip it and `git add path/to/file` the whole file if it's related to one commit

### Step 4: Review Staged Changes

Before committing, verify what's staged:

```bash
# See what's staged
git diff --cached

# See staged files
git diff --cached --name-status

# See summary
git diff --cached --stat
```

**Sanity Check**:
- Are all staged files related to one concern?
- Is anything staged that shouldn't be?
- Did you accidentally include unrelated changes?

### Step 5: Commit with Convention

Write a clear, conventional commit message:

```
<type>(<scope>): <subject>

<body (optional)>

<footer (optional)>
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code restructuring (no behavior change)
- `docs`: Documentation only
- `style`: Code style (formatting, no logic change)
- `test`: Test changes only
- `perf`: Performance improvement
- `chore`: Build, dependency, tooling, config

**Examples**:

```bash
# Feature with scope
git commit -m "feat(auth): add JWT token refresh mechanism"

# Bug fix with body explaining the issue
git commit -m "fix(post-controller): handle null title validation

Previously, the EditPost endpoint didn't validate empty titles,
allowing posts with null or whitespace-only titles to be saved.
This caused UI errors when rendering posts.

Now validates that title is non-empty before save."

# Refactoring with impact note
git commit -m "refactor(user-service): extract password hashing to utility

Extracted bcrypt logic into PasswordHasher utility for reuse.
No behavior change; existing tests pass."

# Multiple scopes in complex change
git commit -m "docs: update README with JWT setup instructions"
```

### Step 6: Repeat for Next Group

Once your first commit is done, unstaged changes remain:

```bash
# Check what's left
git status

# Stage the next logical group
git add -p

# Commit again
git commit -m "..."
```

**Continue** until all changes are organized into focused commits.

---

## Scenarios & Solutions

### Scenario 1: Separating Mixed Changes

**Problem**: You have both bug fixes and new features in uncommitted changes.

**Solution**:

1. Run `git status` to see all changed files
2. Identify which files belong to which feature/bug
3. Use `git add -p` to stage by hunk, selecting only the bug-fix hunks for the first commit
4. Commit the bug fix: `git commit -m "fix(posts): validate post title"`
5. Use `git add -p` again for feature changes
6. Commit the feature: `git commit -m "feat(posts): add markdown support"`

```bash
# Example workflow
git add -p          # Pick hunks for bug fix
git commit -m "fix(posts): validate post title"

git add -p          # Pick hunks for new feature
git commit -m "feat(posts): add markdown support"
```

### Scenario 2: Consolidating Related Changes

**Problem**: Multiple files changed for one feature, but you staged them separately.

**Solution**:

1. Use `git reset` to unstage everything
2. Use `git add <file1> <file2> <file3>` to stage all related files
3. Commit once with a comprehensive message

```bash
git reset                          # Unstage everything
git add src/Service.cs src/Tests/ServiceTest.cs
git commit -m "feat(service): implement new service layer"
```

### Scenario 3: Filtering Out WIP Changes

**Problem**: Some changes are incomplete and shouldn't be committed yet.

**Solution**:

1. Use `git add -p` to exclude WIP hunks (answer `n` to skip them)
2. Commit only the finished changes
3. Incomplete changes remain in working directory for future commits

```bash
git add -p
# Answer 'n' to skip WIP hunks
git commit -m "feat(auth): add login endpoint"

# WIP changes remain unstaged for later
git status  # Shows remaining unstaged changes
```

### Scenario 4: Conventional Commit Organization

**Problem**: You have a mix of features, fixes, and refactoring that should follow conventional commits.

**Solution**:

1. Stage and commit in order by type:
   - `fix`: Bug fixes first
   - `feat`: New features
   - `refactor`: Code improvements
   - `docs`: Documentation
   - `test`: Test changes
   - `chore`: Config/tooling

```bash
# Commit 1: Bug fix
git add -p          # Select fix hunks
git commit -m "fix(auth): prevent expired tokens from validating"

# Commit 2: Feature
git add -p          # Select feature hunks
git commit -m "feat(auth): add token refresh endpoint"

# Commit 3: Refactor
git add -p          # Select refactor hunks
git commit -m "refactor(auth): extract token validation to utility"

# Commit 4: Tests
git add tests/
git commit -m "test(auth): add comprehensive token refresh tests"

# Commit 5: Docs
git add docs/
git commit -m "docs(auth): update API documentation for token refresh"
```

### Scenario 5: PR Preparation

**Problem**: You're preparing a branch for a pull request and want clean, reviewable commits.

**Solution**:

1. Identify the commits you've already made: `git log --oneline origin/main..HEAD`
2. Review each commit's changes: `git show <commit-hash>`
3. If commits are out of order or should be combined, rebase: `git rebase -i origin/main`
4. If new changes were made, organize using interactive staging
5. Verify final history: `git log --oneline origin/main..HEAD`

```bash
# See commits in your PR
git log --oneline origin/main..HEAD

# Verify each commit is clean and focused
git log -p origin/main..HEAD

# If messy, rebase to reorganize
git rebase -i origin/main
# Reorder, squash, or edit commits as needed

# Verify final state
git log --oneline origin/main..HEAD
```

---

## Advanced Techniques

### Technique 1: Edit Hunks Manually (git add -p + e)

For very granular control, you can edit a hunk:

```bash
git add -p
# When you see a hunk, answer 'e' to edit it
```

This opens an editor where you can:
- Delete lines starting with `-` to exclude them
- Delete lines starting with `+` to exclude new additions
- Keep lines you want to stage

**Warning**: Make sure resulting code is valid (correct syntax, no incomplete edits).

### Technique 2: Amend Previous Commits

If you missed changes for an earlier commit:

```bash
# Stage the forgotten changes
git add <file>

# Amend the previous commit
git commit --amend --no-edit

# Or, if you need to update the message
git commit --amend -m "feat(auth): updated message"
```

**Warning**: Don't amend commits already pushed to shared branches.

### Technique 3: Splitting a Commit

If you realize a commit mixes concerns:

```bash
# Reset to the parent commit
git reset HEAD~1

# Interactively stage the first concern
git add -p
git commit -m "feat(auth): part 1"

# Stage the next concern
git add -p
git commit -m "feat(auth): part 2"
```

### Technique 4: Reordering Commits (Interactive Rebase)

If commits are out of order before pushing:

```bash
# Rebase interactively
git rebase -i origin/main

# In the editor:
# - Reorder lines (cut and paste)
# - Save and quit
# - Git replays commits in new order
```

**Warning**: Don't reorder commits already pushed to shared branches.

---

## Commit Message Template

Use this template in `assets/commit-template.txt`:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Configure it**:

```bash
git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt
```

---

## Quick Reference

| Goal | Command |
|------|---------|
| **Stage interactively** | `git add -p` |
| **See staged changes** | `git diff --cached` |
| **See unstaged changes** | `git diff` |
| **Unstage everything** | `git reset` |
| **Unstage one file** | `git reset HEAD path/to/file` |
| **Commit staged changes** | `git commit -m "..."` |
| **Amend last commit** | `git commit --amend` |
| **View commit history** | `git log --oneline` |
| **View commit details** | `git show <hash>` |
| **Reorder commits** | `git rebase -i origin/main` |

---

## Safety Tips

1. **Always review before committing**: `git diff --cached` to see exactly what you're committing
2. **Commit frequently**: Smaller, focused commits are safer to rebase/amend
3. **Don't force push to shared branches**: If you've pushed a commit, don't rewrite it (rebase)
4. **Backup before rebasing**: `git branch backup-<branch-name>` before interactive rebase
5. **Test after staging**: Run tests after staging to ensure changes are valid
6. **Use feature branches**: Keep experimental grouping on feature branches, not main

---

## Summary

1. **Assess** changes with `git status` and `git diff`
2. **Group** related changes into logical commits
3. **Stage** each group interactively with `git add -p`
4. **Review** staged changes with `git diff --cached`
5. **Commit** with a clear conventional message
6. **Repeat** until all changes are organized

Clean, focused commits make code review faster, history clearer, and debugging easier with `git bisect`.

---

**Skill Version**: 1.0.0  
**Last Updated**: 2026-05-20  
**Status**: Active
