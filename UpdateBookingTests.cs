using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class UpdateBookingTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";
    private string token;
    private string bookingId;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        client = new RestClient(BaseUrl);

        var authRequest = new RestRequest("/auth", Method.Post);
        authRequest.AddHeader("Content-Type", "application/json");
        authRequest.AddJsonBody(new { username = "admin", password = "password123" });
        token = JObject.Parse(client.Execute(authRequest).Content!)["token"]!.ToString();

        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Content-Type", "application/json");
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Original",
            lastname = "Name",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2025-01-01", checkout = "2025-01-07" },
            additionalneeds = "None"
        });
        bookingId = JObject.Parse(client.Execute(createRequest).Content!)["bookingid"]!.ToString();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void UpdateBooking_ReturnsOk()
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Put);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        request.AddJsonBody(new
        {
            firstname = "Updated",
            lastname = "Name",
            totalprice = 200,
            depositpaid = false,
            bookingdates = new { checkin = "2025-06-01", checkout = "2025-06-07" },
            additionalneeds = "Breakfast"
        });

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "PUT /booking/{id} should return 200 OK");
    }

    [Test]
    public void UpdateBooking_ResponseContainsUpdatedData()
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Put);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        request.AddJsonBody(new
        {
            firstname = "John",
            lastname = "Updated",
            totalprice = 999,
            depositpaid = true,
            bookingdates = new { checkin = "2025-12-01", checkout = "2025-12-31" },
            additionalneeds = "Lunch"
        });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(body["firstname"]?.ToString(), Is.EqualTo("John"),
            "Firstname should be updated");
        Assert.That(body["totalprice"]?.ToString(), Is.EqualTo("999"),
            "Totalprice should be updated");
    }

    [Test]
    public void UpdateBooking_WithoutToken_ReturnsForbidden()
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Put);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "Hacker",
            lastname = "Attempt",
            totalprice = 0,
            depositpaid = false,
            bookingdates = new { checkin = "2025-01-01", checkout = "2025-01-02" },
            additionalneeds = "None"
        });

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden),
            "PUT without auth token should return 403 Forbidden");
    }
}
