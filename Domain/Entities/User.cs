namespace Domain.Entities;

public class User
{
	public Guid Id { get; init; } = Guid.NewGuid();

	public long TelegramUserId { get; set; }

	public string? Username { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

