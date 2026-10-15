# LinkForge — Day 1 Requirement Gathering

## Status

**Day 1: COMPLETED ✅**

We completed all **30 requirement-gathering questions**.

LinkForge is an anonymous, public URL-shortening MVP. Advanced production-scale technologies will be introduced only when the project reaches the appropriate stage and an actual problem justifies them.

---

# 1. Product Goal

LinkForge is a URL shortening platform focused initially on:

- Creating short URLs
- Redirecting short URLs to original URLs
- Optional custom aliases
- 7-day link expiration
- Basic public statistics

The initial version intentionally avoids unnecessary complexity.

---

# 2. User Requirements

## 2.1 Anonymous Users

Anyone can create a short URL.

No registration or login is required.

Anonymous users can:

- Create a short URL
- Access a valid short URL

Anonymous users cannot:

- Manage links
- Edit links
- Delete links
- Disable links
- Claim ownership
- Manage personal analytics

---

# 3. Link Ownership

There is **no link ownership** in the initial version.

After creation:

```text
Create URL
    ↓
Short URL
    ↓
No management
```

The creator cannot:

- Edit the destination
- Delete the link
- Disable the link
- Manage the link

---

# 4. Link Visibility

All short URLs are public.

Anyone who knows the short URL can access it.

There will initially be no:

- Private links
- Password-protected links
- User-specific access control

---

# 5. URL Creation

A user provides an original URL.

Example:

```text
https://example.com/products/123
```

LinkForge generates a short URL:

```text
https://linkforge.com/aZ91k7P
```

The creation response should contain:

- Success message
- Short URL

The UI can allow the user to open the short URL in a new tab.

---

# 6. URL Validation

The submitted URL must have a valid HTTP(S) URL structure.

Example:

```text
https://example.com
```

Invalid examples include:

```text
example.com
hello
```

Input URL validation is required before processing.

---

# 7. Maximum URL Length

Maximum original URL length:

```text
2,000 characters
```

URLs exceeding the limit must be rejected.

---

# 8. Duplicate Original URLs

The same original URL should return the same short URL.

Example:

```text
User A
https://example.com
        ↓
/abc1234

User B
https://example.com
        ↓
/abc1234
```

This means LinkForge requires original-URL deduplication.

---

# 9. Link Expiration

Every link expires after:

```text
7 days
```

Example:

```text
Created: 7 Oct 2026
Expires: 14 Oct 2026
```

After expiration, the link is no longer active.

---

# 10. Expired Links — Soft Delete

Expired links will use **soft deletion** rather than immediate physical deletion.

Conceptually:

```text
Active
  ↓
7 days
  ↓
Expired / Soft Deleted
```

The database record can remain available with an inactive/deleted state.

If the same original URL is requested again, the existing soft-deleted record can be reactivated.

```text
Old link
  ↓
Expired
  ↓
Soft deleted
  ↓
Same URL requested again
  ↓
Reactivate
  ↓
New 7-day expiration period
```

Because there are no user accounts, reactivation is based on the original URL rather than a specific user identity.

The exact database representation will be decided during database design.

---

# 11. Redirect Behavior

A valid short URL should redirect immediately to the original URL.

```text
GET /aZ91k7P
      ↓
Find active link
      ↓
Original URL
      ↓
Redirect
```

The browser/frontend can open the short URL in a new tab.

Opening a new tab is a browser/client behavior; the API itself returns the HTTP redirect.

---

# 12. Redirect Status Code

The initial redirect behavior will use:

```text
HTTP 302 Found
```

Reason:

- LinkForge controls the redirect.
- Links have a limited lifetime.
- The mapping is not intended to be treated as permanently fixed.
- The application should retain control of redirect behavior.

---

# 13. Invalid Short Code

If the requested short code does not exist:

```text
GET /doesNotExist
```

return:

```text
HTTP 404 Not Found
```

---

# 14. Expired Short Code

Expired/soft-deleted links must not redirect to the original destination.

They should behave as unavailable/not found from the redirect perspective.

---

# 15. Custom Aliases

Users can optionally choose a custom alias.

Example:

```text
https://linkforge.com/my-resume
```

instead of:

```text
https://linkforge.com/aZ91k7P
```

---

# 16. Custom Alias Uniqueness

