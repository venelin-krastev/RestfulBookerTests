using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class DeleteBookingTests_v2 : BaseApiTest
{
    private string token;

    [OneTimeSetUp]
    public void Setup()
    {
        var authRequest = new RestRequest("/auth", Method.Post);
        authRequest.AddJsonBody(new { username = "admin", password = "password123" });
        var authResponse = Client.Execute(authRequest);
        token = JObject.Parse(authResponse.Content!)["token"]!.ToString();
    }

    private string CreateTestBooking()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "Delete",
            lastname = "Test",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2026-01-01", checkout = "2026-01-02" },
            additionalneeds = "None"
        });
        return JObject.Parse(Client.Execute(request).Content!)["bookingid"]!.ToString();
    }

    [Test]
    public void DeleteBookingWithValidToken_Returns201()
    {
        var bookingId = CreateTestBooking();

        var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
        request.AddHeader("Cookie", $"token={token}");

        var response = Client.Execute(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(201),
            "DELETE with valid auth token should return 201 Created (known API quirk)");
    }

    [Test]
    public void DeleteBookingWithoutAuth_Returns403()
    {
        var bookingId = CreateTestBooking();

        var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
        // intentionally no Cookie header

        var response = Client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden),
            "DELETE without auth token should be rejected with 403 Forbidden");
    }

    [Test]
    public void DeleteNonExistentBooking_ReturnsNotFoundOrMethodNotAllowed()
    {
        var request = new RestRequest("/booking/99999999", Method.Delete);
        request.AddHeader("Cookie", $"token={token}");

        var response = Client.Execute(request);

        Assert.That((int)response.StatusCode, Is.AnyOf(404, 405),
            "DELETE on non-existent ID should return 404 or 405");
    }

    [Test]
    public void DeletedBooking_CannotBeRetrievedAfterDeletion()
    {
        var bookingId = CreateTestBooking();

        var deleteRequest = new RestRequest($"/booking/{bookingId}", Method.Delete);
        deleteRequest.AddHeader("Cookie", $"token={token}");
        Client.Execute(deleteRequest);

        var getRequest = new RestRequest($"/booking/{bookingId}", Method.Get);
        getRequest.AddHeader("Accept", "application/json");
        var getResponse = Client.Execute(getRequest);

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
            "Deleted booking should return 404 — resource must no longer exist");
    }
}
