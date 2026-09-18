using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class UpdateBookingTests_v2 : BaseApiTest
{
    private string GetAuthToken()
    {
        var authRequest = new RestRequest("/auth", Method.Post);
        authRequest.AddJsonBody(new { username = "admin", password = "password123" });
        var token = JObject.Parse(Client.Execute(authRequest).Content!)["token"]!.ToString();
        return token;
    }

    private string CreateTestBooking()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "UpdateTest",
            lastname = "Original",
            totalprice = 100,
            depositpaid = false,
            bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-05" },
            additionalneeds = "None"
        });
        return JObject.Parse(Client.Execute(request).Content!)["bookingid"]!.ToString();
    }

    [Test]
    public void UpdateBooking_WithValidToken_ReturnsOkWithUpdatedFields()
    {
        var bookingId = CreateTestBooking();
        var token = GetAuthToken();

        var putRequest = new RestRequest($"/booking/{bookingId}", Method.Put);
        putRequest.AddHeader("Accept", "application/json");
        putRequest.AddHeader("Cookie", $"token={token}");
        putRequest.AddJsonBody(new
        {
            firstname = "UpdatedName",
            lastname = "UpdatedLast",
            totalprice = 999,
            depositpaid = true,
            bookingdates = new { checkin = "2026-11-01", checkout = "2026-11-10" },
            additionalneeds = "Breakfast"
        });

        var response = Client.Execute(putRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "PUT /booking/{id} with valid token should return 200 OK");

        var body = JObject.Parse(response.Content!);
        Assert.That(body["firstname"]?.ToString(), Is.EqualTo("UpdatedName"),
            "Response must reflect the updated firstname");
        Assert.That(body["totalprice"]?.Value<int>(), Is.EqualTo(999),
            "Response must reflect the updated totalprice");
        Assert.That(body["depositpaid"]?.Value<bool>(), Is.True,
            "Response must reflect the updated depositpaid");
    }

    [Test]
    public void UpdateBooking_WithoutAuth_Returns403()
    {
        var bookingId = CreateTestBooking();

        var putRequest = new RestRequest($"/booking/{bookingId}", Method.Put);
        putRequest.AddHeader("Accept", "application/json");
        putRequest.AddJsonBody(new
        {
            firstname = "ShouldFail",
            lastname = "NoAuth",
            totalprice = 1,
            depositpaid = false,
            bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-02" },
            additionalneeds = "None"
        });

        var response = Client.Execute(putRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden),
            "PUT /booking/{id} without auth token should return 403 Forbidden");
    }

    [Test]
    public void UpdateBooking_WithNonExistentId_Returns404()
    {
        var token = GetAuthToken();

        var putRequest = new RestRequest("/booking/99999999", Method.Put);
        putRequest.AddHeader("Accept", "application/json");
        putRequest.AddHeader("Cookie", $"token={token}");
        putRequest.AddJsonBody(new
        {
            firstname = "Ghost",
            lastname = "Booking",
            totalprice = 1,
            depositpaid = false,
            bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-02" },
            additionalneeds = "None"
        });

        var response = Client.Execute(putRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
            "PUT /booking/{id} for non-existent booking should return 404 Not Found");
    }
}
