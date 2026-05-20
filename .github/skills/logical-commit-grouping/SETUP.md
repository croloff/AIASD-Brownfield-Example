# Setup Guide: Logical Commit Grouping

## Quick Setup

### 1. Configure Git to Use Commit Template

Apply the commit template for this project:

```bash
git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt
```

Verify:

```bash
git config commit.template
# Output: .github/skills/logical-commit-grouping/assets/commit-template.txt
```

### 2. Enable Interactive Staging Helpers

Optionally configure helpful Git aliases:

```bash
# Alias: 'git add-patch' → interactive staging
git config alias.add-patch "add -p"

# Alias: 'git staged' → see staged changes
git config alias.staged "diff --cached"

# Alias: 'git unstaged' → see unstaged changes
git config alias.unstaged "diff"

# Alias: 'git commits' → see local commits
git config alias.commits "log --oneline origin/main..HEAD"
```

Use them:

```bash
git add-patch        # Same as git add -p
git staged           # Same as git diff --cached
git unstaged         # Same as git diff
git commits          # See your commits ahead of main
```

### 3. Configure Your Editor for Commit Messages

Ensure your Git editor is set correctly:

```bash
# Set VS Code as default editor (cross-platform)
git config --global core.editor "code --wait"

# Verify
git config core.editor
```

---

## Team Setup (Project-Wide)

To enforce this across the team, commit the template and setup instructions:

```bash
# Already committed at:
# .github/skills/logical-commit-grouping/assets/commit-template.txt

# Provide teammates with setup script or instructions
```

Teammates should run:

```bash
git config commit.template .github/skills/logical-commit-grouping/assets/commit-template.txt
```

---

## Workflow: Your First Logical Commit

### Step 1: Check Your Changes

```bash
git status                    # See all changes
git diff --stat              # See file counts
```

### Step 2: Stage Interactively

```bash
git add -p                   # Review each hunk
# Answer y/n/s/e for each hunk
```

### Step 3: Review Staged Changes

```bash
git diff --cached            # See what you're committing
```

### Step 4: Commit

```bash
git commit                   # Opens editor with template
# Follow the template, save and close
```

Your commit message will include the template guide for reference.

### Step 5: Repeat for Next Group

```bash
git add -p                   # Stage next logical group
git diff --cached            # Review
git commit                   # Write message
```

---

## Common Commands Cheat Sheet

```bash
# Check your current changes
git status
git diff --stat

# Stage interactively (one hunk at a time)
git add -p

# See staged vs unstaged
git diff --cached            # Staged
git diff                     # Unstaged

# Commit with message
git commit -m "feat(scope): subject"

# Or use editor (will show template)
git commit

# View your commits
git log --oneline

# View details of a commit
git show <hash>

# Before pushing, see your local commits
git log origin/main..HEAD

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Amend last commit
git commit --amend

# Backup before risky operations
git branch backup-<name>
```

---

## Troubleshooting

### Problem: "I staged the wrong hunk"

```bash
# Unstage everything and start over
git reset
git add -p  # Stage again, more carefully
```

### Problem: "I committed, but forgot a file"

```bash
# Amend the previous commit
git add <forgotten-file>
git commit --amend --no-edit
```

### Problem: "I need to split a commit"

```bash
# Undo the commit, keep changes
git reset --soft HEAD~1

# Stage first group
git add -p
git commit -m "feat(auth): part 1"

# Stage next group
git add -p
git commit -m "feat(auth): part 2"
```

### Problem: "I mixed fixes and features in one commit"

```bash
# Create a backup
git branch backup-main

# Undo the commit, keep changes
git reset --soft HEAD~1

# Stage fixes first
git add -p  # Choose only fix hunks
git commit -m "fix(posts): validate title"

# Stage features
git add -p  # Choose only feature hunks
git commit -m "feat(posts): add markdown"

# If you messed up, restore from backup
git reset --hard backup-main
```

---

## Next Steps

1. **Apply the template**: Run the setup commands above
2. **Try interactive staging**: `git add -p` on your next changes
3. **Write conventional commits**: Follow the template for commit messages
4. **Review before pushing**: `git log origin/main..HEAD` to see your commits
5. **Keep commits focused**: Each commit should address one concern

---

## Further Reading

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Interactive Staging (git add -p)](https://git-scm.com/book/en/v2/Git-Tools-Interactive-Staging)
- [Git Rebasing](https://git-scm.com/book/en/v2/Git-Branching-Rebasing)
