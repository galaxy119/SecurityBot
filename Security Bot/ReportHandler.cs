using System.Threading.Tasks;

namespace Security_Bot
{
	public class ReportHandler
	{
		private Program program;

		public ReportHandler(Program program)
		{
			this.program = program;
		}

		public async Task ReportCheater(string SteamID64, string reason, string proof)
		{
			
		}
	}
}