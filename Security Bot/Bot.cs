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
		private DiscordSocketClient Client => client ?? (client = new DiscordSocketClient());

		private DiscordSocketClient client;
		private readonly Program program;

		public Bot(Program program)
		{
			this.program = program;
			InitBot().GetAwaiter().GetResult();
		}

		private async Task InitBot()
		{
			Client.Log += Program.Log;
			Client.MessageReceived += OnMessageReceived;
			await Client.LoginAsync(TokenType.Bot, program.Config.BotToken);
			await Client.StartAsync();
			await Task.Delay(-1);
		}

		private async Task OnMessageReceived(SocketMessage message)
		{
			if (message.Content.StartsWith(program.Config.BotPrefix))
			{
				CommandContext context = new CommandContext(Client, (IUserMessage) message);
				HandleCommands(context);
			}
		}

		private async Task HandleCommands(CommandContext context)
		{
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "report") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await DoReportCommand(context);
				Console.WriteLine("handling report");
				return;
			}
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "baninfo") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await BanSystem.DoGetInfoCommand(context);
				Console.WriteLine("handling baninfo");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "ban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await BanSystem.DoDiscordBanCommand(context);
				Console.WriteLine("handling ban");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "reason") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await BanSystem.DoReasonCmd(context, program);
				Console.WriteLine("handling reason");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "unban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await BanSystem.DoUnbanCmd(context);
				Console.WriteLine("handling unban");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "ping"))
			{
				await context.Channel.SendMessageAsync("pong!");
				Console.WriteLine("handling pong");
			}
		}

		private async Task DoReportCommand(ICommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length < 3)
			{
				await context.Channel.SendMessageAsync(
					$"Invalid arguments. Usage: {program.Config.BotPrefix}ban (SteamID64) (proof link) (reason)");
				return;
			}

			if (!ulong.TryParse(args[1], out ulong steamid))
			{
				await context.Channel.SendMessageAsync(
					$"Invalid SteamID64. Usage: {program.Config.BotPrefix}ban (SteamID64) (proof link) (reason)");
				return;
			}

			string[] reasonArray = args.Where(p => p != args[0] && p != args[1] && p != args[2]).ToArray();
			string reason = string.Join(' ', reasonArray);
			string response = await program.ReportHandler.ReportCheater(steamid.ToString(), args[2], reason);
			await context.Channel.SendMessageAsync(response);
		}
	}
}