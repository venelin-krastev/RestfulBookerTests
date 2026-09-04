# Interview Prep — RestfulBookerTests

Real questions a senior QA interviewer would ask when reviewing this project.

---

## 2026-09-04 — BaseApiTest Abstraction

**Q: What is BaseApiTest and why did you introduce it?**
A: `BaseApiTest` is an abstract class that owns the `RestClient` instance and manages its lifecycle. It initialises the client once in `[OneTimeSetUp]` — shared across all tests in the class — and disposes it in `[OneTimeTearDown]`. Without it, every test class (`BookingApiTests`, `AuthTokenTests`, `DeleteBookingTests`...) would duplicate the same 6-7 lines of setup code. By inheriting `BaseApiTest`, each class gets a ready-to-use `client` with no repetition.

**Q: Why [OneTimeSetUp] and not [SetUp] for RestClient?**
A: `[SetUp]` runs before every single test — creating and disposing an HTTP client 32 times per run is wasteful and slow. `RestClient` is stateless between requests, so one instance per test class is both correct and efficient. `[OneTimeSetUp]` runs once per class, which matches the lifecycle of a connection-pool-aware HTTP client.

**Q: What is the difference between abstract and virtual in C#?**
A: A `virtual` method has a full body in the base class — subclasses *can* override it but are not required to. An `abstract` method has no body — it is a contract that every non-abstract subclass *must* implement. `BaseApiTest` uses neither for its setup methods because `[OneTimeSetUp]` in NUnit is inherited automatically without needing override.

---

## 2026-09-04 — RestSharp: AddQueryParameter vs AddHeader vs AddJsonBody

**Q: How do you add a query parameter in RestSharp?**
A: `request.AddQueryParameter("firstname", "John")` appends it to the URL: `/booking?firstname=John`. This is for filtering or searching — parameters the server reads from the URL, not the body.

**Q: How is that different from AddHeader and AddJsonBody?**
A: `AddHeader` sets an HTTP header (e.g. `Cookie: token=abc`, `Accept: application/json`) — metadata about the request. `AddJsonBody` serialises an object into the request body as JSON and automatically sets `Content-Type: application/json` — used for POST and PUT payloads.

---

## 2026-09-04 — GitHub Actions: if: always()

**Q: Why does the artifact upload step have `if: always()`?**
A: Without it, GitHub Actions skips subsequent steps when a previous step fails. The upload step must run even when tests fail — that is exactly when the `.trx` report is most valuable for diagnosing what broke. `if: always()` overrides the default skip behaviour and guarantees the evidence is always uploaded.

**Q: What is the difference between `--no-restore` and `--no-build` in dotnet commands?**
A: `--no-restore` skips NuGet package download — used in `dotnet build` when restore already ran as a separate step. `--no-build` skips recompilation — used in `dotnet test` when build already ran. Separating the three steps (restore → build → test) gives clearer failure attribution in CI: a restore failure means a dependency problem, a build failure means a compilation error, a test failure means a logic error.
