using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestfulBookerTests;

[TestFixture]
public class AuthTokenTests : BaseApiTest
{
    [Test]
    public void GetAuthToken_ReturnsToken()
    {
        var request = new RestRequest("/auth", Method.Post);
        request.AddJsonBody(new { username = "admin", password = "password123" });

        var response = Client.Execute(request);
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
        request.AddJsonBody(new { username = "wrong", password = "wrong" });

        var response = Client.Execute(request);
        var body = JObject.Parse(response.Content!);

        Assert.That(body["reason"]?.ToString(), Is.EqualTo("Bad credentials"),
            "Wrong credentials should return error reason");
    }
}
