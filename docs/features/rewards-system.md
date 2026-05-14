# Rewards System — Feature Documentation

## Overview

Wessal uses a **gamified points and leveling system** to incentivize volunteer participation. Points are awarded when a requester marks a task as complete, tracked in a `PointTransactions` audit table, and reflected in a public leaderboard.

---

## Points Earning

| Trigger | Formula | Minimum |
|---------|---------|---------|
| Task completed | `EstimatedHours × 20` | 10 points |

**Examples:**
- 30-minute task → 10 points (minimum floor)
- 1-hour task → 20 points
- 2-hour task → 40 points
- 5-hour task → 100 points
- 10-hour task → 200 points

Points are **additive only** — no deductions currently exist.

---

## User Levels

Four levels based on cumulative total points:

| Level | Arabic | English | Points Required |
|-------|--------|---------|----------------|
| 0 | مبتدئ الخير | Newcomer | 0 – 99 |
| 1 | صانع الفرق | Helper | 100 – 299 |
| 2 | موثوق العطاء | Trusted | 300 – 699 |
| 3 | بطل المجتمع | Champion | 700+ |

Level is recalculated on every task completion:

```csharp
volunteer.Level = volunteer.TotalPoints switch
{
    < 100 => UserLevel.Newcomer,
    < 300 => UserLevel.Helper,
    < 700 => UserLevel.Trusted,
    _     => UserLevel.Champion
};
```

---

## Points Audit Trail

Every point award creates a `PointTransaction` record:

```csharp
_db.pointTransactions.Add(new PointTransaction
{
    UserId = volunteer.UserId,
    Points = points,
    Reason = $"اكتمل: {acceptance.Request.Title}",
    AcceptanceId = acceptanceId,
    User = volunteer
});
```

The `Reason` field stores the Arabic text "اكتمل: [Title]" — this is the task completion reason. The `AcceptanceId` links the transaction to the specific task (SET NULL on acceptance deletion to preserve the history).

---

## Leaderboard

**Route:** `/Leaderboard` — `LeaderboardController`

Displays all non-banned users ranked by `TotalPoints` descending. The current user's row is highlighted.

---

## Statistics — Top Volunteers This Month

The `StatisticsController` shows **top 5 volunteers by monthly points**:

```csharp
_db.pointTransactions
    .Where(pt => pt.CreatedAt >= startOfMonth)
    .GroupBy(pt => pt.UserId)
    .Select(g => new { UserId = g.Key, MonthlyPoints = g.Sum(x => x.Points) })
    .OrderByDescending(x => x.MonthlyPoints)
    .Take(5)
```

This correctly computes **monthly** points from the transaction log (not from the user's cumulative total), allowing for fair monthly comparisons.

---

## Known Limitations

| Limitation | Impact |
|-----------|--------|
| Points are never deducted | No penalty for poor behavior |
| Level badge not updated in real-time | Requires page reload to see new level |
| No notification when level changes | User may miss the upgrade |
| `CompletedTasksCount` tracked separately from actual count | Can become inconsistent if data is manually modified |
