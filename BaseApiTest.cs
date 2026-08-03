using NUnit.Framework;
using RestSharp;

namespace RestfulBookerTests;

public abstract class BaseApiTest
{
    protected RestClient Client;
    protected const string BaseUrl = "https://restful-booker.herokuapp.com";

    [OneTimeSetUp]
    public void BaseSetup() => Client = new RestClient(BaseUrl);

    [OneTimeTearDown]
    public void BaseTeardown() => Client.Dispose();
}
