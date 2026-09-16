using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class GetSingleBookingTests_v2 : BaseApiTest
{
    [Test]
    public void GetBookingById_ReturnsOkWithCorrectFields()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Alice",
            lastname = "Walker",
            totalprice = 300,
            depositpaid = true,
            bookingdates = new { checkin = "2026-11-10", checkout = "2026-11-15" },
            additionalneeds = "Dinner"
        });
        var bookingId = JObject.Parse(Client.Execute(createRequest).Content!)["bookingid"]!.ToString();

        var getRequest = new RestRequest($"/booking/{bookingId}", Method.Get);
        getRequest.AddHeader("Accept", "application/json");

        var response = Client.Execute(getRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "GET /booking/{id} for an existing booking should return 200 OK");

        var body = JObject.Parse(response.Content!);

        Assert.That(body["firstname"]?.ToString(), Is.EqualTo("Alice"),
            "Response must contain the correct firstname");
        Assert.That(body["lastname"]?.ToString(), Is.EqualTo("Walker"),
            "Response must contain the correct lastname");
        Assert.That(body["totalprice"]?.Value<int>(), Is.EqualTo(300),
            "Response must contain the correct totalprice");
    }

    [Test]
    public void GetBookingById_WithNonExistentId_Returns404()
    {
        var request = new RestRequest("/booking/99999999", Method.Get);
        request.AddHeader("Accept", "application/json");

        var response = Client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
            "GET /booking/{id} for a non-existent id should return 404 Not Found");
    }
}
