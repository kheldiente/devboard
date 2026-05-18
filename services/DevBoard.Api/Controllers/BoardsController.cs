using DevBoard.Application.Features.Boards.Queries.GetAllBoards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BoardsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllBoardsQuery());
        return Ok(result);
    }
}