using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RestSharp;

namespace Security_Bot
{
	public class ReportHandler
	{
		private readonly Program program;
		private readonly WebClient client;
		private const string kReportURL = "https://staff.scpslgame.com/api/rest/reportcheater.php";

		public ReportHandler(Program program)
		{
			this.program = program;
			client = new WebClient();
		}

		public async Task<string> ReportCheater(string steamId64, string proof, string reason)
		{
			NameValueCollection collection = new NameValueCollection
			{
				{ "serverhostkey", program.Config.ReportKey },
				{ "cheatdescription", reason },
				{ "serverhostprooflink", proof },
				{ "serverhoststeamid", steamId64 }
			};
			byte[] response = await client.UploadValuesTaskAsync(kReportURL, collection);
			
			return JsonConvert.DeserializeObject<CheaterReportResponse>(Encoding.UTF8.GetString(response)).message;
		}
		
		public static async Task<string> ReportPlayer(ICommandContext context, string server, string playername, string reason)
		{
			IUser reporter = context.Message.Author;
			string message = "<@&540272592167370764> A player report has been filed! \n" + "Reporter: " + reporter.Username + "\n" +
			                 "Reportee: " + playername + "\n" + "Server: Playground " + server + "\n" + "Reason: " +
			                 reason;
			ITextChannel chan = context.Guild.GetTextChannelAsync(533883038468145154).Result;
			
			await chan.SendMessageAsync(message);

			return "<@" + reporter.Id + "> Your report has been received and will be reviewed by staff shortly.";
		}

		public static async Task<string> ReportBug(ICommandContext context, string server, string description)
		{
			IUser reporter = context.Message.Author;
			string message = "<@&530252029361127439> A bug report has been filed! \n" + "Reporter: " + reporter.Username + "\n" +
			                 "Server: Playground " + server + "\n" + "Description: " + description;
			ITextChannel chan = context.Guild.GetTextChannelAsync(526314073491767303).Result;

			await chan.SendMessageAsync(message);

			return "<@" + reporter.Id +
			       "> Your bug report has been received and joker will ~~break~~ fix the servers shortly.";
		}
		
		public static async Task<string> Recommendations(ICommandContext context, string description)
		{
			IUser reporter = context.Message.Author;
			string message = "<@&530252029361127439> A recommendation has been submitted! \n" + "Reporter: " + reporter.Username + "\n" + "Description: " + description;
			ITextChannel chan = context.Guild.GetTextChannelAsync(526450520869699584).Result;

			await chan.SendMessageAsync(message);

			return "<@" + reporter.Id +
			       "> Your recommendation has been submitted, the community will review and ~~browbeat~~ critique your idea in #recommendations-discussion.";
		}
	}

	public class CheaterReportResponse
	{
		public string message { get; set; }
		public string error { get; set; }
	}
}