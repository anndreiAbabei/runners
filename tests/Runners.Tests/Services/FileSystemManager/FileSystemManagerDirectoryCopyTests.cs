using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerDirectoryCopyTests
{
    [Fact]
    public void Should_CopyDirectory()
    {
        // arrange
        const string fileName = "file.txt";
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);
        var fileData = Path.Combine(tempPath, fileName);
        var expectedData = Guid.NewGuid().ToString("N");
        File.WriteAllText(fileData, expectedData);
        var newLocation = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        var copyFileData = Path.Combine(newLocation, fileName);

        try
        {
            // act
            sut.DirectoryCopy(tempPath, newLocation);

            // assert
            Assert.True(Directory.Exists(tempPath));
            Assert.True(Directory.Exists(newLocation));
            Assert.Equal(expectedData, File.ReadAllText(fileData));
            Assert.Equal(expectedData, File.ReadAllText(copyFileData));
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            if (Directory.Exists(newLocation))
                Directory.Delete(newLocation, true);
        }
    }
    [Fact]
    public void Should_CopyDirectory_WithSubDir()
    {
        // arrange
        const string fileName = "file.txt";
        const string subDir = "subdir";
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        var tempSubPath = Path.Combine(tempPath, subDir);
        Directory.CreateDirectory(tempPath);
        Directory.CreateDirectory(tempSubPath);
        var fileData = Path.Combine(tempSubPath, fileName);
        var expectedData = Guid.NewGuid().ToString("N");
        File.WriteAllText(fileData, expectedData);
        var newLocation = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        var newTempSubPath = Path.Combine(newLocation, subDir);
        var copyFileData = Path.Combine(newTempSubPath, fileName);

        try
        {
            // act
            sut.DirectoryCopy(tempPath, newLocation);

            // assert
            Assert.True(Directory.Exists(tempPath));
            Assert.True(Directory.Exists(newLocation));
            Assert.Equal(expectedData, File.ReadAllText(fileData));
            Assert.Equal(expectedData, File.ReadAllText(copyFileData));
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            if (Directory.Exists(newLocation))
                Directory.Delete(newLocation, true);
        }
    }
}
