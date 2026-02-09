using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerDirectoryExistsTests
{
    [Fact]
    public void Should_ReturnTrue_WhenDirectoryExists()
    {
        // arrange
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);

        try
        {
            // act
            var result = sut.DirectoryExists(tempPath);

            // assert
            Assert.True(result);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, false);
            }
        }
    }

    [Fact]
    public void Should_ReturnFalse_WhenDirectoryDoesNotExist()
    {
        // arrange
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            // act
            var result = sut.DirectoryExists(tempPath);

            // assert
            Assert.False(result);
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, false);
            }
        }
    }
}
