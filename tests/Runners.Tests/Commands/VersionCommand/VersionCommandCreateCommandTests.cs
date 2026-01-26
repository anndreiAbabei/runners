using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Runners.Commands;
using SlimeTools.Commander;
using SlimeTools.Commander.Parsing;

namespace Runners.Tests.Commands;

public sealed class VersionCommandCreateCommandTests
{
    [Fact]
    public void Should_CreateSimpleCommand_WithCorrectName()
    {
        // arrange
        var sp = new ServiceCollection().AddSingleton(Substitute.For<ICommandParser>())
                                        .BuildServiceProvider();
        var builder = new CommandBuilder<VersionCommand>(sp);
        const string expectedCommand = "version";
        
        // act
        var command = VersionCommand.CreateCommand(builder);

        // assert
        Assert.Equal(expectedCommand, command.Name);
    }
    
    [Fact]
    public void Should_CreateSimpleCommand_WithDescription()
    {
        // arrange
        var sp = new ServiceCollection().AddSingleton(Substitute.For<ICommandParser>())
                                        .BuildServiceProvider();
        var builder = new CommandBuilder<VersionCommand>(sp);
        var sut = VersionCommand.CreateCommand(builder);
        const string expectedDescription = "Prints out the version of this application.";
        
        // act
        var result = sut.Details;

        // assert
        Assert.NotNull(result);
        Assert.Equal(expectedDescription, result.Description);
    }
}
