using System;
using System.Collections.Generic;
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
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length < 2) return;
			TimeSpan span = TimeSpan.FromDays(18250);

			string[] reasonarray = args.Where(p => p != args[0] && p != args[1]).ToArray();
			string reason = string.Join(' ', reasonarray);

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
			coll.Add("Discord", "Discord");
			coll.Add("reason", reason);

			string name = args[1];
			if (args[1].Contains("@"))
			{
				name = args[1].Replace("@", "");
				name = name.Replace("<", "");
				name = name.Replace(">", "");
				if (name.Contains("!"))
					name = name.Replace("!", "");
			}
			IEnumerable<IGuildUser> users = context.Guild.GetUsersAsync(CacheMode.CacheOnly).Result.Where(u => ulong.TryParse(name, out ulong result) ? u.Id == result : u.Username == name);
			IGuildUser usr = users.OrderBy(u => u.Username.Length).First();
			
			// await context.Channel.SendMessageAsync(usr.Username);
			

			coll.Add("username", usr.Username);
			coll.Add("steamid64", usr.Username);
			if (reason.Length <= 0)
			{
				await context.Channel.SendMessageAsync("You must supply a warning reason.");
				return;
			}
			await Upload(coll, out string response, "http://joker.hivehosted.com/DiscordWarns/WarnLogs.php");
			await context.Channel.SendMessageAsync(response);
			await usr.SendMessageAsync("You have received a warning on Joker's Playground! \n" + "Staff who issued warning: " + context.Message.Author.Username + "\n" + "Reason: " + reason);
		}
		
		public static async Task DoServerWarnCommand(CommandContext context, Program program)
		{
			TimeSpan span;
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			// await context.Channel.SendMessageAsync(Convert.ToString(args.Length));
			if (args.Length < 2) return;
			if (args[1].Length != 17 || !long.TryParse(args[1], out long l))
			{
				await context.Channel.SendMessageAsync("Invalid SteamID.");
				return;
			}
			
			span = TimeSpan.FromDays(18250);

			string[] reasonarray = args.Where(p => p != args[0] && p != args[1]).ToArray();
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
				if (reason.Contains("[1]"))
					reason = "Rule 1 - Cheating or Glitching.";
				if (reason.Contains("[2]"))
					reason = "Rule 2 - Teaming";
				if (reason.Contains("[3]"))
					reason = "Rule 3 - Ear Rape/Micspam";
				if (reason.Contains("[4]"))
					reason = "Rule 4 - Derogatory / Racist / Hateful language";
				if (reason.Contains("[5]"))
					reason = "Rule 5 - Camping";
				if (reason.Contains("[6]"))
					reason = "Rule 6 - NSFW Music or Media.";
				if (reason.Contains("[7]"))
					reason = "Rule 7 - Harrassment";
				if (reason.Contains("[8]"))
					reason = "Rule 8 - Team killing";
				if (reason.Contains("[9]"))
					reason = "Rule 9 - Ghosting";
				
				var chan = context.Guild.GetTextChannelAsync(program.Config.BotCommandId).Result;
				await chan.SendMessageAsync("+pbc " + args[1] + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
				await chan.SendMessageAsync("-pbc " + args[1] + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
				await chan.SendMessageAsync(".pbc " + args[1] + " 10 " + "You have been warned for breaking rule " +
				                            reason +
				                            " please review the rules and refrain from breaking more in the future.");
			}
		}

		private static Task DownloadwebClient(out string result, string url)
		{
			using (var webClient = new WebClient())
			{
				webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
				result = webClient.DownloadString(url);
			}
			return Task.FromResult(1);
		}

		public static async Task DoGetInfoCommand(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			NameValueCollection collection = new NameValueCollection
			{
				{ "apikey", "vyJCxUBi9D785DuXxAVoDuD1llZ5jPO4T4YdvfLYNk2Qe2wyHNZVZOJoQ5rXTE97" },
				{ "steamid64", args[1] }
			};
			if (args[1].Contains("@"))
			{
				string name;
				name = args[1].Replace("@", "");
				name = name.Replace("<", "");
				name = name.Replace(">", "");
				if (name.Contains("!"))
					name = name.Replace("!", "");
				IEnumerable<IGuildUser> users = context.Guild.GetUsersAsync(CacheMode.CacheOnly).Result.Where(u => ulong.TryParse(name, out ulong result) ? u.Id == result : u.Username == name);
				IGuildUser usr = users.OrderBy(u => u.Username.Length).First();
				collection.Remove("steamid64");
				collection.Add("steamid64", usr.Username);
			}
			if (_webClient == null) _webClient = new WebClient();
			string url = "http://joker.hivehosted.com/Warns/GetWarnInfo.php";
			if (!long.TryParse(args[1], out long _) || args[1].Length != 17)
				url = "http://joker.hivehosted.com/DiscordWarns/GetWarnInfo.php";
			await Upload(collection, out string response, url);
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoUnWarnCommand(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length <= 2) return;
			NameValueCollection collection = new NameValueCollection
			{
				{ "apikey", "Ntba7mQeAyKRsoZs6qe85t37npzsyLUNPaUc6CRzKcNWgKUp470ja91DX7wioIUH" },
				{ "index", args[2] }
			};
			if (_webClient == null) _webClient = new WebClient();
			string url = "http://joker.hivehosted.com/Warns/RevokeWarn.php";
			if (!long.TryParse(args[1], out long _) || args[1].Length != 17)
			{
				url = "http://joker.hivehosted.com/DiscordWarns/RevokeWarn.php";
				collection.Add("name", args[1]);
			}

			await Upload(collection, out string response, url);
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoReasonCmd(CommandContext context, Program program, string time = null)
		{
			//if (context.Channel.Id != ulong.Parse("536784180999356416")) return;
			//if (!context.Message.Content.ToLower().StartsWith(")reason")) return;

			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length <= 2) return;
			string steamid = args[1];
			if (!ulong.TryParse(steamid, out ulong _) || steamid.Length != 17) return;
			string reason = args.Where(p => p != program.Config.BotPrefix + "reason" && p != steamid).Aggregate("", (current, str) => current + str + " ");
			
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
	}
}