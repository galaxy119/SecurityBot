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
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "reqgban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await DoGbanCommand(context);
				Console.WriteLine("handling gban");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "shoplift"))
			{
				await context.Channel.SendMessageAsync("Sir, put the item back on the shelf!");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "bloodsugar"))
			{
				await context.Channel.SendMessageAsync(
					"https://tenor.com/view/paul-blart-mall-cop-kevin-james-gif-4287418");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "miranda"))
			{
				await context.Channel.SendMessageAsync(
					"You have the right to remain silent. Anything you say can be used against you in court. You have the right to talk to a lawyer for advice before we ask you any questions. You have the right to have a lawyer with you during questioning. If you cannot afford a lawyer, one will be appointed for you before any questioning if you wish. If you decide to answer questions now without a lawyer present, you have the right to stop answering at any time.");
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
				await BanSystem.DoServerBanCommand(context, program);
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
			
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "warninfo") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await WarnSystem.DoGetInfoCommand(context);
				Console.WriteLine("handling warninfo");
				return;
			}
			
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "warnreason") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await WarnSystem.DoReasonCmd(context, program);
				Console.WriteLine("handling warnreason");
				return;
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "warn") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await WarnSystem.DoServerWarnCommand(context, program);
				Console.WriteLine("handling warn");
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "unwarn") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId))
			{
				await WarnSystem.DoUnWarnCommand(context);
				Console.WriteLine("handling unwarn");
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "report"))
			{
				await DoReportCommand(context);
				Console.WriteLine("handling report");

			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "bug"))
			{
				await DoBugRepCommand(context);
				Console.WriteLine("handling bug report");
			}
			
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "recommend") ||
			    context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "suggest"))
			{
				await DoRecommend(context);
				Console.WriteLine("handling bug report");
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "dwarn") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.DiscStaffId))
			{
				await WarnSystem.DoDiscordWarnCommand(context, program);
				Console.WriteLine("handling discord warn");
			}
			
			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "dban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.DiscStaffId))
			{
				await BanSystem.DoDiscordBanCommand(context, program);
				Console.WriteLine("handling discord ban");
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "dkick") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.DiscStaffId))
			{
				await BanSystem.DoDiscordBanCommand(context, program, true);
				Console.WriteLine("handling discord kick");
			}

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "dsoftban") &&
			    ((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.DiscStaffId))
			{
				await BanSystem.DoDiscordBanCommand(context, program, false, true);
				Console.WriteLine("handling softban");
			}
				

			if (context.Message.Content.ToLower().StartsWith(program.Config.BotPrefix + "help"))
			{
				string response;
				if (((IGuildUser) context.Message.Author).RoleIds.Any(p => p == program.Config.ReportRoleId) && !context.Message.Content.ToLower().Contains("discord"))
					response = "```md\n" + "Command prefix: ~ \n" +
					           "< report > <ServerNumber> <PlayerName> <Reason> - Reports a player on the SCP server to the SCP Staff. \n" +
					           "\n" +
					           "< bug > <ServerNumber> <Description> - Reports a server bug to the Server Manager. \n" +
					           "\n" +
					           "< suggest/recommend > <Description> - Submits a recommendation in #scp-recommendations. \n" +
					           "\n" + 
					           "< ban > <SteamID> <Duration> - Bans a player from the SCP server. \n" + "\n" +
					           "< baninfo > <SteamID> - Gives information about any bans on a player. \n" + "\n" +
					           "< unban > <SteamID> - Unbans a player. \n" + "\n" +
					           "< reason > <SteamID> <Reason> - Adds a reason to a player's current ban. Accepts rule# shortcodes in this format: [#] \n" +
					           "\n" +
					           "< warn > <SteamID> <Reason> - Warns a player on the SCP server Accepts Rule# shortcodes in this format: [#] \n" +
					           "\n" + "< warninfo > <SteamID> - Lists all active warnings for a player. \n" + "\n" +
					           "< unwarn > <WarningID> - Removes a warning. WarningID is the ID: number printed in warninfo. \n" +
					           "\n" +
					           "< reqgban > <SteamID> <Proof> <Description> - Submits a proof to the Global Moderation team to request a Game Ban for the specified SteamID.```";
				else if (((IGuildUser)context.Message.Author).RoleIds.Any(p => p == program.Config.DiscStaffId) || context.Message.Content.ToLower().Contains("discord"))
					response = "```md\n" + "Command prefix: ~ \n" +
					           "< report > <ServerNumber> <PlayerName> <Reason> - Reports a player on the SCP server to the SCP Staff. \n" +
					           "\n" +
					           "< bug > <ServerNumber> <Description> - Reports a server bug to the Server Manager. \n" +
					           "\n" +
					           "< suggest/recommend > <Description> - Submits a recommendation in #scp-recommendations. \n" +
					           "\n" + 
					           "< dban > <Username> - Bans a player from the Discord server. \n" + "\n" +
					           "< baninfo > <Username> - Gives information about any bans on a player. \n" + "\n" +
					           "< unban > <username> - Unbans a player. \n" + "\n" +
					           "< dkick > <Username> - Kicks a user from the server. \n" + "\n" +
					           "< dsoftban > <Username> - Kicks a user from the server, but removes all of their recent messages aswell. \n" + "\n" +
					           "< dwarn > <Username> <Reason> - Warns a player on the Discord server. \n" +
					           "\n" + "< warninfo > <Username> - Lists all active warnings for a player. \n" + "\n" +
					           "< unwarn > <WarningID> - Removes a warning. WarningID is the ID: number printed in warninfo. ```";
				else
					response = "```md\n" + "Command prefix: ~ \n" +
					           "< report > <ServerNumber> <PlayerName> <Reason> - Reports a player on the SCP server to the SCP Staff. \n" +
					           "\n" +
					           "< bug > <ServerNumber> <Description> - Reports a server bug to the Server Manager. \n" +
					           "\n" +
					           "< suggest/recommend > <Description> - Submits a recommendation in #scp-recommendations.```";
				await context.Channel.SendMessageAsync(response);
				Console.WriteLine("handling help");
			}

		}

		private static async Task DoRecommend(ICommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] { " " }, StringSplitOptions.None);
			if (args.Length < 2)
			{
				await context.Channel.SendMessageAsync($"Invalid description. Please provide one.");
				return;
			}

			string[] descarray = args.Where(p => p != args[0]).ToArray();
			string desc = string.Join(' ', descarray);
			string response = await ReportHandler.Recommendations(context, desc);
			await context.Channel.SendMessageAsync(response);
		}

		private async Task DoReportCommand(ICommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] { " " }, StringSplitOptions.None);
			if (args.Length < 3)
			{
				await context.Channel.SendMessageAsync(
					$"Invalid arguments. Usage: {program.Config.BotPrefix}report (servernumber) (playername) (reason)");
				return;
			}

			if (!int.TryParse(args[1], out int _))
			{
				await context.Channel.SendMessageAsync(
					$"Invalid server number. Usage: {program.Config.BotPrefix}report (ServerNumber) (PlayerName) (Reason)");
				return;
			}

			string[] reasonarray = args.Where(p => p != args[0] && p != args[1] && p != args[2]).ToArray();
			string reason = string.Join(' ', reasonarray);
			string response = await ReportHandler.ReportPlayer(context, args[1], args[2], reason);
			await context.Channel.SendMessageAsync(response);
		}

		private async Task DoBugRepCommand(ICommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] { " " }, StringSplitOptions.None);
			if (args.Length < 2)
			{
				await context.Channel.SendMessageAsync(
					$"Invalid Arguments. Usage: {program.Config.BotPrefix}bug (ServerNumber) (Description)");
				return;
			}

			if (!int.TryParse(args[1], out int _))
			{
				await context.Channel.SendMessageAsync(
					$"Invalid server number. Usage: {program.Config.BotPrefix}bug (ServerNumber) (Description)");
				return;
			}

			string[] descriptionarray = args.Where(p => p != args[0] && p != args[1]).ToArray();
			string description = string.Join(' ', descriptionarray);
			string response = await ReportHandler.ReportBug(context, args[1], description);
			await context.Channel.SendMessageAsync(response);
		}

		private async Task DoGbanCommand(ICommandContext context)
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