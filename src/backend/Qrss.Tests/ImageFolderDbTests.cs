namespace Qrss.Tests;

internal class ImageFolderDbTests
{
    [Test]
    public void Test_ImageFileDatabase_AddImages()
    {
        GrabImageFolder db = new("./");
        db.Count.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.Count.Should().Be(1);

        db.SaveIfUnique("grabber1", [4, 5, 6], "myGrabber1.png");
        db.Count.Should().Be(2);
    }

    [Test]
    public void Test_ImageFileDatabase_PermitIdenticalHashesFromDifferentGrabbers()
    {
        GrabImageFolder db = new("./");
        db.Count.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.Count.Should().Be(1);

        db.SaveIfUnique("grabber2", [1, 2, 3], "myGrabber1.png");
        db.Count.Should().Be(2);
    }

    [Test]
    public void Test_ImageFileDatabase_IgnoreDuplicateImagesFromSameGrabber()
    {
        GrabImageFolder db = new("./");
        db.Count.Should().Be(0);

        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.Count.Should().Be(1);

        // these are not unique so they should be ignored
        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        db.Count.Should().Be(1);
    }

    [Test]
    public void Test_ImageFileDatabase_AutomaticFilename()
    {
        GrabImageFolder db = new("./");

        string? filename1 = db.SaveIfUnique("grabber1", [1, 2, 3], "myGrabber1.png");
        string? filename2 = db.SaveIfUnique("grabber1", [4, 5, 6], "myGrabber1.png");
        Console.WriteLine(filename1);
        Console.WriteLine(filename2);
        filename1.Should().NotBe(filename2);
    }
}
