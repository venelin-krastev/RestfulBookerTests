# RestfulBookerTests

![Tests](https://github.com/venelin-krastev/RestfulBookerTests/actions/workflows/tests.yml/badge.svg)

Automated API tests for [Restful Booker](https://restful-booker.herokuapp.com) — a practice REST API for QA automation.

## Tech Stack

- **C# / .NET 10**
- **RestSharp 114**
- **NUnit 4.3**
- **Newtonsoft.Json**

## Test Coverage

| Class | Endpoint | Method | Tests |
|---|---|---|---|
| `BookingApiTests.cs` | `/booking` | GET | 3 |
| `CreateBookingTests.cs` | `/booking` | POST | 2 |

**Total: 5 tests**

## Key Concepts Demonstrated

- **RestClient in `[OneTimeSetUp]`** — thread-safe, expensive to initialize once per suite
- **HTTP status code assertions** — 200 OK, 404 Not Found
- **JSON parsing** — `JObject.Parse(response.Content!)` for field-level assertions
- **POST with JSON body** — `request.AddJsonBody()` with explicit Content-Type header
- **API bug documentation** — POST /booking returns 200 instead of 201 (known API quirk)

## Tests

### BookingApiTests.cs
| Test | Description |
|---|---|
| `GetAllBookings_ReturnsOkWithBookingIds` | GET /booking returns 200 with booking IDs |
| `GetBookingById_ReturnsCorrectFields` | GET /booking/1 returns firstname and totalprice fields |
| `GetBookingWithInvalidId_ReturnsNotFound` | GET /booking/999999 returns 404 |

### CreateBookingTests.cs
| Test | Description |
|---|---|
| `CreateBooking_ReturnsCreatedWithBookingId` | POST /booking returns 200 with new bookingid |
| `CreateBooking_ResponseContainsSubmittedData` | POST /booking response matches submitted firstname, lastname, totalprice |

## How to Run

```bash
dotnet test
```

## Author

Venelin Krastev — Junior QA Automation Engineer, Sofia
