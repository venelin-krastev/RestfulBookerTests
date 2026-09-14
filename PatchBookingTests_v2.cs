using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class PatchBookingTests_v2 : BaseApiTest
{
    private string token;

    [OneTimeSetUp]
    public void Setup()
    {
        var authRequest = new RestRequest("/auth", Method.Post);
        authRequest.AddJsonBody(new { username = "admin", password = "password123" });
        token = JObject.Parse(Client.Execute(authRequest).Content!)["token"]!.ToString();
    }

    [Test]
    public void PatchBookingFirstname_ReturnsOkWithUpdatedFirstname()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Jane",
            lastname = "Doe",
            totalprice = 150,
            depositpaid = true,
            bookingdates = new { checkin = "2026-12-01", checkout = "2026-12-05" },
            additionalneeds = "None"
        });
        var bookingId = JObject.Parse(Client.Execute(createRequest).Content!)["bookingid"]!.ToString();

        var patchRequest = new RestRequest($"/booking/{bookingId}", Method.Patch);
        patchRequest.AddHeader("Accept", "application/json");
        patchRequest.AddHeader("Cookie", $"token={token}");
        patchRequest.AddJsonBody(new { firstname = "Janet" });

        var response = Client.Execute(patchRequest);

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            "PATCH /booking/{id} with valid token should return 200 OK");

        var body = JObject.Parse(response.Content!);

        Assert.That(body["firstname"]?.ToString(), Is.EqualTo("Janet"),
            "firstname must be updated to Janet while other fields remain unchanged");
    }
}
