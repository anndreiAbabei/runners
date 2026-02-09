using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerDirectoryDeleteTests
{
    [Fact]
    public void Should_DeleteDirectory()
    {
        // arrange
        var sut = new FileSystemManager();
        var tempPath = Path.Combine(Path.GetTempPath(), "Runners.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);

        try
        {
            // act
            sut.DirectoryDelete(tempPath, false);

            // assert
            Assert.False(Directory.Exists(tempPath));
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
