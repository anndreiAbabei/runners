using Runners.Persistence;

namespace Runners.Tests.Persistence.Models;

public sealed class RunnerItemPropsTests
{
    [Fact]
    public void Should_InitId()
    {
        // arrange
        const int expectedId = 123;

        // act
        var sut = new RunnerItem
        {
            Id = expectedId,
            Name = null!,
            Token = null!,
            GitUrl = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedId, sut.Id);
    }

    [Fact]
    public void Should_InitName()
    {
        // arrange
        const string expectedName = "runner";

        // act
        var sut = new RunnerItem
        {
            Name = expectedName,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedName, sut.Name);
    }
    
    [Fact]
    public void Should_InitGitUrl()
    {
        // arrange
        const string expectedGitUrl = "https://example.test/repo.git";

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = expectedGitUrl,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedGitUrl, sut.GitUrl);
    }

    [Fact]
    public void Should_InitToken()
    {
        // arrange
        const string expectedToken = "123456test";

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = expectedToken,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedToken, sut.Token);
    }

    [Fact]
    public void Should_InitFolderPath()
    {
        // arrange
        const string expectedFolderPath = "/tmp/repo";

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = expectedFolderPath,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedFolderPath, sut.FolderPath);
    }

    [Fact]
    public void Should_InitCreatedAt()
    {
        // arrange
        const long expectedTicks = 638379936000000000;
        var expectedCreatedAt = new DateTimeOffset(expectedTicks, TimeSpan.Zero);

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = expectedCreatedAt
        };

        // assert
        Assert.Equal(expectedCreatedAt, sut.CreatedAt);
    }

    [Fact]
    public void Should_InitUpdatedAt()
    {
        // arrange
        const long expectedTicks = 638379936000000000;
        var expectedUpdatedAt = new DateTimeOffset(expectedTicks, TimeSpan.Zero);

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            UpdatedAt = expectedUpdatedAt
        };

        // assert
        Assert.Equal(expectedUpdatedAt, sut.UpdatedAt);
    }

    [Fact]
    public void Should_InitState()
    {
        // arrange
        const RunnerState expectedState = RunnerState.Configured;

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            State = expectedState
        };

        // assert
        Assert.Equal(expectedState, sut.State);
    }

    [Fact]
    public void Should_SetState()
    {
        // arrange
        const RunnerState expectedState = RunnerState.Installed;
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            State = default
        };

        // act
        sut.State = expectedState;

        // assert
        Assert.Equal(expectedState, sut.State);
    }

    [Fact]
    public void Should_DefaultStateToAdded()
    {
        // arrange
        const RunnerState expectedState = RunnerState.Added;

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedState, sut.State);
    }

    [Fact]
    public void Should_InitTag()
    {
        // arrange
        const string expectedTag = "alpha";

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            UpdatedAt = null,
            Tag = expectedTag
        };

        // assert
        Assert.Equal(expectedTag, sut.Tag);
    }

    [Fact]
    public void Should_SetTag()
    {
        // arrange
        const string expectedTag = "beta";
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            Tag = "1234"
        };

        // act
        sut.Tag = expectedTag;

        // assert
        Assert.Equal(expectedTag, sut.Tag);
    }

    [Fact]
    public void Should_HaveNullAsDefaultTag()
    {
        // arrange
        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Null(sut.Tag);
    }

    [Fact]
    public void Should_InitDeleted()
    {
        // arrange
        const bool expectedDeleted = true;

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            Deleted = expectedDeleted
        };

        // assert
        Assert.Equal(expectedDeleted, sut.Deleted);
    }

    [Fact]
    public void Should_SetDeleted()
    {
        // arrange
        const bool expectedDeleted = true;
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default,
            Deleted = false
        };

        // act
        sut.Deleted = expectedDeleted;

        // assert
        Assert.Equal(expectedDeleted, sut.Deleted);
    }

    [Fact]
    public void Should_DefaultDeletedToFalse()
    {
        // arrange
        const bool expectedDeleted = false;

        // act
        var sut = new RunnerItem
        {
            Name = null!,
            GitUrl = null!,
            Token = null!,
            FolderPath = null!,
            CreatedAt = default
        };

        // assert
        Assert.Equal(expectedDeleted, sut.Deleted);
    }
}
