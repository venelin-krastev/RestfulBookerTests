using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class FilterBookingTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";
    private const string UniqueFirstname = "UniqueQATest";

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        client = new RestClient(BaseUrl);

        var createRequest = new RestRequest("/booking", Method.Post);
        createRequest.AddHeader("Content-Type", "application/json");
        createRequest.AddHeader("Accept", "application/json");
        createRequest.AddJsonBody(new
        {
            firstname = UniqueFirstname,
            lastname = "Filter",
            totalprice = 123,
            depositpaid = true,
            bookingdates = new { checkin = "2025-05-01", checkout = "2025-05-07" },
            additionalneeds = "None"
        });
        client.Execute(createRequest);
    }

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void FilterByFirstname_ReturnsAtLeastOneResult()
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddQueryParameter("firstname", UniqueFirstname);

        var response = client.Execute(request);
        var results = JArray.Parse(response.Content!);

        Assert.That(results.Count, Is.GreaterThan(0),
            $"Filter by firstname={UniqueFirstname} should return at least one booking");
    }

    [Test]
    public void FilterByNonExistentName_ReturnsEmptyArray()
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddQueryParameter("firstname", "ZZZNOBODYHASTHISNAME99999");

        var response = client.Execute(request);
        var results = JArray.Parse(response.Content!);

        Assert.That(results.Count, Is.EqualTo(0),
            "Filter by non-existent name should return empty array");
    }

    [Test]
    public void FilterByCheckin_ReturnsResults()
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddQueryParameter("checkin", "2025-05-01");

        var response = client.Execute(request);
        var results = JArray.Parse(response.Content!);

        Assert.That(results.Count, Is.GreaterThan(0),
            "Filter by checkin=2025-05-01 should return at least one booking");
    }
}
