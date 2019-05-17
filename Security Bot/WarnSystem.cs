using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Discord.Commands;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Discord;

namespace Security_Bot
{
	public class WarnSystem
	{
		private static WebClient _webClient;

		public static async Task DoDiscordWarnCommand(CommandContext context, Program program)
		{
			TimeSpan span;
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			// await context.Channel.SendMessageAsync(Convert.ToString(args.Length));
			if (args.Length < 2) return;
			if (args[1].Length != 17 || !long.TryParse(args[1], out long l)) return;
			if (args[2].ToUpper() == "PERM")
				span = TimeSpan.FromDays(18250);
			else
			{
				string humanReadableDuration = "";
				string toParse = args.Where(p => p == args[2]).Aggregate("", (current, s) => current + " " + s);

				span = ParseBanDuration(toParse, ref humanReadableDuration);
				if (span.Ticks <= 0)
				{
					await context.Channel.SendMessageAsync("Error while parsing the ban duration.");
					return;
				}
			}

			string[] reasonarray = args.Where(p => p != args[0] && p != args[1] && p != args[2]).ToArray();
			string reason = string.Join(' ', reasonarray);
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
			coll.Add("reason", reason);
			//await context.Channel.SendMessageAsync("asdasdasd");
			string apikey = "4C3DD54D9D92C18291F05FF2F59639A4";
			await DownloadwebClient(out string result, "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apikey + "&steamids=" + args[1]);
			RootObject players = JsonConvert.DeserializeObject<RootObject>(result);
			Player player = players.response.players.FirstOrDefault();
			if (player == null)
			{
				await context.Channel.SendMessageAsync("ERROR Player is null.");
				return;
			}

			coll.Add("username", player.personaname);
			if (reason.Length <= 0)
			{
				await context.Channel.SendMessageAsync("You must supply a warning reason.");
				return;
			}
			await Upload(coll, out string response, "http://joker.hivehosted.com/Warns/WarnLogs.php");
			await context.Channel.SendMessageAsync(response);
			if (response.StartsWith("Warn added"))
			{
				var chan = context.Guild.GetTextChannelAsync(516375236212555779).Result;
				await chan.SendMessageAsync("+p " + player.personaname + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
				await chan.SendMessageAsync("-p " + player.personaname + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
				await chan.SendMessageAsync(".p " + player.personaname + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
			}
		}

		private static Task DownloadwebClient(out string result, string url)
		{
			using (var webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
				Console.WriteLine("client download");
				result = webClient.DownloadString(url);
				Console.WriteLine("Downloaded");
			}
			return Task.FromResult(1);
		}

		public static async Task DoGetInfoCommand(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			if (!long.TryParse(args[1], out long steamid64) || args[1].Length != 17) return;
			NameValueCollection collection = new NameValueCollection
			{
				{ "apikey", "vyJCxUBi9D785DuXxAVoDuD1llZ5jPO4T4YdvfLYNk2Qe2wyHNZVZOJoQ5rXTE97" },
				{ "steamid64", args[1] }
			};
			if (_webClient == null) _webClient = new WebClient();
			await Upload(collection, out string response, "http://joker.hivehosted.com/Warns/GetWarnInfo.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoUnWarnCommand(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			Console.WriteLine("Hi");
			NameValueCollection collection = new NameValueCollection
			{
				{ "apikey", "Ntba7mQeAyKRsoZs6qe85t37npzsyLUNPaUc6CRzKcNWgKUp470ja91DX7wioIUH" },
				{ "index", args[1] }
			};
			Console.WriteLine(collection.Get("index"));
			if (_webClient == null) _webClient = new WebClient();
			await Upload(collection, out string response, "http://joker.hivehosted.com/Warns/RevokeWarn.php");
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoReasonCmd(CommandContext context, Program program, string time = null)
		{
			//if (context.Channel.Id != ulong.Parse("536784180999356416")) return;
			//if (!context.Message.Content.ToLower().StartsWith(")reason")) return;

			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length <= 2) return;
			string steamid = args[1];
			if (!ulong.TryParse(steamid, out ulong steamid64ulong) || steamid.Length != 17) return;
			string reason = args.Where(p => ((p != program.Config.BotPrefix + "reason") && (p != steamid))).Aggregate("", (current, str) => current + str + " ");

			Console.WriteLine(reason);
			NameValueCollection col = new NameValueCollection
			{
				{"apikey", "npN8L6plU6FtxK8KzmXd0XoM6TqIEilibHZqreIK0lXuf6KJ8O9G2o6Wn6t1emtp"},
				{"reason", reason},
				{"steamid64", steamid},
				{"time", time}
			};
			if (_webClient == null) _webClient = new WebClient();
			await Upload(col, out string response, "http://joker.hivehosted.com/Warns/UpdateReason.php");
			await context.Channel.SendMessageAsync(response);
		}

		private static Task Upload(NameValueCollection col, out string response, string url)
		{
			if (_webClient == null) _webClient = new WebClient();
			byte[] byteresponse = _webClient.UploadValues(url, col);
			response = Encoding.Default.GetString(byteresponse);
			return Task.FromResult(1);
		}

		private static TimeSpan ParseBanDuration(string duration, ref string humanReadableDuration)
		{
			string[] parts = duration.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
			TimeSpan span = new TimeSpan();
			foreach (string s in parts)
			{
				string digits = new string(s.ToCharArray().Where(char.IsDigit).ToArray());
				if (!int.TryParse(digits, out int result)) 
					return TimeSpan.MinValue;

				char type = s.ToCharArray().FirstOrDefault(char.IsLetter);
				Console.WriteLine(type + "  " + digits);
				if (type == default(char)) 
					return TimeSpan.MinValue;

				span += FromChar(type, result);
			}

			return span;
		}


		private static TimeSpan FromChar(char c, int duration)
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