A custom alias must be unique.

If an alias is already taken:

```text
my-resume
```

the system must not silently change it.

The user gets two choices:

1. Choose another alias.
2. Generate a random short code.

Example:

```text
❌ "my-resume" is already taken.

Choose another alias:
[ my-resume-2026 ] [Use Alias]

OR

[ Generate Random Short Code ]
```

---

# 17. Random Short Code

Random short codes use:

**Base62**

Character set:

```text
a-z
A-Z
0-9
```

---

# 18. Random Short Code Length

Random codes contain:

```text
7 characters
```

Examples:

```text
aZ91k7P
7Hd2PqX
x8Kq19A
```

The theoretical number of 7-character Base62 combinations is:

```text
62^7 = 3,521,614,606,208
```

---

# 19. Case Sensitivity

Short codes are **case-sensitive**.

Therefore:

```text
/aZ91k7P
```

and:

```text
/az91k7p
```

are different short codes.

---

# 20. Basic Public Statistics

The UI will show basic system-level statistics publicly.

The purpose is partly to make the LinkForge landing page attractive.

Statistics:

- Total URLs created
- Total active URLs
- Total clicks

Example:

```text
        LinkForge

  12,458       8,932       245,781
URLs Created  Active URLs  Total Clicks
```

These statistics are visible to everyone.

---

# 21. Analytics Scope

Only basic analytics are required initially.

Track:

- Total URLs created
- Total active URLs
- Total clicks

Do **not** initially track:

- IP address
- Browser
- Device
- Operating system
- Country
- Referrer
- Geographic information

Detailed analytics can be introduced later.

---

# 22. Link Management

There is no management functionality in the initial version.

Not supported initially:

```text
Edit
Delete manually
Disable
Ownership
User dashboard
```

The link lifecycle is controlled by the system's expiration behavior.

---

# 23. Availability

A production availability target is **deferred**.

We will decide this later when the project is ready for production architecture.

No premature high-availability architecture will be introduced.

---

# 24. Performance / Latency

Initial performance target:

```text
< 500 ms
```

However, we will not prematurely optimize.

The intended progression is:

```text
Build
  ↓
Measure
  ↓
Identify bottleneck
  ↓
Optimize
  ↓
Measure again
```

Latency should improve progressively as the architecture evolves.

---

# 25. Scalability

Scalability decisions are intentionally **deferred**.

We will first build the simple system.

Later, based on actual requirements and bottlenecks, we may consider:

```text
Redis
Read Replicas
Message Queues
Microservices
Sharding
Horizontal Scaling
```

We will not introduce these technologies merely to make the project look advanced.

---

# 26. Security

Basic security requirements apply from the beginning.

Required:

- HTTPS
- Input URL validation
- URL structure validation

Advanced security decisions are deferred.

Potential future topics include:

- Authentication
- Authorization
- Rate limiting
- Secrets management
- Security headers
- Other production security mechanisms

---

# 27. Reliability

Initial reliability behavior:

```text
Dependency failure
      ↓
Appropriate error response
```

For example, if SQL Server is temporarily unavailable, the API should return an appropriate error rather than pretending the operation succeeded.

Advanced resilience is deferred.

Later we can introduce:

- Retry
- Timeout
- Exponential backoff
- Jitter
- Circuit breaker
- Fallback
- Bulkhead

---

# 28. Consistency

The system requires **strong consistency for URL deduplication**.

Requirement:

> The same original URL must consistently map to the same active short URL.

For concurrent requests:

```text
Request A ──┐
            ├──> https://example.com
Request B ──┘
                  ↓
               /abc1234
```

We do not want two different active short URLs to be created for the same original URL.

This will later require careful consideration of:

- Unique constraints
- Transactions
- Concurrency
- Database behavior

---

# 29. Durability

Created links must persist in SQL Server.

An application/server restart must not lose valid links.

```text
Create link
    ↓
SQL Server
    ↓
API restart
    ↓
Link still works
```

SQL Server is the durable source of truth for the initial system.

---

# 30. Capacity Estimation

Detailed capacity estimation is **deferred**.

We will first build the working system.

We will then introduce capacity/scaling decisions when the project reaches the appropriate system-design stage.

We will not prematurely choose:

