using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
	long TelegramUserId,
	string? Username,
	string? FirstName,
	string? LastName
) : IRequest<CreateUserResult>;

public sealed record CreateUserResult(
	Guid UserId,
	long TelegramUserId,
	string? Username,
	string? FirstName,
	string? LastName,
	bool Created
);

public sealed class RegisterFriendCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateUserCommand, CreateUserResult>
{
	public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken ct)
	{
		if (request.TelegramUserId <= 0)
			throw new ArgumentException("TelegramUserId must be > 0");

		var existing = await dbContext.Users
			.FirstOrDefaultAsync(x => x.TelegramUserId == request.TelegramUserId, ct);

		var created = false;

		if (existing is null)
		{
			created = true;
			existing = new User
			{
				TelegramUserId = request.TelegramUserId,
				Username = request.Username,
				FirstName = request.FirstName,
				LastName = request.LastName,
				CreatedAt = DateTimeOffset.UtcNow,
				UpdatedAt = DateTimeOffset.UtcNow
			};

			dbContext.Users.Add(existing);
		}
		else
		{
			existing.Username = request.Username;
			existing.FirstName = request.FirstName;
			existing.LastName = request.LastName;
			existing.UpdatedAt = DateTimeOffset.UtcNow;
		}

		await dbContext.SaveChangesAsync(ct);

		return new CreateUserResult(
			existing.Id,
			existing.TelegramUserId,
			existing.Username,
			existing.FirstName,
			existing.LastName,
			created
		);
	}
}
