using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System.Linq;

namespace Security_Bot
{
    public class Program
    {
        private string DiscordToken { get; set; }
        private DiscordSocketClient client;
        public Rest RestClient { get; private set; }
        public string ApiKey { get; private set; }
        public List<string> AllowedRoles = new List<string>();
        public List<string> AllowedUsers = new List<string>();

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            string path = Directory.GetCurrentDirectory() + "/SecBot.cfg";

            if (!File.Exists(path))
            {
                StreamWriter writer = new StreamWriter(path);
                File.CreateText(path);
                writer.WriteLine("api_key: ");
                writer.WriteLine("discord_token: ");
                writer.WriteLine("allowed_role_ids: ");
                writer.WriteLine("allowed_user_ids: ");
            }

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                string[] kvp = System.Text.RegularExpressions.Regex.Split(line, ": ");
                switch (kvp[0].ToLower())
                {
                    case "api_key":
                        ApiKey = kvp[1];
                        break;
                    case "discord_token":
                        DiscordToken = kvp[1];
                        break;
                    case "allowed_role_ids":
                        AllowedRoles = kvp[1].Split(',').ToList();
                        break;
                    case "allowed_user_ids":
                        AllowedUsers = kvp[1].Split(',').ToList();
                        break;
                }
            }

            client = new DiscordSocketClient();
            client.Log += Log;
            CommandHandler commands = new CommandHandler(client, new CommandService(), this);

            await commands.InstallCommandsAsync();
            RestClient = new Rest(this);

            await client.LoginAsync(TokenType.Bot, DiscordToken);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
