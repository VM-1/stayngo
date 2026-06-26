# 003. Anchoring the cancellation cut-off to the listing's timezone

## The rule

A guest may cancel a booking up to **24 hours before check-in**, where check-in is anchored to the
**listing's local timezone** (15:00 on the check-in date, in the property's IANA zone). After that
moment, cancellation is closed.

## Why the *listing's* timezone, not the guest's

The deadline is a property of the *property*, not of whoever is looking at it. If the cut-off were
computed in the guest's local time, the same booking would have a different deadline depending on
where the guest happens to be — and a guest could **change their device timezone to move the
deadline** and bypass the rule. Anchoring to the listing's zone makes the cut-off a single, fixed,
tamper-proof instant.

## The bug to avoid

Check-in is stored as a `DateOnly`, and the cut-off (15:00 the day before) is a **wall-clock time in
the listing's local zone**. The mistake is comparing it against a UTC "now" as if the cut-off were
also UTC. That's only correct for listings that happen to sit in GMT — for every other zone the cut-
off lands at a *different UTC instant*, so the comparison is wrong by the zone's offset (hours off,
and worse across DST).

The fix is to put both sides in the **same frame** before comparing: convert "now" into the
listing's zone (`TimeZoneInfo.ConvertTimeFromUtc(now.UtcDateTime, listingZone)`) and compare to the
local 15:00-day-before cut-off. Passing `DateTimeOffset.UtcNow` is fine — it's the correct *instant*;
the bug is never the instant, it's failing to convert it into the listing's local clock.

## Concrete check

Same instant `2026-07-09 16:00 UTC`, check-in `2026-07-10`:

- Lisbon (UTC+1 in July): local 17:00 → past the 15:00 cut-off → **rejected**.
- Honolulu (UTC−10): local 06:00 → before the cut-off → **allowed**.

Same moment, opposite outcomes — proof the decision depends on the listing's zone.

## Testability

The rule lives on the entity as `Cancel(DateTimeOffset now, IanaTimeZone listingTimeZone)` — `now` is
a **parameter**, not `DateTime.UtcNow` read inside. That makes the cut-off a pure function of its
inputs, so the boundary (just-before vs at/after) is unit-testable without the wall clock.

Reference: Noda Time user guide on time-zone handling (nodatime.org/userguide) — why `DateTime`
arithmetic across zones is error-prone; .NET `TimeProvider` for an injectable "now".
