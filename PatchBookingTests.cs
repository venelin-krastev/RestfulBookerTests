using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class PatchBookingTests
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
            lastname = "Patch",
            totalprice = 500,
            depositpaid = true,
            bookingdates = new { checkin = "2025-03-01", checkout = "2025-03-07" },
            additionalneeds = "None"
        });
        bookingId = JObject.Parse(client.Execute(createRequest).Content!)["bookingid"]!.ToString();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void PatchBooking_FirstnameOnly_ReturnsOk()
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Patch);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Cookie", $"token={token}");
        request.AddJsonBody(new { firstname = "Patched" });

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "PATCH /booking/{id} should return 200 OK");
    }

    [Test]
    public void PatchBooking_OnlyUpdatesTargetField()
    {
        var patchRequest = new RestRequest($"/booking/{bookingId}", Method.Patch);
        patchRequest.AddHeader("Content-Type", "application/json");
        patchRequest.AddHeader("Accept", "application/json");
        patchRequest.AddHeader("Cookie", $"token={token}");
        patchRequest.AddJsonBody(new { firstname = "PatchedName" });
        client.Execute(patchRequest);

        var getRequest = new RestRequest($"/booking/{bookingId}", Method.Get);
        getRequest.AddHeader("Accept", "application/json");
        var getResponse = client.Execute(getRequest);
        var body = JObject.Parse(getResponse.Content!);

        Assert.That(body["firstname"]?.ToString(), Is.EqualTo("PatchedName"),
            "PATCH should update firstname");
        Assert.That(body["totalprice"]?.ToString(), Is.EqualTo("500"),
            "PATCH should not modify totalprice");
    }

    [Test]
    public void PatchBooking_WithoutToken_ReturnsForbidden()
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Patch);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new { firstname = "Unauthorized" });

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden),
            "PATCH without auth token should return 403 Forbidden");
    }
}
