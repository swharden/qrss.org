namespace Qrss.Tests;

public class GrabberInfoCsvDbTests
{
    [Test]
    public void Test_CsvDatabase_Read()
    {
        Qrss.Core.GrabberInfoDatabases.CsvGrabberInfoDB csvDb = new(SampleData.GrabberInfoDatabaseCsvFilePath);
        var infos = csvDb.ReadAll();
        infos.Count().Should().Be(181);
    }

    [Test]
    public void Test_CsvDatabase_Write()
    {
        string saveTestFilename = "grabber-info-save-test.csv";

        Qrss.Core.GrabberInfoDatabases.CsvGrabberInfoDB csvDb1 = new(SampleData.GrabberInfoDatabaseCsvFilePath);
        csvDb1.SaveAs(saveTestFilename);

        Qrss.Core.GrabberInfoDatabases.CsvGrabberInfoDB csvDb2 = new(saveTestFilename);

        csvDb1.ReadAll().Count().Should().Be(csvDb2.ReadAll().Count());

        foreach (var id in csvDb1.ReadAll().Select(x => x.ID))
        {
            var info1 = csvDb1.Read(id);
            var info2 = csvDb2.Read(id);
            info1.Should().NotBeNull();
            info2.Should().NotBeNull();
            info1!.Equals(info2).Should().BeTrue();
        };
    }
}
