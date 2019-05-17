using System.Collections.Specialized;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
			NameValueCollection collection = new NameValueCollection();
			collection.Add("serverhostkey", program.Config.ReportKey);
			collection.Add("cheatdescription", reason);
			collection.Add("serverhostprooflink", proof);
			collection.Add("serverhoststeamid", steamId64);
			byte[] response = await client.UploadValuesTaskAsync(kReportURL, collection);
			
			return JsonConvert.DeserializeObject<CheaterReportResponse>(Encoding.UTF8.GetString(response)).message;
		}
	}

	public class CheaterReportResponse
	{
		public string message { get; set; }
		public string error { get; set; }
	}
}