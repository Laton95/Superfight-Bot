using System;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using SuperfightBot.Game;
using Discord.Addons.Interactive;
using System.IO;

namespace SuperfightBot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Decks.LoadDecks();

            client = new DiscordSocketClient();

            client.Log += Log;
            commands = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();

            await InstallCommands();

            string token = string.Empty;

            using (StreamReader reader = new StreamReader("token"))
            {
                token = reader.ReadLine();
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            if (message == null)
            {
                return;
            }
            
            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(client, message);

            IResult result = await commands.ExecuteAsync(context, argPos, services);

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
                
        }
        
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
