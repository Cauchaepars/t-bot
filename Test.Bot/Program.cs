using Application.Users.Commands.CreateUser;
using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

if (string.IsNullOrWhiteSpace(token))
{
	Console.WriteLine("ERROR: TELEGRAM_BOT_TOKEN is not set.");
	return;
}

var botClient = new TelegramBotClient(token);

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions { AllowedUpdates = [] };

botClient.StartReceiving(
	HandleUpdateAsync,
	HandleErrorAsync,
	receiverOptions,
	cts.Token
);

var me = await botClient.GetMe(cts.Token);
Console.WriteLine($"Bot started: @{me.Username} ({me.Id})");
Console.WriteLine("Press Enter to stop.");
await Task.Delay(Timeout.Infinite, cts.Token);


async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
	if (update.Type != UpdateType.Message) return;

	var msg = update.Message;
	if (msg?.Text is null) return;

	var text = msg.Text.Trim();

	if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
	{
		var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";

		using var http = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

		var payload = new
		{
			telegramUserId = msg.From!.Id,
			username = msg.From.Username,
			firstName = msg.From.FirstName,
			lastName = msg.From.LastName
		};

		var res = await http.PostAsJsonAsync("/users", payload, ct);

		if (!res.IsSuccessStatusCode)
		{
			var body = await res.Content.ReadAsStringAsync(ct);
			await bot.SendMessage(msg.Chat.Id, $"Ошибка регистрации 😢\n{body}", cancellationToken: ct);
			return;
		}

		var data = await res.Content.ReadFromJsonAsync<CreateUserResult>(cancellationToken: ct);

		await bot.SendMessage(
			msg.Chat.Id,
			data?.Created == true
				? "Ты зарегистрирован 🎉!"
				: "С возвращением 🙂 Ты уже зарегистрирован.",
			cancellationToken: ct
		);
	}

	await bot.SendMessage(
		chatId: msg.Chat.Id,
		text: "Пока я понимаю только /start 🙂",
		cancellationToken: ct
	);
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
{
	Console.WriteLine($"ERROR: {exception}");
	return Task.CompletedTask;
}
