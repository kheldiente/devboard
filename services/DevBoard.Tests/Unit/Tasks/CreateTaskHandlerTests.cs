using System.Text.RegularExpressions;
using DevBoard.Application.Features.Tasks.Commands.CreateTask;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace DevBoard.Tests.Unit.Tasks;

public class CreateTaskHandlerTests
{
    private readonly ITaskRepository _repository;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _repository = Substitute.For<ITaskRepository>();
        _handler = new CreateTaskHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewGuid()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new CreateTaskCommand(
            "Fix login bug",
            "Users cannot login",
            Guid.NewGuid(),
            null,
            3);
        
        _repository
            .CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>())
            .Returns(expectedId);

        // act
        var result = await _handler.Handle(command, CancellationToken.None);

        // assert
        result.Should().Be(expectedId);
        await _repository.Received(1)
            .CreateAsync(Arg.Any<TaskItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTaskWithCorrectTitle()
    {
        // arrange
        var command = new CreateTaskCommand(
            "Fix login bug",
            null,
            Guid.NewGuid(),
            null,
            5);
        
        TaskItem? capturedTask = null;
        _repository
            .CreateAsync(Arg.Do<TaskItem>(t => capturedTask = t), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());

        // act
        await _handler.Handle(command, CancellationToken.None);

        // assert
        capturedTask.Should().NotBeNull();
        capturedTask!.Title.Should().Be("Fix login bug");
        capturedTask.StoryPoints.Should().Be(5);
    }
}