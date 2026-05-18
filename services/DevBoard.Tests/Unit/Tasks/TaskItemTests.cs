using DevBoard.Domain.Entities;
using FluentAssertions;

namespace DevBoard.Tests.Unit.Tasks;

public class TaskItemTests
{
    [Fact]
    public void Transition_TodoToInProgress_Succeeds()
    {
        // arrange
        var task = new TaskItem { Status = TaskItemStatus.Todo };

        // act
        task.Transition(TaskItemStatus.InProgress);

        // assert
        task.Status.Should().Be(TaskItemStatus.InProgress);
    }

    [Fact]
    public void Transition_TodoToDone_ThrowsInvalidOperation()
    {
        // arrange
        var task = new TaskItem { Status = TaskItemStatus.Todo };

        // act
        var act = () => task.Transition(TaskItemStatus.Done);

        // assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*");
    }

    [Fact]
    public void Transition_InProgressToDone_Succeeds()
    {
        // arrange
        var task = new TaskItem { Status = TaskItemStatus.InProgress };

        // act
        task.Transition(TaskItemStatus.Done);

        // assert
        task.Status.Should().Be(TaskItemStatus.Done);
    }

    [Fact]
    public void Transition_DoneToInProgerss_ThrowsInvalidOperation()
    {
        // arrange
        var task = new TaskItem { Status = TaskItemStatus.Done };

        // act
        var act = () => task.Transition(TaskItemStatus.InProgress);

        // assert

        act.Should().Throw<InvalidOperationException>();
    }

}