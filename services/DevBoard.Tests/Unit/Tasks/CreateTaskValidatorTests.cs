using DevBoard.Application.Features.Tasks.Commands.CreateTask;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DevBoard.Tests.Unit.Tasks;

public class CreateTaskValidatorTests
{
    private readonly CreateTaskValidator _validator = new();

    [Fact]
    public void Validate_EmptyTitle_ReturnsError()
    {
        var command = new CreateTaskCommand(string.Empty, null, Guid.NewGuid(), null, 3);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_TitleTooLong_RetursnError()
    {
        var command = new CreateTaskCommand(new string ('a', 201), null, Guid.NewGuid(), null, 3);
        var result = _validator.TestValidate(command);
    }

    [Fact]
    public void Validate_NegativeStoryPoints_ReturnsError()
    {
        var command = new CreateTaskCommand("Valid title", null, Guid.NewGuid(), null, -1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.StoryPoints);
    }

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateTaskCommand("Valid title", null, Guid.NewGuid(), null, 3);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}