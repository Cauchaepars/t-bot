using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(x => x.Id);
		builder.HasIndex(x => x.TelegramUserId).IsUnique();

		builder.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
		builder.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
		builder.Property(e => e.Username).HasMaxLength(64);
		builder.Property(e => e.FirstName).HasMaxLength(128);
		builder.Property(e => e.LastName).HasMaxLength(128);
	}
}
