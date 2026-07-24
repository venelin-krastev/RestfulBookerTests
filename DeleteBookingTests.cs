using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class DeleteBookingTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";
    private string token;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        client = new RestClient(BaseUrl);

        var authRequest = new RestRequest("/auth", Method.Post);
        authRequest.AddHeader("Content-Type", "application/json");
        authRequest.AddJsonBody(new { username = "admin", password = "password123" });
        var authResponse = client.Execute(authRequest);
        token = JObject.Parse(authResponse.Content!)["token"]!.ToString();
    }

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void DeleteBooking_ReturnsCreated()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Content-Type", "application/json");
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Delete",
            lastname = "Me",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2025-01-01", checkout = "2025-01-02" },
            additionalneeds = "None"
        });
        var bookingId = JObject.Parse(client.Execute(createRequest).Content!)["bookingid"]!.ToString();

        var deleteRequest = new RestRequest($"/booking/{bookingId}", Method.Delete);
        deleteRequest.AddHeader("Cookie", $"token={token}");
        var deleteResponse = client.Execute(deleteRequest);

        Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(201),
            "DELETE should return 201 Created (known API quirk)");
    }

    [Test]
    public void DeletedBooking_CannotBeRetrieved()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Content-Type", "application/json");
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "Gone",
            lastname = "Soon",
            totalprice = 50,
            depositpaid = false,
            bookingdates = new { checkin = "2025-02-01", checkout = "2025-02-03" },
            additionalneeds = "None"
        });
        var bookingId = JObject.Parse(client.Execute(createRequest).Content!)["bookingid"]!.ToString();

        var deleteRequest = new RestRequest($"/booking/{bookingId}", Method.Delete);
        deleteRequest.AddHeader("Cookie", $"token={token}");
        client.Execute(deleteRequest);

        var getRequest = new RestRequest($"/booking/{bookingId}", Method.Get);
        getRequest.AddHeader("Accept", "application/json");
        var getResponse = client.Execute(getRequest);

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
            "Deleted booking should return 404 when retrieved");
    }
}
