using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Discord.Commands;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Security_Bot
{
	public class BanSystem
	{
		public static WebClient webClient;

		public static async Task DoDiscordBanCommand(CommandContext context)
		{
			TimeSpan span;
			var args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			// await context.Channel.SendMessageAsync(Convert.ToString(args.Length));
			if (args.Length < 2) return;
			if (args[1].Length != 17 || !long.TryParse(args[1], out long l)) return;
			if (args[2].ToUpper() == "PERM")
			{
				span = TimeSpan.FromDays(18250);
			}
			else
			{
				string humanReadableDuration = "";
				string toParse = "";
				foreach (string s in args.Where(p => p != args[1] && p != args[0]))
				{
					toParse = toParse + " " + s;
				}

				span = ParseBanDuration(toParse, ref humanReadableDuration);
				if (span.Ticks <= 0)
				{
					await context.Channel.SendMessageAsync("Error while parsing the ban duration.");
					return;
				}
			}
			// await context.Channel.SendMessageAsync("1");


			// await context.Channel.SendMessageAsync("asdasdasd");
			NameValueCollection coll = new NameValueCollection();

			coll.Add("apikey", "4qj3Mk0e49J78W3IU$9&0yQ7z48x1420R6tST126d3oLX76449030756o1268922");
			coll.Add("server", "N/A");
			coll.Add("timestamp",
				DateTime.UtcNow.Year.ToString("0000") + "-" + DateTime.UtcNow.Month.ToString("00") + "-" +
				DateTime.UtcNow.Day.ToString("00") + "T" + DateTime.UtcNow.Hour.ToString("00") + ":" +
				DateTime.UtcNow.Minute.ToString("00") + ":" + DateTime.UtcNow.Second + "." +
				DateTime.UtcNow.Millisecond.ToString("000"));
			coll.Add("starttime", Convert.ToString(DateTime.UtcNow.Ticks));
			coll.Add("expiretime", Convert.ToString(span.Ticks));
			coll.Add("color", (span.Days > 365) ? "16580608" : "1208580");
			coll.Add("admin", context.Message.Author.Username);
			coll.Add("steamid64", args[1]);
			coll.Add("duration",
				(span.Days >= 18250)
					? "Permanent"
					: (span.Days + " days, " + span.Hours + " hours, and " + span.Minutes +
					   ((span.Minutes != 1) ? " minutes." : " minute.")));
			coll.Add("Discord", "Discord");
			//await context.Channel.SendMessageAsync("asdasdasd");
			string apikey = "4C3DD54D9D92C18291F05FF2F59639A4";
			await downloadwebClient(out string result,
				"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apikey + "&steamids=" +
				args[1]);
			var players = JsonConvert.DeserializeObject<RootObject>(result);
			var player = players.response.players.FirstOrDefault();
			if (player == null)
			{
				await context.Channel.SendMessageAsync("ERROR Player is null.");
				return;
			}

			coll.Add("username", player.personaname);
			await upload(coll, out string response, "http://joker.hivehosted.com/Bans/BanLogs.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static Task downloadwebClient(out string result, string url)
		{
			result = webClient.DownloadString(url);
			return Task.FromResult(1);
		}

		public static async Task DoGetInfoCommand(CommandContext context)
		{
			var args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			if (!long.TryParse(args[1], out long steamid64) || args[1].Length != 17) return;
			NameValueCollection collection = new NameValueCollection();
			collection.Add("apikey", "vyJCxUBi9D785DuXxAVoDuD1llZ5jPO4T4YdvfLYNk2Qe2wyHNZVZOJoQ5rXTE97");
			collection.Add("steamid64", args[1]);
			if (webClient == null) webClient = new WebClient();
			await upload(collection, out string response, "http://joker.hivehosted.com/Bans/GetBanInfo.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoUnbanCmd(CommandContext context)
		{
			var args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			if (!long.TryParse(args[1], out long steamid64) || args[1].Length != 17) return;
			NameValueCollection collection = new NameValueCollection();
			collection.Add("apikey", "Ntba7mQeAyKRsoZs6qe85t37npzsyLUNPaUc6CRzKcNWgKUp470ja91DX7wioIUH");
			collection.Add("steamid64", args[1]);
			if (webClient == null) webClient = new WebClient();
			await upload(collection, out string response, "http://joker.hivehosted.com/Bans/RevokeBan.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoReasonCmd(CommandContext context, Program program)
		{
			//if (context.Channel.Id != ulong.Parse("536784180999356416")) return;
			//if (!context.Message.Content.ToLower().StartsWith(")reason")) return;

			var args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length <= 2) return;
			var steamid = args[1];
			if (!ulong.TryParse(steamid, out ulong steamid64ulong) || steamid.Length != 17) return;
			var reason = "";
			foreach (string str in args.Where(p => ((p != program.config.BotPrefix + "reason") && (p != steamid))))
			{
				reason = reason + str + " ";
			}

			Console.WriteLine(reason);
			NameValueCollection col = new NameValueCollection
			{
				{"apikey", "npN8L6plU6FtxK8KzmXd0XoM6TqIEilibHZqreIK0lXuf6KJ8O9G2o6Wn6t1emtp"},
				{"reason", reason},
				{"steamid64", steamid}
			};
			if (webClient == null) webClient = new WebClient();
			await upload(col, out string response, "http://joker.hivehosted.com/Bans/UpdateReason.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static Task upload(NameValueCollection col, out string response, string url)
		{
			if (webClient == null) webClient = new WebClient();
			var byteresponse = webClient.UploadValues(url, col);
			response = Encoding.Default.GetString(byteresponse);
			return Task.FromResult(1);
		}

		private static TimeSpan ParseBanDuration(string duration, ref string humanReadableDuration)
		{
			string[] parts = duration.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
			TimeSpan span = new TimeSpan();
			foreach (string s in parts)
			{
				string digits = new string(s.ToCharArray().Where(ch => Char.IsDigit(ch)).ToArray());
				if (!int.TryParse(digits, out int result))
				{
					return TimeSpan.MinValue;
				}

				char type = s.ToCharArray().Where(ch => Char.IsLetter(ch)).FirstOrDefault();
				Console.WriteLine(type + "  " + digits);
				if (type == default(char))
				{
					return TimeSpan.MinValue;
				}

				span += fromChar(type, result);
			}

			return span;
		}


		private static TimeSpan fromChar(char c, int duration)
		{
			switch (c)
			{
				case 's':
					return TimeSpan.FromSeconds(duration);
				case 'm':
					return TimeSpan.FromMinutes(duration);
				case 'h':
					return TimeSpan.FromHours(duration);
				case 'd':
					return TimeSpan.FromDays(duration);
				case 'w':
					return TimeSpan.FromDays(duration * 7);
				case 'M':
					return TimeSpan.FromDays(duration * 30);
				case 'y':
					return TimeSpan.FromDays(duration * 365);
				default:
					return TimeSpan.MinValue;
			}
		}
	}
}