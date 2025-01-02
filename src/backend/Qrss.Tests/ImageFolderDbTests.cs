using Qrss.Core.GrabImageManagers;

namespace Qrss.Tests;

internal class ImageFolderDbTests
{
    [Test]
    public void Test_ImageFileDatabase_AddImages()
    {
        FlatFolder db = new("./");
        db.ImageCount.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.ImageCount.Should().Be(1);

        db.SaveIfUnique("grabber1", [4, 5, 6], "myGrabber1.png");
        db.ImageCount.Should().Be(2);
    }

    [Test]
    public void Test_ImageFileDatabase_PermitIdenticalHashesFromDifferentGrabbers()
    {
        FlatFolder db = new("./");
        db.ImageCount.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.ImageCount.Should().Be(1);

        db.SaveIfUnique("grabber2", [1, 2, 3], "myGrabber1.png");
        db.ImageCount.Should().Be(2);
    }

    [Test]
    public void Test_ImageFileDatabase_IgnoreDuplicateImagesFromSameGrabber()
    {
        FlatFolder db = new("./");
        db.ImageCount.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.ImageCount.Should().Be(1);

        // these are not unique so they should be ignored
        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.ImageCount.Should().Be(1);
    }

    [Test]
    public void Test_ImageFileDatabase_AutomaticFilename()
    {
        FlatFolder db = new("./");

        string? filename1 = db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        string? filename2 = db.SaveIfUnique("grabber1", [4, 5, 6], "myGrabber1.png");
        Console.WriteLine(filename1);
        Console.WriteLine(filename2);
        filename1.Should().NotBe(filename2);
    }
}
