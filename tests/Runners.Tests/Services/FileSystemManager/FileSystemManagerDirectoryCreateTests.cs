using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerDirectoryCreateTests
{
    [Fact]
    public void Should_CreateDirectory()
    {
        // arrange
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            // act
            sut.DirectoryCreate(tempPath);

            // assert
            Assert.True(Directory.Exists(tempPath));
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
