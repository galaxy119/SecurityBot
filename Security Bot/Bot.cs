using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Security_Bot
{
	public class Bot
	{
		public DiscordSocketClient client
		{
			get
			{
				if (_client == null)
					_client = new DiscordSocketClient();
				return _client;
			}
		}

		private DiscordSocketClient _client;
		public readonly Program program;

		public Bot(Program program)
		{
			this.program = program;
			InitBot().GetAwaiter().GetResult();
		}

		private async Task InitBot()
		{
			client.Log += Program.Log;
			client.MessageReceived += OnMessageReceived;
			await client.LoginAsync(TokenType.Bot, program.config.BotToken);
			await client.StartAsync();
			await Task.Delay(-1);
		}

		private async Task OnMessageReceived(SocketMessage message)
		{
			if (message.Content.StartsWith(program.config.BotPrefix))
			{
				CommandContext context = new CommandContext(client, (IUserMessage) message);
				HandleCommands(context);
			}
		}

		public async Task HandleCommands(CommandContext context)
		{
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "report") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.config.ReportRoleID))
			{
				await DoReportCommand(context);
				Console.WriteLine("handling report");
			}

			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "ban") &&
			    ((context.Message.Author as IGuildUser).RoleIds.Any(p => p == program.config.ReportRoleID)))
			{
				await BanSystem.DoDiscordBanCommand(context);
				Console.WriteLine("handling ban");
			}

			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "reason") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.config.ReportRoleID))
			{
				await BanSystem.DoReasonCmd(context, program);
				Console.WriteLine("handling reason");
			}

			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "baninfo") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.config.ReportRoleID))
			{
				await BanSystem.DoGetInfoCommand(context);
				Console.WriteLine("handling baninfo");
			}

			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "unban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.config.ReportRoleID))
			{
				await BanSystem.DoUnbanCmd(context);
				Console.WriteLine("handling unban");
			}

			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "ping"))
			{
				await context.Channel.SendMessageAsync("pong!");
				Console.WriteLine("handling pong");
			}
		}

		public async Task DoReportCommand(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length < 3)
			{
				await context.Channel.SendMessageAsync(
					$"Invalid arguments. Usage: {program.config.BotPrefix}ban (SteamID64) (proof link) (reason)");
				return;
			}

			if (!ulong.TryParse(args[1], out ulong steamid))
			{
				await context.Channel.SendMessageAsync(
					$"Invalid SteamID64. Usage: {program.config.BotPrefix}ban (SteamID64) (proof link) (reason)");
				return;
			}

			string[] reasonArray = args.Where(p => p != args[0] && p != args[1] && p != args[2]).ToArray();
			string reason = string.Join(' ', reasonArray);
		}
	}
}