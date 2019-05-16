using System.Threading.Tasks;
using Discord.Commands;

namespace Security_Bot
{
	public class InfoModule : ModuleBase<SocketCommandContext>
	{
		// ~say hello world -> hello world
		[Command("say"), Summary("Echoes a message.")]
		public Task SayAsync([Remainder, Summary("The text to echo")]
			string echo) =>
			ReplyAsync(echo);
	}

	public class PongModule : ModuleBase<SocketCommandContext>
	{
		[Command("ping"), Summary("Pongs you.")]
		public Task PongAsync() => ReplyAsync("Pong!");
	}

	public class BanModule : ModuleBase<SocketCommandContext>
	{
		private readonly Rest rest = new Rest();

		[Command("ban"), Summary("Bans someone. hehexd")]
		public async Task BanAsync(string steamid, string link, string reason)
		{
			string response = await rest.RestPost(steamid, link, reason);
			await ReplyAsync(response);
		}
	}
}