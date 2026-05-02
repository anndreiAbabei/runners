using Runners.Services;

namespace Runners.Tests.Services;

public sealed class FileSystemManagerGetFile
{
    [Fact]
    public void Should_GetFile_WhenFileExists()
    {
        // arrange
        var filePath = Path.GetTempFileName();
        var file = File.Create(filePath);
        file.Dispose();
        var sut = new FileSystemManager();

        try
        {
            // act
            using var result = sut.GetFile(filePath, FileMode.Open, FileAccess.Read);

            // assert
            Assert.NotNull(result);
        }
        finally
        {
            file.Dispose();
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
    
    [Fact]
    public void Should_GetNull_WhenFileDoesNotExists()
    {
        // arrange
        var filePath = Path.GetTempFileName();
        if(File.Exists(filePath))
            File.Delete(filePath);
        var sut = new FileSystemManager();

        try
        {
            // act
            using var result = sut.GetFile(filePath, FileMode.Open, FileAccess.Read);

            // assert
            Assert.Null(result);
        }
        finally
        {
            if(File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
