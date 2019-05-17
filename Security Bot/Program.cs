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
		private readonly Bot bot;
		private const string kCfgFile = "SecBotConfig.json";
		public ReportHandler ReportHandler;

		public Config Config => config ?? (config = GetConfig());

		private Config config;
		public static void Main()
		{
			new Program();
		}

		private Program()
		{
			ReportHandler = new ReportHandler(this);
			bot = new Bot(this);
		}

		public static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		public static Task<Config> GetConfigAsync()
		{
			if (File.Exists(kCfgFile))
				return Task.FromResult(JsonConvert.DeserializeObject<Config>(File.ReadAllText(kCfgFile)));
			
			File.WriteAllText(kCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
			return Task.FromResult(Config.Default);
		}

		private static Config GetConfig()
		{
			if (File.Exists(kCfgFile)) 
				return JsonConvert.DeserializeObject<Config>(File.ReadAllText(kCfgFile));
			File.WriteAllText(kCfgFile, JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
			return Config.Default;
		}
	}

	public class Config
	{
		public string BotPrefix { get; set; }
		public string BotToken { get; set; }
		public ulong ReportRoleId { get; set; }
		public string ReportKey { get; set; }

		public static readonly Config Default = new Config
		{
			BotToken = "",
			BotPrefix = "~",
			ReportRoleId = 0,
			ReportKey = ""
		};
	}
}
