using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class UpdateBookingTests : BaseApiTest
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

    [Test]
    public void UpdateBookingWithValidToken_ReturnsOkWithUpdatedFirstname()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Original",
            lastname = "Name",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2026-11-01", checkout = "2026-11-05" },
            additionalneeds = "None"
        });
        var bookingId = JObject.Parse(Client.Execute(createRequest).Content!)["bookingid"]!.ToString();

        var updateRequest = new RestRequest($"/booking/{bookingId}", Method.Put);
        updateRequest.AddHeader("Accept", "application/json");
        updateRequest.AddHeader("Cookie", $"token={token}");
        updateRequest.AddJsonBody(new
        {
            firstname = "Updated",
            lastname = "Name",
            totalprice = 200,
            depositpaid = false,
            bookingdates = new { checkin = "2026-11-01", checkout = "2026-11-05" },
            additionalneeds = "Lunch"
        });

        var response = Client.Execute(updateRequest);

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            "PUT /booking/{id} with valid token should return 200 OK");

        var body = JObject.Parse(response.Content!);

        Assert.That(body["firstname"]!.ToString(), Is.EqualTo("Updated"),
            "firstname in response must reflect the updated value");
    }
}
