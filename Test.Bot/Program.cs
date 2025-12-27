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
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
	if (update.Type != UpdateType.Message) return;

	var msg = update.Message;
	if (msg?.Text is null) return;

	var text = msg.Text.Trim();

	if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
	{
		var firstName = msg.From?.FirstName ?? "друг";
		await bot.SendMessage(
			chatId: msg.Chat.Id,
			text: $"Привет, {firstName}! 🎁\nЯ бот-квест на день рождения.\nКогда придёт время, я пришлю вопросы 😉",
			cancellationToken: ct
		);
		return;
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
