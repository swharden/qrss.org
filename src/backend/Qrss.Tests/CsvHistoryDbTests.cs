using Qrss.Core.GrabberHistoryDatabases;

namespace Qrss.Tests;
internal class CsvHistoryDbTests
{
    [Test]
    public void Test_CsvHistory_IgnoreDuplicateHashes()
    {
        CsvHistoryDB db = new(string.Empty);
        db.ImageCount.Should().Be(0);

        // add a unique grab
        db.AddGrab("grabber1", [1, 2, 3]);
        db.ImageCount.Should().Be(1);

        // add a duplicate grab
        db.AddGrab("grabber1", [1, 2, 3]);
        db.ImageCount.Should().Be(1);

        // add a unique grab
        db.AddGrab("grabber1", [2, 2, 2]);
        db.ImageCount.Should().Be(2);

        // add a unique grab
        db.AddGrab("grabber2", [2, 2, 2]);
        db.ImageCount.Should().Be(3);
    }
}
