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
				if(_client == null)
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
				CommandContext context = new CommandContext(client, (IUserMessage)message);
				HandleCommands(context);
			}
		}

		public async Task HandleCommands(CommandContext context)
		{
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "report") && ((IGuildUser) context.Message.Author).RoleIds.Any(p => program.config.ReportRoleIDs.Contains(p.ToString())))
			{
				DoReportCommand(context);
			}
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "ban") && ((IGuildUser) context.Message.Author).RoleIds.Any(p => program.config.ReportRoleIDs.Contains(p.ToString())))
			{
				BanSystem.DoDiscordBanCommand(context);
			}
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "reason") && ((IGuildUser) context.Message.Author).RoleIds.Any(p => program.config.ReportRoleIDs.Contains(p.ToString())))
			{
				BanSystem.DoReasonCmd(context);
			}
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "baninfo") && ((IGuildUser) context.Message.Author).RoleIds.Any(p => program.config.ReportRoleIDs.Contains(p.ToString())))
			{
				BanSystem.DoGetInfoCommand(context);
			}
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "unban") && ((IGuildUser) context.Message.Author).RoleIds.Any(p => program.config.ReportRoleIDs.Contains(p.ToString())))
			{
				BanSystem.DoUnbanCmd(context);
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