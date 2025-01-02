using FluentAssertions;

namespace Qrss.Tests;

public static class SampleData
{
    public static string GrabberInfoDatabaseCsvFilePath => Path.GetFullPath("test-data/grabbers.csv");

    [Test]
    public static void Test_GrabberInfoDatabaseCsvFilePath_Exists()
    {
        File.Exists(GrabberInfoDatabaseCsvFilePath).Should().BeTrue();
    }
}