- Redis
- Read replicas
- Sharding
- Microservices
- Message queues

without a requirement or bottleneck that justifies them.

---

# 31. Final Functional Requirements Summary

| # | Requirement | Decision |
|---|---|---|
| 1 | User model | Anonymous |
| 2 | URL creation | Required |
| 3 | Login | Not required initially |
| 4 | Link ownership | None |
| 5 | Link visibility | Public |
| 6 | URL validation | Required |
| 7 | Maximum URL length | 2,000 characters |
| 8 | Duplicate original URL | Same short URL |
| 9 | Expiration | 7 days |
| 10 | Expiration handling | Soft delete |
| 11 | Reuse after expiration | Reactivate when same URL is requested |
| 12 | Redirect | Immediate |
| 13 | Redirect status | HTTP 302 |
| 14 | Invalid short code | HTTP 404 |
| 15 | Custom alias | Supported |
| 16 | Alias uniqueness | Required |
| 17 | Alias collision | Choose another or generate random |
| 18 | Random code | Base62 |
| 19 | Random code length | 7 |
| 20 | Case sensitivity | Case-sensitive |
| 21 | Public statistics | Required |
| 22 | Basic analytics | Required |
| 23 | Detailed analytics | Later |
| 24 | Link management | Not required |

---

# 32. Final Non-Functional Requirements Summary

| Requirement | Decision |
|---|---|
| Availability target | Decide later |
| Latency | Initial target <500 ms |
| Optimization | Progressive, measurement-driven |
| Scalability | Decide later |
| HTTPS | Required |
| Input validation | Required |
| Advanced security | Later |
| Basic error handling | Required |
| Advanced resilience | Later |
| Consistency | Strong for URL deduplication |
| Durability | SQL Server persistence |
| Capacity estimation | Later |

---

# 33. Current Link Lifecycle

```text
                User
                 ↓
          Submit Original URL
                 ↓
             Validate
                 ↓
       Existing URL available?
          ┌──────┴──────┐
         Yes            No
          ↓              ↓
   Return existing    Generate
     short URL        short code
                         ↓
                  Save to SQL Server
                         ↓
                   Active for 7 days
                         ↓
                    Expiration
                         ↓
                  Soft Deleted
                         ↓
               Same URL requested?
                    ┌────┴────┐
                   Yes        No
                    ↓          ↓
                Reactivate   Remains
                             inactive
```

---

# 34. Current Redirect Flow

```text
Client
  ↓
GET /{shortCode}
  ↓
SQL Server
  ↓
Find active link
  ↓
Found?
 ┌──────┴──────┐
Yes            No
 ↓              ↓
302             404
 ↓
Original URL
```

---

# 35. Current Architecture Direction

Initial system:

```text
Client / Browser
       ↓
ASP.NET Core Web API
       ↓
SQL Server
```

We are intentionally **not** starting with:

```text
Redis
Kafka
RabbitMQ
Microservices
Read Replicas
Sharding
Kubernetes
```

Those technologies will be introduced only when their corresponding problem is reached.

---

# 36. System Design Principle Learned

The most important principle from Day 1 is:

> **Requirements should drive architecture.**

Example progression:

```text
Need simple URL shortening
        ↓
ASP.NET Core + SQL Server

Need faster reads
        ↓
Investigate indexing/caching

Database reads become bottleneck
        ↓
Consider Redis

Read traffic becomes very large
        ↓
Consider read replicas

Data capacity becomes a bottleneck
        ↓
Consider sharding
```

We should not reverse this process:

```text
"I know Redis, therefore let's add Redis."
```

---

# 37. Day 1 Completion

```text
Requirement Questions
        ↓
30 / 30 ✅

Functional Requirements
        ↓
Completed ✅

Non-Functional Requirements
        ↓
Completed for current MVP scope ✅

Capacity / Scaling
        ↓
Intentionally deferred ⏸️

Day 1
        ↓
COMPLETED ✅
```

**Progress: 1 / 15 days completed.**

---

# Day 2 Preview

The next stage is:

## Requirement Review & Finalization

We will review these decisions for:

- Contradictions
- Ambiguous behavior
- Missing requirements
- Edge cases
- Business-rule consistency

Then we will finalize the **LinkForge SRS** before moving toward system design and implementation.
