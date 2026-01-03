using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

		var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default");

		if (string.IsNullOrWhiteSpace(cs))
		{ 
			throw new InvalidOperationException("Set ConnectionStrings__Default environment variable before running EF migrations."); 
		}

		optionsBuilder.UseNpgsql(cs);

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
