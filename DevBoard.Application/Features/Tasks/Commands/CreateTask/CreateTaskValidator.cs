using FluentValidation;

namespace DevBoard.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        
        RuleFor(x => x.BoardId)
            .NotEmpty().WithMessage("BoardId is required.");

        RuleFor(x => x.StoryPoints)
            .GreaterThanOrEqualTo(0).WithMessage("Story points cannot be negative.")
            .LessThanOrEqualTo(100).WithMessage("Story points cannot exceeed 100.");
    }
}