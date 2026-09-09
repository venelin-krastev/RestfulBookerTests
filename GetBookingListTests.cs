using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class GetBookingListTests : BaseApiTest
{
    [Test]
    public void GetAllBookings_ReturnsOkWithNonEmptyListContainingBookingIds()
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddHeader("Accept", "application/json");

        var response = Client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "GET /booking should return 200 OK");

        Assert.That(response.Content, Is.Not.Null.And.Not.Empty,
            "Response body must not be empty");

        var bookings = JArray.Parse(response.Content!);

        Assert.That(bookings.Count, Is.GreaterThan(0),
            "Booking list should contain at least one entry");

        Assert.That(bookings[0]["bookingid"], Is.Not.Null,
            "First booking object must have a bookingid field");
    }
}
