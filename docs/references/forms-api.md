# Microsoft Forms API Reference

> **Date:** 2026-05-02
> **Status:** Undocumented — this reference was compiled from network traffic analysis and community research
> **Source:** https://forms.office.com/formapi/api/

Microsoft Forms does not have an official public API. The endpoints below were reverse-engineered from the Forms web interface and community research (Jack Parker, Vasil Michev, and others). They are **unsupported** and may change without notice.

## Authentication

### App Registration

1. Register an Azure AD app
2. Add API permission: **Microsoft Forms** → **Application permissions** → `Forms.Read.All`
3. Grant admin consent
4. Create a client secret

### Token Acquisition

| Flow | Grant Type | Use Case |
|------|-----------|----------|
| Client Credentials | `client_credentials` | User-owned forms |
| ROPC | `password` | Group-owned forms (requires MFA-excluded account) |

**Scope:** `https://forms.office.com/.default`
**Token endpoint:** `https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token`

### Environment Variables

| Variable | Purpose |
|----------|---------|
| `MSAL_CLIENT_ID` | Azure AD app client ID |
| `MSAL_CLIENT_SECRET` | Client secret (client credentials flow) |
| `MSAL_USERNAME` | Service account UPN (ROPC flow) |
| `MSAL_PASSWORD` | Service account password (ROPC flow) |

## Base URL

```
https://forms.office.com/formapi/api/{tenantId}/{context}/{ownerId}/light/forms
```

- `{tenantId}` — GUID of the Microsoft 365 tenant
- `{context}` — `users` for user-owned forms, `groups` for group-owned forms
- `{ownerId}` — Entra ID object ID of the owner, or `me` for current user

## Endpoints

### List Forms

```
GET /{tenantId}/{context}/{ownerId}/light/forms
```

**Query parameters:**
| Parameter | Description |
|-----------|-------------|
| `$select` | Comma-separated field list |
| `$filter` | OData filter expression |
| `$top` | Page size |
| `$skip` | Offset |

**Example:**
```
GET /formapi/api/contoso.onmicrosoft.com/users/me/light/forms?$select=id,title,status,createdDate
```

**Response:**
```json
{
  "value": [
    {
      "id": "form-id-123",
      "title": "Customer Satisfaction Survey",
      "status": "Active",
      "createdDate": "2026-01-15T10:30:00Z",
      "modifiedDate": "2026-03-20T14:00:00Z",
      "ownerId": "user-id-456",
      "version": 3,
      "softDeleted": false,
      "type": "Survey"
    }
  ]
}
```

### Get Form Detail

```
GET /{tenantId}/{context}/{ownerId}/light/forms('{formId}')
```

**Query parameters:** `$select` — specify fields, e.g. `rowCount`

**Response** includes all form fields plus:
| Field | Type | Description |
|-------|------|-------------|
| `rowCount` | int | Total number of responses |

### Get Branching Rules

```
GET /{tenantId}/{context}/{ownerId}/light/forms('{formId}')/branching
```

Returns JSON with branching configuration: question-level conditions, section routing, and default paths.

### Get Analytics

```
GET /{tenantId}/{context}/{ownerId}/light/forms('{formId}')/analytics
```

Returns analytics data including:
- Submission count over time
- Completion rate
- Median duration
- Question-level dropoff rates

### List Responses

```
GET /{tenantId}/{context}/{ownerId}/light/forms('{formId}')/responses
```

**Query parameters:**
| Parameter | Description |
|-----------|-------------|
| `$top` | Page size (max 200) |
| `$skip` | Offset for pagination |
| `$filter` | OData filter (e.g. `id eq 42`) |
| `$expand` | Related entities (e.g. `comments`) |

**Response:**
```json
{
  "value": [
    {
      "id": 1,
      "submittedAt": "2026-01-15T10:30:00Z",
      "answers": "{\"q1\":\"Yes\",\"q2\":\"Some text\"}",
      "comments": []
    }
  ]
}
```

### Share Form

```
POST /{tenantId}/{context}/{ownerId}/light/forms('{formId}')/permissions
```

**Body:**
```json
{
  "groupId": "entra-group-id",
  "role": "respond"
}
```

Roles: `respond` (can submit), `collaborate` (can edit)

### Transfer Ownership

```
POST /{tenantId}/{context}/{ownerId}/light/forms('{formId}')/owner
```

**Body:**
```json
{
  "targetUserPrincipalName": "new-owner@contoso.com"
}
```

### Templates

List:
```
GET /{tenantId}/templates
```

Create:
```
POST /{tenantId}/templates
```
Body: `{ "formId": "...", "scope": "team" }` (scope: `org` or `team`)

Share:
```
POST /{tenantId}/templates('{templateId}')/shares
```
Body: `{ "groupId": "..." }`

## Pagination

The responses endpoint supports standard OData `$top`/`$skip` pagination. There is no `@odata.nextLink` — iterate by incrementing `$skip` until an empty `value` array is returned. Recommended page size: 100.

## Rate Limits

The Forms API enforces rate limiting. Observed limits:
- ~300 requests per 60 seconds per application
- 429 responses include a `Retry-After` header

## Known Limitations

1. **Group forms require ROPC** — Application permissions do not work for group-owned forms
2. **No `@odata.nextLink`** — Clients must implement manual skip-based pagination
3. **No create/update endpoint** — Full form creation/modification is not available via the undocumented API; PACX uses local JSON manifests for authoring
4. **No form close/reopen** — No discovered endpoint for closing/reopening responses
5. **Tenant ID must be a GUID** — The API rejects tokens issued with a domain name instead of a GUID

## Reliability Notes

This API has been observed to:
- Return 401 when tokens are issued with domain names instead of GUID tenant IDs
- Return empty `value` arrays for users with no forms (not a 404)
- Require the exact `https://forms.office.com/.default` scope (not a resource URL)
- Not support `User.Read.All` from the same token — separate Graph API auth needed for user enumeration
