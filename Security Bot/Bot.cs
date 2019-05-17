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
			if (context.Message.Content.ToLower().StartsWith(program.config.BotPrefix + "ban") && ((IGuildUser) context.Message.Author).RoleIds.Contains(program.config.ReportRoleID))
			{
				
			}
		}
	}
}