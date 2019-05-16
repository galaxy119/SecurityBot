using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Security_Bot
{
    internal class Program
    {
        private const string discordToken = "[REDACTED]";
        private DiscordSocketClient client;
        
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            CommandHandler commands = new CommandHandler(client, new CommandService());

            await commands.InstallCommandsAsync();
            
            await client.LoginAsync(TokenType.Bot, discordToken);
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
