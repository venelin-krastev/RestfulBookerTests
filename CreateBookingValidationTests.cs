using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class CreateBookingValidationTests : BaseApiTest
{
    [Test]
    public void CreateBookingWithValidData_ReturnsOkWithPositiveBookingId()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "Jane",
            lastname = "Doe",
            totalprice = 250,
            depositpaid = true,
            bookingdates = new { checkin = "2026-10-01", checkout = "2026-10-05" },
            additionalneeds = "Breakfast"
        });

        var response = Client.Execute(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            "POST /booking should return 200 OK (Restful Booker quirk — not 201)");

        var body = JObject.Parse(response.Content!);

        Assert.That(body["bookingid"], Is.Not.Null,
            "Response must contain a bookingid field");

        Assert.That(body["bookingid"]!.Value<int>(), Is.GreaterThan(0),
            "bookingid must be a positive integer");
    }
}
