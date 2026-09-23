using NUnit.Framework;
using NUnit.Framework.Interfaces;
using RestSharp;

namespace RestfulBookerTests;

public abstract class BaseApiTest : ITestLogger
{
    protected RestClient Client;
    protected const string BaseUrl = "https://restful-booker.herokuapp.com";

    [OneTimeSetUp]
    public void BaseSetup() => Client = new RestClient(BaseUrl);

    [OneTimeTearDown]
    public void BaseTeardown() => Client.Dispose();

    [SetUp]
    public void LogStart() =>
        ((ITestLogger)this).LogTestStart(TestContext.CurrentContext.Test.Name);

    [TearDown]
    public void LogEnd() =>
        ((ITestLogger)this).LogTestEnd(
            TestContext.CurrentContext.Test.Name,
            TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed);
}
