using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class ParameterizedAuthTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";

    [OneTimeSetUp]
    public void OneTimeSetup() => client = new RestClient(BaseUrl);

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [TestCase("wrong", "password123")]
    [TestCase("admin", "wrongpass")]
    [TestCase("", "")]
    public void InvalidCredentials_ReturnsBadCredentials(string username, string password)
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username, password });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(body["reason"]?.ToString(), Is.EqualTo("Bad credentials"),
            $"username='{username}', password='{password}' should return Bad credentials");
    }

    [TestCase("admin", "password123")]
    public void ValidCredentials_ReturnsNonEmptyToken(string username, string password)
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username, password });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "POST /auth should return 200 OK");
        Assert.That(body["token"]?.ToString(), Is.Not.Null.And.Not.Empty,
            $"Valid credentials should return a non-empty token");
    }

    [TestCase("firstname", "Original")]
    [TestCase("firstname", "UniqueQATest")]
    public void FilterByParameter_ReturnsOk(string paramName, string paramValue)
    {
        var request = new RestRequest("/booking", Method.Get);
        request.AddQueryParameter(paramName, paramValue);

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            $"GET /booking?{paramName}={paramValue} should return 200 OK");
    }
}
