using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using NSubstitute;
using Runners.Persistence;
using Runners.Services;

namespace Runners.Tests.Services;

public sealed class AppSettingsManagerWriteTests
{
    [Fact]
    public async Task Should_DoNothing_IfFileCannotBeCreated()
    {
        // arrange
        var fs = Substitute.For<IFileSystemManager>();
        fs.GetFile(AppSettingsManager.AppSettingsFileName, FileMode.OpenOrCreate, FileAccess.Write)
          .Returns((Stream?)null);
        var sut = new AppSettingsManager(fs);

        // act
        var ex = await Record.ExceptionAsync(async () => await sut.Write(new AppSettings()));

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public async Task Should_WriteAppSettings_ToFileAsJson()
    {
        // arrange
        var ms = new MemoryStream();
        var stream = new TestStream(ms);
        var fs = Substitute.For<IFileSystemManager>();
        fs.GetFile(AppSettingsManager.AppSettingsFileName, FileMode.OpenOrCreate, FileAccess.Write)
          .Returns(stream);
        var sut = new AppSettingsManager(fs);
        var expectedAppSettings = new AppSettings
        {
            RunnersFolder = "test"
        };

        // act
        await sut.Write(expectedAppSettings);

        // assert
        ms.Seek(0, SeekOrigin.Begin);
        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(ms);
        Assert.Equal(expectedAppSettings, settings, AppSettingsComparer.Default);
        
        stream.RealDispose();
    }
    
    [ExcludeFromCodeCoverage]
    private sealed class TestStream(MemoryStream ms) : Stream
    {
        private readonly Stream _streamImplementation = ms;
        
        public override bool CanRead => _streamImplementation.CanRead;
        public override bool CanSeek => _streamImplementation.CanSeek;
        public override bool CanWrite => _streamImplementation.CanWrite;
        public override long Length => _streamImplementation.Length;
        public override long Position
        {
            get => _streamImplementation.Position;
            set => _streamImplementation.Position = value;
        }

        public override void Flush() => _streamImplementation.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _streamImplementation.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _streamImplementation.Seek(offset, origin);
        public override void SetLength(long value) => _streamImplementation.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _streamImplementation.Write(buffer, offset, count);

        protected override void Dispose(bool disposing) {/*DO NOTHING*/}
        internal void RealDispose() => _streamImplementation.Dispose();
    }
}
