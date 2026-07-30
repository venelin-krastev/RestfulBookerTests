using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class SchemaValidationTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";
    private JObject booking;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        client = new RestClient(BaseUrl);

        var listRequest = new RestRequest("/booking", Method.Get);
        var listResponse = client.Execute(listRequest);
        var ids = JArray.Parse(listResponse.Content!);
        var firstId = ids[0]["bookingid"]!.ToString();

        var getRequest = new RestRequest($"/booking/{firstId}", Method.Get);
        getRequest.AddHeader("Accept", "application/json");
        var getResponse = client.Execute(getRequest);
        booking = JObject.Parse(getResponse.Content!);
    }

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void GetBooking_ResponseHasAllRequiredFields()
    {
        Assert.That(booking["firstname"], Is.Not.Null, "Missing field: firstname");
        Assert.That(booking["lastname"], Is.Not.Null, "Missing field: lastname");
        Assert.That(booking["totalprice"], Is.Not.Null, "Missing field: totalprice");
        Assert.That(booking["depositpaid"], Is.Not.Null, "Missing field: depositpaid");
        Assert.That(booking["bookingdates"], Is.Not.Null, "Missing field: bookingdates");
    }

    [Test]
    public void GetBooking_TotalpriceIsInteger()
    {
        Assert.That(booking["totalprice"]?.Type, Is.EqualTo(JTokenType.Integer),
            "totalprice should be Integer, not string or other type");
    }

    [Test]
    public void GetBooking_DepositpaidIsBoolean()
    {
        Assert.That(booking["depositpaid"]?.Type, Is.EqualTo(JTokenType.Boolean),
            "depositpaid should be Boolean, not string or other type");
    }

    [Test]
    public void GetBooking_BookingdatesHasCheckinAndCheckout()
    {
        var dates = booking["bookingdates"];

        Assert.That(dates?["checkin"], Is.Not.Null, "Missing field: bookingdates.checkin");
        Assert.That(dates?["checkout"], Is.Not.Null, "Missing field: bookingdates.checkout");
    }
}
