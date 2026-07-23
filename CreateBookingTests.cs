using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class CreateBookingTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        client = new RestClient(BaseUrl);
    }

    [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        client.Dispose();
    }

    [Test]
    public void CreateBooking_ReturnsCreatedWithBookingId()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "John",
            lastname = "Smith",
            totalprice = 150,
            depositpaid = true,
            bookingdates = new { checkin = "2025-01-01", checkout = "2025-01-07" },
            additionalneeds = "Breakfast"
        });

        var response = client.Execute(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200),
            "POST /booking should return 200 (API quirk — not 201)");

        var body = JObject.Parse(response.Content!);
        Assert.That(body["bookingid"]?.ToString(), Is.Not.Null.And.Not.Empty,
            "Response should contain the new booking ID");
    }

    [Test]
    public void CreateBooking_ResponseContainsSubmittedData()
    {
        var request = new RestRequest("/booking", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new
        {
            firstname = "Jane",
            lastname = "Doe",
            totalprice = 200,
            depositpaid = false,
            bookingdates = new { checkin = "2025-03-01", checkout = "2025-03-05" },
            additionalneeds = "Lunch"
        });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);
        var booking = body["booking"]!;

        Assert.That(booking["firstname"]?.ToString(), Is.EqualTo("Jane"),
            "Firstname should match submitted value");
        Assert.That(booking["lastname"]?.ToString(), Is.EqualTo("Doe"),
            "Lastname should match submitted value");
        Assert.That(booking["totalprice"]?.ToString(), Is.EqualTo("200"),
            "Totalprice should match submitted value");
    }
}
