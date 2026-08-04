using System.Diagnostics;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class PerformanceTests : BaseApiTest
{
    private const int MaxResponseTimeMs = 2000;

    [Test]
    public void GetAllBookings_RespondsWithinTimeLimit()
    {
        var request = new RestRequest("/booking", Method.Get);

        var sw = Stopwatch.StartNew();
        Client.Execute(request);
        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(MaxResponseTimeMs),
            $"GET /booking should respond within {MaxResponseTimeMs}ms");
    }

    [Test]
    public void GetSingleBooking_RespondsWithinTimeLimit()
    {
        var listRequest = new RestRequest("/booking", Method.Get);
        var ids = JArray.Parse(Client.Execute(listRequest).Content!);
        var firstId = ids[0]["bookingid"]!.ToString();

        var request = new RestRequest($"/booking/{firstId}", Method.Get);
        request.AddHeader("Accept", "application/json");

        var sw = Stopwatch.StartNew();
        Client.Execute(request);
        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(MaxResponseTimeMs),
            $"GET /booking/{{id}} should respond within {MaxResponseTimeMs}ms");
    }

    [Test]
    public void PostAuth_RespondsWithinTimeLimit()
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username = "admin", password = "password123" });

        var sw = Stopwatch.StartNew();
        Client.Execute(request);
        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(MaxResponseTimeMs),
            $"POST /auth should respond within {MaxResponseTimeMs}ms");
    }

    [Test]
    public void CreateBooking_RespondsWithinTimeLimit()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "Perf",
            lastname = "Test",
            totalprice = 1,
            depositpaid = false,
            bookingdates = new { checkin = "2025-01-01", checkout = "2025-01-02" },
            additionalneeds = "None"
        });

        var sw = Stopwatch.StartNew();
        Client.Execute(request);
        sw.Stop();

        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(MaxResponseTimeMs),
            $"POST /booking should respond within {MaxResponseTimeMs}ms");
    }
}
