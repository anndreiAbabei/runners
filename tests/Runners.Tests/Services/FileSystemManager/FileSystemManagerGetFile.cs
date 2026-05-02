using Microsoft.Extensions.Logging;
using NSubstitute;
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
        var log = Substitute.For<ILogger<FileSystemManager>>();
        var sut = new FileSystemManager(log);

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
        var log = Substitute.For<ILogger<FileSystemManager>>();
        var sut = new FileSystemManager(log);

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
