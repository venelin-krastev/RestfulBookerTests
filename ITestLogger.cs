namespace RestfulBookerTests;

public interface ITestLogger
{
    void LogTestStart(string testName) =>
        Console.WriteLine($"[START] {testName} - {DateTime.Now:HH:mm:ss}");

    void LogTestEnd(string testName, bool passed) =>
        Console.WriteLine($"[{(passed ? "PASS" : "FAIL")}] {testName}");
}
