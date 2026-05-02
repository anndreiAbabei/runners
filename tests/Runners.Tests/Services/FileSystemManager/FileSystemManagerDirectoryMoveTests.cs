using Microsoft.Extensions.Logging;
using NSubstitute;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerDirectoryMoveTests
{
    [Fact]
    public void Should_MoveDirectory()
    {
        // arrange
        const string fileName = "file.txt";
        var log = Substitute.For<ILogger<FileSystemManager>>();
        var sut = new FileSystemManager(log);
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);
        var fileData = Path.Combine(tempPath, fileName);
        var expectedData = Guid.NewGuid().ToString("N");
        File.WriteAllText(fileData, expectedData);
        var newLocation = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        fileData = Path.Combine(newLocation, fileName);

        try
        {
            // act
            sut.DirectoryMove(tempPath, newLocation);

            // assert
            Assert.False(Directory.Exists(tempPath));
            Assert.True(Directory.Exists(newLocation));
            Assert.Equal(expectedData, File.ReadAllText(fileData));
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
