using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Schema;
using Newtonsoft.Json;
using RestSharp;

namespace Security_Bot
{
	public class Rest
	{
		private readonly RestClient rest = new RestClient("https://staff.scpslgame.com/api/rest/reportcheater.php");
		private const string key = "[REDACTED]";

		public async Task<string> RestPost(string steamid, string link, string reason)
		{
			RestRequest request = new RestRequest("resource/{id}", Method.POST);
			request.AddParameter("serverhostkey", key);
			request.AddParameter("cheatdescription", reason);
			request.AddParameter("serverhostprooflink", link);
			request.AddParameter("serverhoststeamid", steamid);
			IRestResponse response = null;

			rest.ExecuteAsync(request, resp =>
			{
				Console.WriteLine(resp.Content);
				response = resp;
			});

			await Task.Delay(1000);
			Message msg = JsonConvert.DeserializeObject<Message>(response.Content);
			string message = msg.message;

			return message;
		}
	}

	internal class Message
	{
		public string message { get; set; }
		public int error { get; set; }
	}
}