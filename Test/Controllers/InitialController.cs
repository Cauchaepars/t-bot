using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitialController(IConfiguration configuration) : ControllerBase
    {
        [HttpGet("health")]
        public ActionResult Get()
        {
            return Ok();
        }

		[HttpGet("db-ping")]
		public async Task<ActionResult> DbPing()
		{
			var connectionString = configuration.GetConnectionString("Default");

			await using var connection = new NpgsqlConnection(connectionString);
			await connection.OpenAsync();

			await using var command = new NpgsqlCommand("SELECT 1", connection);
			var result = await command.ExecuteScalarAsync();

			return Ok(new
			{
				status = "ok",
				db = result
			});
		}
	}
}
