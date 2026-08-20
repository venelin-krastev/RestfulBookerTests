# RestfulBookerTests

![Tests](https://github.com/venelin-krastev/RestfulBookerTests/actions/workflows/tests.yml/badge.svg)

Automated API test suite for [Restful Booker](https://restful-booker.herokuapp.com) — a practice REST API for QA automation.

## Tech Stack

- **C# / .NET 10**
- **RestSharp 114**
- **NUnit 4.3**
- **Newtonsoft.Json**

## Test Coverage

| Class | Endpoint | Method | Tests |
|---|---|---|---|
| `BookingApiTests.cs` | `/booking`, `/booking/{id}` | GET | 3 |
| `CreateBookingTests.cs` | `/booking` | POST | 2 |
| `AuthTokenTests.cs` | `/auth` | POST | 2 |
| `DeleteBookingTests.cs` | `/booking/{id}` | DELETE | 2 |
| `UpdateBookingTests.cs` | `/booking/{id}` | PUT | 3 |
| `PatchBookingTests.cs` | `/booking/{id}` | PATCH | 3 |
| `SchemaValidationTests.cs` | `/booking/{id}` | GET | 4 |
| `FilterBookingTests.cs` | `/booking?firstname` | GET | 3 |
| `ParameterizedAuthTests.cs` | `/auth`, `/booking` | POST, GET | 6 |
| `PerformanceTests.cs` | `/booking`, `/auth` | GET, POST | 4 |

**Total: 32 tests**

## Key Concepts Demonstrated

- **RestClient in `[OneTimeSetUp]`** — thread-safe, initialized once per suite to avoid per-test TCP overhead
- **Dynamic booking ID** — created fresh in `[OneTimeSetUp]`, never hardcoded (CI resets data on each run)
- **Token-based auth** — `POST /auth` → token stored → sent as `Cookie: token={value}` on PUT/PATCH/DELETE
- **Full CRUD coverage** — GET, POST, PUT, PATCH, DELETE with positive and negative scenarios
- **PUT vs PATCH** — full replace vs partial update, verified with GET after modification
- **Schema validation** — `JTokenType.Integer`, `JTokenType.Boolean` for type-level assertions
- **Query parameter filtering** — `request.AddQueryParameter()` for `GET /booking?firstname=`
- **Parameterized tests** — `[TestCase]` for data-driven scenarios — multiple credential combinations in one test method
- **BaseApiTest** — abstract base class eliminating `RestClient` setup duplication across all test fixtures
- **Response time assertions** — `Stopwatch` measuring actual HTTP response time against a 2000ms threshold
- **Non-standard status codes** — POST returns 200 (not 201), DELETE returns 201 (not 204) — asserted on actual behaviour
- **403 Forbidden** — PUT/PATCH/DELETE without auth token

## Non-Obvious Implementation Details

**Why `RestClient` is in `[OneTimeSetUp]`**
`RestClient` is thread-safe and reuses the underlying HTTP connection pool. Creating a new client per test would open and close a TCP connection for every request — wasteful and slower. One shared instance per test fixture eliminates that overhead.

**Why the auth token is sent as a Cookie, not an Authorization header**
Restful-Booker's API does not follow the standard `Authorization: Bearer {token}` convention. It expects `Cookie: token={value}` on PUT, PATCH, and DELETE requests. This is a known quirk of the practice API — tests assert on actual behaviour, not RFC convention.

**Why `BaseApiTest` is `abstract`**
Prevents direct instantiation of the base class. Every test fixture inherits `RestClient` setup and teardown from one place — adding a header, changing the base URL, or adjusting timeout requires a change in exactly one file.

**Why a dynamic booking ID is created in `[OneTimeSetUp]`**
The Restful-Booker API resets its data between CI runs and bookings may be deleted by other users. Hardcoding an ID would cause intermittent 404 failures. A fresh booking is created before the suite runs, its ID stored as a static property, and all tests that need a real booking ID reference it.

## Non-Standard API Behaviour

| Scenario | Expected | Actual |
|---|---|---|
| POST /booking — create booking | 201 Created | 200 OK |
| DELETE /booking/{id} — delete booking | 204 No Content | 201 Created |

Tests assert on actual behaviour, not RFC convention.

## Tests

### BookingApiTests.cs
| Test | Description |
|---|---|
| `GetAllBookings_ReturnsOkWithBookingIds` | GET /booking returns 200 with booking IDs |
| `GetBookingById_ReturnsCorrectFields` | Dynamic first ID — returns firstname and totalprice |
| `GetBookingWithInvalidId_ReturnsNotFound` | GET /booking/999999 returns 404 |

### CreateBookingTests.cs
| Test | Description |
|---|---|
| `CreateBooking_ReturnsOk` | POST /booking returns 200 (known API quirk, not 201) |
| `CreateBooking_ResponseContainsSubmittedData` | Response matches submitted firstname, lastname, totalprice |

### AuthTokenTests.cs
| Test | Description |
|---|---|
| `GetAuthToken_ReturnsToken` | POST /auth with valid credentials returns non-empty token |
| `GetAuthToken_WithWrongCredentials_ReturnsError` | Wrong credentials return `{"reason":"Bad credentials"}` |

### DeleteBookingTests.cs
| Test | Description |
|---|---|
| `DeleteBooking_ReturnsCreated` | DELETE returns 201 Created (known API quirk) |
| `DeletedBooking_CannotBeRetrieved` | GET after DELETE returns 404 |

### UpdateBookingTests.cs
| Test | Description |
|---|---|
| `UpdateBooking_ReturnsOk` | PUT /booking/{id} with auth returns 200 |
| `UpdateBooking_ResponseContainsUpdatedData` | Response body reflects all updated fields |
| `UpdateBooking_WithoutToken_ReturnsForbidden` | PUT without Cookie token returns 403 |

### PatchBookingTests.cs
| Test | Description |
|---|---|
| `PatchBooking_FirstnameOnly_ReturnsOk` | PATCH with single field returns 200 |
| `PatchBooking_OnlyUpdatesTargetField` | GET after PATCH confirms only target field changed |
| `PatchBooking_WithoutToken_ReturnsForbidden` | PATCH without Cookie token returns 403 |

### SchemaValidationTests.cs
| Test | Description |
|---|---|
| `GetBooking_ResponseHasAllRequiredFields` | All 5 required fields present in response |
| `GetBooking_TotalpriceIsInteger` | `totalprice` type is `JTokenType.Integer` |
| `GetBooking_DepositpaidIsBoolean` | `depositpaid` type is `JTokenType.Boolean` |
| `GetBooking_BookingdatesHasCheckinAndCheckout` | Nested `bookingdates` object has both date fields |

### FilterBookingTests.cs
| Test | Description |
|---|---|
| `FilterByFirstname_ReturnsAtLeastOneResult` | GET /booking?firstname=UniqueQATest returns results |
| `FilterByNonExistentName_ReturnsEmptyArray` | Filter by non-existent name returns empty array |
| `FilterByCheckin_ReturnsResults` | GET /booking?checkin=2025-05-01 returns results |

## How to Run

```bash
dotnet test
```

## Author

Venelin Krastev — Junior QA Automation Engineer, Sofia
