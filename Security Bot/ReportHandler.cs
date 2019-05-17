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
		private Program program;
		private WebClient client;
		public const string kReportURL = "https://titnoas.xyz/PepeSmug/reportcheatertest.php";

		public ReportHandler(Program program)
		{
			this.program = program;
			client = new WebClient();
		}

		public async Task<CheaterReportResponse> ReportCheater(string SteamID64, string reason, string proof)
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("serverhostkey", program.config.ReportKey);
			collection.Add("cheatdescription", reason);
			collection.Add("serverhostprooflink", proof);
			collection.Add("serverhoststeamid", SteamID64);
			byte[] response = await client.UploadValuesTaskAsync(kReportURL, collection);
			return JsonConvert.DeserializeObject<CheaterReportResponse>(Encoding.UTF8.GetString(response));
		}
	}

	public class CheaterReportResponse
	{
		public string message { get; set; }
		public string error { get; set; }
	}
}