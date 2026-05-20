---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-project-documentation-20260519"
prompt: |
  @codebase create documentation for this project
started: "2026-05-19T00:00:00Z"
ended: "2026-05-19T00:25:00Z"
task_durations:
  - task: "endpoint inventory"
    duration: "00:10:00"
  - task: "reference authoring"
    duration: "00:12:00"
  - task: "traceability updates"
    duration: "00:03:00"
total_duration: "00:25:00"
ai_log: "ai-logs/2026/05/19/create-project-documentation-20260519/conversation.md"
source: "github-copilot-chat"
---

# API Reference

Base URL: `https://localhost:<port>/api`

## Authentication

Protected endpoints require:

```http
Authorization: Bearer <jwt-token>
```

JWT token is issued by:

- `POST /api/User/Register`
- `POST /api/User/Login`

## User endpoints

### Register

- Method: `POST`
- Route: `/api/User/Register`
- Auth: No

Request body:

```json
{
  "email": "alice@example.com",
  "username": "alice",
  "password": "StrongPass123!",
  "confirmPassword": "StrongPass123!"
}
```

Successful response:

- `200 OK` with JWT token string

### Login

- Method: `POST`
- Route: `/api/User/Login`
- Auth: No

Request body:

```json
{
  "username": "alice",
  "password": "StrongPass123!"
}
```

Successful response:

- `200 OK` with JWT token string

## Post endpoints

### List posts

- Method: `GET`
- Route: `/api/Post`
- Auth: No

Successful response:

- `200 OK` with array of posts

### Get post by id

- Method: `GET`
- Route: `/api/Post/{id}`
- Auth: No

Responses:

- `200 OK` with post
- `404 Not Found` if post does not exist

### Create post

- Method: `POST`
- Route: `/api/Post`
- Auth: Yes

Request body:

```json
{
  "title": "My first post",
  "body": "Post content"
}
```

Responses:

- `201 Created` with created post id
- `400 Bad Request` on validation failure

### Edit post

- Method: `PUT`
- Route: `/api/Post/{id}`
- Auth: Yes

Request body:

```json
{
  "title": "Updated title",
  "body": "Updated body"
}
```

Responses:

- `200 OK` with updated post
- `400 Bad Request` on invalid input
- `404 Not Found` if post does not exist

### Delete post

- Method: `DELETE`
- Route: `/api/Post/{id}`
- Auth: Yes

Responses:

- `204 No Content`
- `404 Not Found` if post does not exist

## Comment endpoints

All comment endpoints require authentication.

### Get comment by id

- Method: `GET`
- Route: `/api/Comment/{id}`
- Auth: Yes

Responses:

- `200 OK` with comment
- `404 Not Found` if comment does not exist

### Create comment

- Method: `POST`
- Route: `/api/Comment/{postId}`
- Auth: Yes

Request body:

```json
{
  "body": "Great post!"
}
```

Responses:

- `201 Created` with created comment id
- `400 Bad Request` on validation failure
- `404 Not Found` if post does not exist

### Edit comment

- Method: `PUT`
- Route: `/api/Comment/{id}`
- Auth: Yes

Request body:

```json
{
  "body": "Edited comment"
}
```

Responses:

- `200 OK` with updated comment
- `400 Bad Request` on validation failure
- `404 Not Found` if comment does not exist

### Delete comment

- Method: `DELETE`
- Route: `/api/Comment/{id}`
- Auth: Yes

Responses:

- `204 No Content`
- `404 Not Found` if comment does not exist

## Data shape examples

Post response example:

```json
{
  "id": 1,
  "title": "My first post",
  "body": "Post content",
  "creationTime": "2026-05-19T00:00:00Z",
  "likes": 0,
  "comments": [
    {
      "id": 10,
      "body": "Great post!",
      "creationTime": "2026-05-19T00:05:00Z"
    }
  ]
}
```
