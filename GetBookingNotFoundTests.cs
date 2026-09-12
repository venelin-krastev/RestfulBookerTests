using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class GetBookingNotFoundTests : BaseApiTest
{
    [Test]
    public void GetBookingWithNonExistentId_Returns404()
    {
        var request = new RestRequest("/booking/99999999", Method.Get);
        request.AddHeader("Accept", "application/json");

        var response = Client.Execute(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(404),
            "GET /booking/{id} with non-existent ID should return 404 Not Found");
    }
}
