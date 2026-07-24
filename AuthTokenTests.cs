using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class AuthTokenTests
{
    private RestClient client;
    private const string BaseUrl = "https://restful-booker.herokuapp.com";

    [OneTimeSetUp]
    public void OneTimeSetup() => client = new RestClient(BaseUrl);

    [OneTimeTearDown]
    public void OneTimeTeardown() => client.Dispose();

    [Test]
    public void GetAuthToken_ReturnsToken()
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username = "admin", password = "password123" });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
            "POST /auth should return 200 OK");
        Assert.That(body["token"]?.ToString(), Is.Not.Null.And.Not.Empty,
            "Response should contain a non-empty token");
    }

    [Test]
    public void GetAuthToken_WithWrongCredentials_ReturnsError()
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { username = "wrong", password = "wrong" });

        var response = client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(body["reason"]?.ToString(), Is.EqualTo("Bad credentials"),
            "Wrong credentials should return error reason");
    }
}
