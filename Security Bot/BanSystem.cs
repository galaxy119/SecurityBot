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
	public class BanSystem
	{
		private static WebClient _webClient;
		
		public static async Task DoServerBanCommand(CommandContext context, Program program)
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
			if (args[2].ToUpper() == "PERM")
				span = TimeSpan.FromDays(18250);
			else
			{
				string humanReadableDuration = "";
				string toParse = args.Where(p => p != args[1] && p != args[0]).Aggregate("", (current, s) => current + " " + s);

				span = ParseBanDuration(toParse, ref humanReadableDuration);
				if (span.Ticks <= 0)
				{
					await context.Channel.SendMessageAsync("Error while parsing the ban duration.");
					await context.Channel.SendMessageAsync(span.ToString());
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
			await DownloadwebClient(out string result, "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + apikey + "&steamids=" + args[1]);
			RootObject players = JsonConvert.DeserializeObject<RootObject>(result);
			Player player = players.response.players.FirstOrDefault();
			if (player == null)
			{
				await context.Channel.SendMessageAsync("ERROR Player is null.");
				return;
			}

			coll.Add("username", player.personaname);
			await Upload(coll, out string response, "http://joker.hivehosted.com/Bans/BanLogs.php");
			await context.Channel.SendMessageAsync(response);
			var chan = await context.Guild.GetTextChannelAsync(program.Config.BotCommandId);
			
			await chan.SendMessageAsync("+kick " + args[1]);
			await chan.SendMessageAsync("-kick " + args[1]);
			await chan.SendMessageAsync(".kick " + args[1]);
		}

		public static async Task DoDiscordBanCommand(CommandContext context, Program program, bool kick = false, bool softban = false)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			//await context.Channel.SendMessageAsync(Convert.ToString(args.Length));
			if (args.Length < 2) return;
			TimeSpan span;
			span = kick ? TimeSpan.Zero : TimeSpan.FromDays(18250);

			string[] reasonarray = args.Where(p => p != args[0] && p != args[1]).ToArray();
			string reason = string.Join(' ', reasonarray);
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
			coll.Add("steamid64", usr.Username);
			coll.Add("duration", "Permanent");
			coll.Add("username", usr.Username);
			coll.Add("reason", reason);
			await Upload(coll, out string response, "http://joker.hivehosted.com/DiscordBans/BanLogs.php");
			await context.Channel.SendMessageAsync(response);

			if (kick)
			{
				await usr.KickAsync();
				await usr.SendMessageAsync("You have been kicked from Joker's Playground! \n" + "Staff issuing kick: " +
				                           context.Message.Author.Username + "\n" + "Reason: " + reason);
			}
			else if (softban)
			{
				await context.Guild.AddBanAsync(usr.Id, 7, reason);
				await context.Guild.RemoveBanAsync(usr.Id);
				await usr.SendMessageAsync("You have been kicked from Joker's Playground! \n" + "Staff issuing kick: " +
				                           context.Message.Author.Username + "\n" + "Reason: " + reason);
			}
			else
			{
				await context.Guild.AddBanAsync(usr.Id, 7, reason);
				await usr.SendMessageAsync("You have been banned from Joker's Playground! \n" + "Staff issuing ban: " +
				                           context.Message.Author.Username + "\n" + "Reason: " + reason);
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
				IEnumerable<IGuildUser> users = context.Guild.GetUsersAsync(CacheMode.CacheOnly).Result.Where(u =>
					ulong.TryParse(name, out ulong result) ? u.Id == result : u.Username == name);
				IGuildUser user = users.OrderBy(u => u.Username.Length).First();
				collection.Remove("steamid64");
				collection.Add("steamid64", user.Username);
			}
			if (_webClient == null) _webClient = new WebClient();
			string url = "http://joker.hivehosted.com/Bans/GetBanInfo.php";
			if (!long.TryParse(args[1], out long _) || args[1].Length != 17)
				url = "http://joker.hivehosted.com/DiscordBans/GetBanInfo.php";
			
			await Upload(collection, out string response, url);
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoUnbanCmd(CommandContext context)
		{
			string[] args = context.Message.Content.Split(new string[] {" "}, StringSplitOptions.None);
			if (args.Length != 2) return;
			
			NameValueCollection collection = new NameValueCollection
			{
				{ "apikey", "Ntba7mQeAyKRsoZs6qe85t37npzsyLUNPaUc6CRzKcNWgKUp470ja91DX7wioIUH" },
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
				IEnumerable<IGuildUser> users = context.Guild.GetUsersAsync(CacheMode.CacheOnly).Result.Where(u =>
					ulong.TryParse(name, out ulong result) ? u.Id == result : u.Username == name);
				IGuildUser user = users.OrderBy(u => u.Username.Length).First();
				collection.Remove("steamid64");
				collection.Add("steamid64", user.Username);
			}
			
			if (_webClient == null) _webClient = new WebClient();
			string url = "http://joker.hivehosted.com/Bans/RevokeBan.php";
			if (!long.TryParse(args[1], out long _) || args[1].Length != 17)
				url = "http://joker.hivehosted.com/DiscordBans/RevokeBan.php";
			await Upload(collection, out string response, url);
			await context.Channel.SendMessageAsync(response);
		}

		public static async Task DoReasonCmd(CommandContext context, Program program)
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
				{"steamid64", steamid}
			};
			if (_webClient == null) _webClient = new WebClient();
			await Upload(col, out string response, "http://joker.hivehosted.com/Bans/UpdateReason.php");
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