---
name: commit-logical-groups
description: Organize and commit staged changes in logical groups with clear commit messages
temperature: 0.2

arguments:
  staged_changes:
    type: string
    description: Output from 'git diff --cached --name-status' showing staged files
  change_scope:
    type: enum
    values: ["feature", "fix", "refactor", "docs", "chore", "perf", "test", "mixed"]
    description: Type of changes or 'mixed' for auto-detection

tags: ["git", "workflow", "commits", "organization"]

ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "github-copilot"
chat_id: "commit-logical-groups-creation"
prompt: |
  Create a prompt file that helps developers commit changes in logical groups.
  The file should analyze staged changes, suggest logical groupings, and provide
  commit messages for each group. It should help organize work before committing.
started: "2026-05-19T00:00:00Z"
ended: "2026-05-19T00:05:00Z"
task_durations:
  - task: "promptfile creation"
    duration: "00:05:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2026/05/19/commit-logical-groups-creation/conversation.md"
source: "github-copilot"
owner: "Development Team"
version: "1.0"
---

# Commit Changes in Logical Groups

This prompt helps you organize staged Git changes into logical commits for a cleaner history.

## Workflow

1. **Stage your changes** (you may have already done this):
   ```powershell
   git add .
   ```

2. **Get staged file list**:
   ```powershell
   git diff --cached --name-status
   ```

3. **Paste the output** and the change scope (or "mixed") into this prompt.

4. **Review the suggested groups** and commit messages.

5. **Stage and commit per group**:
   ```powershell
   git reset HEAD
   git add <files-for-group-1>
   git commit -m "<suggested-message-1>"
   git add <files-for-group-2>
   git commit -m "<suggested-message-2>"
   ```

## Analysis

Analyze the staged changes: {{staged_changes}}

Scope: {{change_scope}}

## Task

1. **Identify Logical Groups**
   - Group files by feature, responsibility, or architectural layer
   - Separate concerns: keep refactors, fixes, and features in different commits
   - Avoid mixing test changes with implementation in the same commit if possible

2. **For Each Group, Provide**:
   - Group name/purpose
   - List of files
   - Suggested commit message (conventional commit format)

3. **Conventional Commit Format**:
   ```
   <type>(<scope>): <subject>
   
   <body (if needed)>
   ```
   
   Types: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`, `perf`

4. **Examples of Good Groups**:
   - **Feature**: All files implementing a new feature → `feat(posts): add post tags`
   - **Bug Fix**: All files fixing one issue → `fix(auth): prevent jwt token expiration race`
   - **Tests**: Test file changes → `test(user-service): add edge case coverage`
   - **Refactor**: Code improvements, no behavior change → `refactor(data): extract mapper logic`
   - **Documentation**: Docs and README → `docs: update api endpoint reference`

5. **Output Format**:
   ```
   ## Group 1: [Group Name]
   **Files**: 
   - path/to/file1.cs
   - path/to/file2.cs
   
   **Commit Message**: `type(scope): description`
   
   ## Group 2: [Group Name]
   ...
   ```

## Tips

- If a file belongs in multiple logical groups, stage and commit it individually
- Small, focused commits are easier to review and revert if needed
- Group related changes even if they touch multiple files
- Keep fixes and features separate for easier rollback
