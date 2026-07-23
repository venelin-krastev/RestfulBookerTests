using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class BookingApiTests
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
    public void GetAllBookings_ReturnsOkWithBookingIds()
    {
        var request = new RestRequest("/booking", Method.Get);
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "GET /booking should return 200 OK");
        Assert.That(response.Content, Does.Contain("bookingid"),
            "Response should contain booking IDs");
    }

    [Test]
    public void GetBookingById_ReturnsCorrectFields()
    {
        var listRequest = new RestRequest("/booking", Method.Get);
        var listResponse = client.Execute(listRequest);
        var ids = JArray.Parse(listResponse.Content!);
        var firstId = ids[0]["bookingid"]!.ToString();

        var request = new RestRequest($"/booking/{firstId}", Method.Get);
        request.AddHeader("Accept", "application/json");
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"GET /booking/{firstId} should return 200 OK");

        var body = JObject.Parse(response.Content!);
        Assert.That(body["firstname"]?.ToString(), Is.Not.Null.And.Not.Empty,
            "Response should contain firstname field");
        Assert.That(body["totalprice"]?.ToString(), Is.Not.Null,
            "Response should contain totalprice field");
    }

    [Test]
    public void GetBookingWithInvalidId_ReturnsNotFound()
    {
        var request = new RestRequest("/booking/999999", Method.Get);
        request.AddHeader("Accept", "application/json");
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
            "Non-existent booking should return 404");
    }
}
