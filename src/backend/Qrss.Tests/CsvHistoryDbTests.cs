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

    [Test]
    public void Test_CsvHistory_WriteAndRead()
    {
        CsvHistoryDB db1 = new(string.Empty);
        db1.AddGrab("grabberA", [1, 1, 1]);
        db1.AddGrab("grabberB", [1, 1, 2]);
        db1.AddGrab("grabberB", [1, 1, 3]);
        db1.AddGrab("grabberC", [1, 1, 4]);
        db1.ImageCount.Should().Be(4);
        db1.GetGrabberIDs().Length.Should().Be(3);

        string csvText = db1.GetCsvText();
        Console.WriteLine(csvText);

        CsvHistoryDB db2 = new(csvText);
        db2.ImageCount.Should().Be(db1.ImageCount);
        db2.GetGrabberIDs().Length.Should().Be(3);

        db1.GetCsvText().Should().Be(db2.GetCsvText());

        foreach (string grabberID in db1.GetGrabberIDs())
        {
            string[] hashes1 = db1.GetHashes(grabberID);
            string[] hashes2 = db2.GetHashes(grabberID);
            hashes1.Should().BeEquivalentTo(hashes2);

            DateTime[] dates1 = db1.GetDates(grabberID);
            DateTime[] dates2 = db2.GetDates(grabberID);
            dates1.Should().BeEquivalentTo(dates2);
        }
    }

    [Test]
    public void Test_CsvHistory_KeepLatestOnly()
    {
        CsvHistoryDB db1 = new(string.Empty);

        for (byte i = 0; i < 10; i++)
        {
            db1.AddGrab("grabberA", [1, 1, i]);
        }
        db1.ImageCount.Should().Be(5);

        Console.WriteLine(db1.GetCsvText());
    }
}
