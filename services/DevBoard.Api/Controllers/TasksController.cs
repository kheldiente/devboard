using DevBoard.Application.DTOs.Tasks;
using DevBoard.Application.Features.Tasks.Commands.CreateTask;
using DevBoard.Application.Features.Tasks.Commands.UpdateTaskStatus;
using DevBoard.Application.Features.Tasks.Queries.GetTasksByBoard;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Jobs;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(IMediator mediator) : ControllerBase
{
    [HttpGet("board/{boardId}")]
    public async Task<IActionResult> GetByBoard(Guid boardId)
    {
        var result = await mediator.Send(new GetTasksByBoardQuery(boardId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.BoardId,
            request.AssigneeId,
            request.StoryPoints
        );
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetByBoard), new { boardId = command.BoardId }, id);
    }

    [HttpPatch("{taskId}/status")]
    public async Task<IActionResult> UpdateStatus(Guid taskId, [FromBody] TaskItemStatus newStatus)
    {
        await mediator.Send(new UpdateTaskStatusCommand(taskId, newStatus));
        return NoContent();
    }

    [HttpPost("{taskId}/notify")]
    public IActionResult TriggerNotification(Guid taskId)
    {
        // fire and forget
        // can be access via http://localhost:5196/hangfire
        BackgroundJob.Enqueue<OverdueTaskScanner>(job => job.RunAsync());
        return Accepted();
    }

}