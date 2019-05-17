using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Security_Bot
{
	public class Program
	{
		public DiscordSocketClient _client;
		public readonly Bot bot;
		public const string kCfgFile = "SecBotConfig.json";
		public static ReportHandler reportHandler;

		public Config config
		{
			get
			{
				if (_config == null)
					_config = GetConfig();
				return _config;
			}
		}

		private Config _config = null;
		public static void Main(string[] args)
		{
			new Program();
		}

		public Program()
		{
			reportHandler = new ReportHandler(this);
			bot = new Bot(this);
		}

		public static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		public static Task<Config> GetConfigAsync()
		{
			if (!File.Exists(kCfgFile))
			{
				File.WriteAllText(kCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
				return Task.FromResult(Config.Default);
			}
			return Task.FromResult(JsonConvert.DeserializeObject<Config>(File.ReadAllText(kCfgFile)));
		}

		public static Config GetConfig()
		{
			if (!File.Exists(kCfgFile))
			{
				File.WriteAllText(kCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
				return Config.Default;
			}
			return JsonConvert.DeserializeObject<Config>(File.ReadAllText(kCfgFile));
		}
	}

	public class Config
	{
		public string BotPrefix { get; set; }
		public string BotToken { get; set; }
		public ulong ReportRoleID { get; set; }
		public string ReportKey { get; set; }

		public static readonly Config Default = new Config()
		{
			BotToken = "",
			BotPrefix = "~",
			ReportRoleID = 0,
			ReportKey = ""
		};
	}
}
