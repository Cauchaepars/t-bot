using Application.Users.Commands.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController(ISender sender) : ControllerBase
{
	public sealed record CreateUserRequest(
		long TelegramUserId,
		string? Username,
		string? FirstName,
		string? LastName
	);

	[HttpPost]
	public async Task<ActionResult<CreateUserResult>> Register(CreateUserRequest req, CancellationToken ct)
	{
		var result = await sender.Send(
			new CreateUserCommand(req.TelegramUserId, req.Username, req.FirstName, req.LastName),
			ct
		);

		return Ok(result);
	}
}
