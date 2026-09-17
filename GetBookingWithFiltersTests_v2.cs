using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class GetBookingWithFiltersTests_v2 : BaseApiTest
{
    [Test]
    public void GetBookings_FilteredByFirstname_ReturnsMatchingResults()
    {
        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = "FilterAlice",
            lastname = "FilterTest",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new { checkin = "2026-12-01", checkout = "2026-12-05" },
            additionalneeds = "None"
        });
        Client.Execute(createRequest);

        var getRequest = new RestRequest("/booking", Method.Get);
        getRequest.AddHeader("Accept", "application/json");
        getRequest.AddQueryParameter("firstname", "FilterAlice");

        var response = Client.Execute(getRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "GET /booking?firstname=FilterAlice should return 200 OK");

        var bookings = JArray.Parse(response.Content!);

        Assert.That(bookings.Count, Is.GreaterThan(0),
            "Filtering by a known firstname must return at least one result");
    }

    [Test]
    public void GetBookings_WithNonMatchingFilter_ReturnsEmptyList()
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddHeader("Accept", "application/json");
        request.AddQueryParameter("firstname", "ZZZnonexistent999");

        var response = Client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "GET /booking with non-matching filter should still return 200 OK");

        var bookings = JArray.Parse(response.Content!);

        Assert.That(bookings.Count, Is.EqualTo(0),
            "Non-matching filter must return an empty list");
    }
}
