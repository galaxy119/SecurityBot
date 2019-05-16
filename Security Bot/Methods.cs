using System.Threading.Tasks;

namespace Security_Bot
{
	public class Methods
	{
		private readonly Program program;
		public Methods(Program program) => this.program = program;
		public async Task ExtReq(string steamid, string[] reason)
		{
			program.client.GetGuild(program.guildid).GetTextChannel(program.reqchid).SendMessageAsync(
				"<@" + program.reqrole + ">" + " A ban extension has been requested for " + steamid + " because " +
				reason);
		}
	}
}