# Migrations History — وصال (Wessal)

Wessal uses **EF Core Code-First Migrations**. The schema evolved over 7 migrations tracking the full development lifecycle.

## Running Migrations

```bash
# Apply all pending migrations
cd VolunteerBridge
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update 20260430154446_init

# Create a new migration (after model changes)
dotnet ef migrations add MigrationName

# Drop the database (dev only)
dotnet ef database drop
```

---

## Migration History

| # | Migration Name | Date | Changes |
|---|---------------|------|---------|
| 1 | `init` | 2026-04-30 | Initial schema: Users, ServiceRequests, Acceptances, Ratings, PointTransactions |
| 2 | `AddEmailConfirmation` | 2026-05-02 | Added `IsEmailConfirmed` + `EmailConfirmationToken` to Users |
| 3 | `AddImagePathToServiceRequest` | 2026-05-08 | Added `ImagePath` to ServiceRequests |
| 4 | `AddBanToUser` | 2026-05-10 | Added `IsBanned` + `BanReason` to Users |
| 5 | `AddRequestRemoval` | 2026-05-10 | Added `IsRemovedByAdmin` + `AdminRemovalReason` + `RemovalAcknowledged` to ServiceRequests |
| 6 | `AddBioSkillsExperience` | 2026-05-10 | Added `Bio`, `Skills`, `Experience` to Users |
| 7 | `AddChatMessages` | 2026-05-13 | Added `ChatMessages` table with sender/receiver/IsRead/SentAt |

---

## Schema Evolution Timeline

```
Init (Apr 30) ──► EmailConfirmation (May 2) ──► ImagePath (May 8)
                                                     │
                                                 BanToUser (May 10)
                                                     │
                                                 RequestRemoval (May 10)
                                                     │
                                                 BioSkillsExp (May 10)
                                                     │
                                                 ChatMessages (May 13)
```

---

## Notes

- The `AddBanToUser`, `AddRequestRemoval`, and `AddBioSkillsExperience` migrations all occurred on the same day (May 10), suggesting a rapid development sprint.
- The `ChatMessages` table was the last major addition, added two days before the `Beta-Final` branch was cut.
- All migrations are idempotent and can be reapplied safely.
