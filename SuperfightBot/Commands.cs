using Discord;
using Discord.Commands;
using SuperfightBot.Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using System.Linq;

namespace SuperfightBot
{
    public class Commands : InteractiveBase
    {
        private CommandService service;

        public Commands(CommandService service)
        {
            this.service = service;
        }

        [Command("random"), Summary("Draw a random full character."), Alias("r")]
        public async Task Random()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled **{1}** with attributes **{2}** and **{3}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawCharacter(), context.Deck.DrawAttribute(), context.Deck.DrawAttribute()));
        }

        [Command("attribute"), Summary("Draw a random attribute."), Alias("a")]
        public async Task Attribute()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled attribute **{1}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawAttribute()));
        }

        [Command("challenge"), Summary("Draw a random challenge."), Alias("chal")]
        public async Task Challenge()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled challenge **{1}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawChallenge()));
        }

        [Command("character"), Summary("Draw a random character."), Alias("char")]
        public async Task Character()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled character **{1}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawCharacter()));
        }

        [Command("location"), Summary("Draw a random location."), Alias("l")]
        public async Task Location()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled location **{1}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawLocation()));
        }

        [Command("scenario"), Summary("Draw a random scenario."), Alias("sc")]
        public async Task Scenario()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            await ReplyAsync(string.Format("{0} pulled scenario **{1}**", ((IGuildUser)Context.User).Nickname, context.Deck.DrawScenario()));
        }

        [Command("reset"), Summary("Reshuffle the deck."), Alias("rs")]
        public async Task Reset()
        {
            await ReplyAsync("Reshuffling...");
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            context.ResetDeck();
        }

        [Command("players"), Summary("Add, remove, clear or list players."), Alias("p")]
        public async Task Players(string argument = null, params IUser[] users)
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            switch (argument)
            {
                case "add":
                    context.AddPlayers(users);
                    break;
                case "remove":
                    context.RemovePlayers(users);
                    break;
                case "clear":
                    context.Players.Clear();
                    break;
                default:
                    if (context.Players.Count > 0)
                    {
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithTitle("Superfight Players");
                        string description = string.Empty;
                        foreach (IUser user in context.Players)
                        {
                            description += ((IGuildUser)user).Nickname + Environment.NewLine;
                        }
                        embed.WithDescription(description);
                        await ReplyAsync("", false, embed.Build());
                    }
                    else
                    {
                        await ReplyAsync("There are currently no players playing Superfight");
                    }
                    
                    break;
            }
        }

        [Command("draw", RunMode = RunMode.Async), Summary("Draw a new hand for all players."), Alias("d")]
        public async Task Draw()
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            foreach (IUser user in context.Players)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithTitle("Superfight Round");
                embed.WithDescription("Respond with \'number, number\' to play cards. E.g. \'2, 1\' \n First number chooses character and second number chooses attribute");
                
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                EmbedFieldBuilder characterField = new EmbedFieldBuilder();
                characterField.WithName("Characters");
                characterField.WithIsInline(true);
                Card[] characters = new Card[3];
                for (int i = 0; i < 3; i++)
                {
                    characters[i] = context.Deck.DrawCharacter();
                }
                characterField.WithValue(string.Format("1: {1} {0} 2: {2} {0} 3: {3}", Environment.NewLine, characters[0], characters[1], characters[2]));

                embed.AddField(characterField);

                EmbedFieldBuilder attributeField = new EmbedFieldBuilder();
                attributeField.WithName("Attributes");
                attributeField.WithIsInline(true);
                Card[] attributes = new Card[3];
                for (int i = 0; i < 3; i++)
                {
                    attributes[i] = context.Deck.DrawAttribute();
                }
                attributeField.WithValue(string.Format("1: {1} {0} 2: {2} {0} 3: {3}", Environment.NewLine, attributes[0], attributes[1], attributes[2]));

                embed.AddField(attributeField);

                IDMChannel channel = await user.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync("", false, embed.Build());

                MessageUser(user, channel, Context.Channel, characters, attributes, context.Deck.DrawAttribute());
            }
        }

        async Task MessageUser(IUser user, IDMChannel dMChannel, ISocketMessageChannel channel, Card[] characters, Card[] attributes, Card randomAttribute)
        {
            var criterion = new Criteria<SocketMessage>();
            criterion.AddCriterion(new EnsureFromUserCriterion(user.Id));
            var response = await NextMessageAsync(criterion, TimeSpan.FromMinutes(10));
            if (response != null)
            {
                try
                {
                    string[] inputs = response.Content.Split(',');

                    Card character = characters[int.Parse(inputs[0]) - 1];
                    Card attribute = attributes[int.Parse(inputs[1]) - 1];

                    string message = string.Format("{0} played **{1}** with attribute **{2}** and random attribute **{3}**", ((IGuildUser)user).Nickname, character, attribute, randomAttribute);
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithDescription(message);
                    await channel.SendMessageAsync("", false, embed.Build());
                }
                catch (Exception e)
                {
                    await dMChannel.SendMessageAsync("Invalid input");
                    await MessageUser(user, dMChannel, channel, characters, attributes, randomAttribute);
                }
            }
            else
                await dMChannel.SendMessageAsync("Round has timed out");
        }

        [Command("decks"), Summary("Add, remove, clear or list decks.")]
        public async Task Deck(string argument = null, params string[] deckNames)
        {
            GameContext context = GameContexts.getContext(Context.Guild.Id);
            switch (argument)
            {
                case "add":
                    context.AddDecks(deckNames);
                    context.SaveToJson();
                    break;
                case "remove":
                    context.RemoveDecks(deckNames);
                    context.SaveToJson();
                    break;
                default:
                    if (context.UsedDecks.Count > 0)
                    {
                        EmbedBuilder embed = new EmbedBuilder();

                        EmbedFieldBuilder usedField = new EmbedFieldBuilder();
                        usedField.WithName("Current Decks");
                        string description = string.Empty;
                        foreach (Deck deck in context.UsedDecks)
                        {
                            description += string.Format("{2} {0} - {1}" + Environment.NewLine, deck.Name, deck.Description, deck.Emoji);
                        }
                        usedField.WithValue(description);

                        EmbedFieldBuilder unusedField = new EmbedFieldBuilder();
                        unusedField.WithName("Avaliable Decks");
                        description = string.Empty;
                        List<Deck> unusedDecks = (List<Deck>)Decks.GetAllDecks();
                        unusedDecks.RemoveAll(s => context.UsedDecks.Contains(s));
                        foreach (Deck deck in unusedDecks)
                        {
                            description += string.Format("{2} {0} - {1}" + Environment.NewLine, deck.Name, deck.Description, deck.Emoji);
                        }
                        unusedField.WithValue(description);

                        embed.AddField(usedField);
                        embed.AddField(unusedField);
                        
                        await ReplyAsync("", false, embed.Build());
                    }
                    else
                    {
                        await ReplyAsync("You currently have no decks.");
                    }

                    break;
            }
        }

        [Command("help"), Summary("List avaliable commands.")]
        public async Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Description = "These are the commands you can use";

            foreach (ModuleInfo module in service.Modules)
            {
                string description = string.Empty;
                foreach (CommandInfo command in module.Commands)
                {
                    if (!command.Aliases.First().Equals("reload"))
                    {
                        PreconditionResult result = await command.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess)
                            description += string.Format("!{0} (!{1}) - {2}" + Environment.NewLine, command.Aliases.First(), command.Aliases.Last(), command.Summary);
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    embed.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, embed.Build());
        }

        [Command("reload"), Summary("Reload decks.")]
        public async Task Reload()
        {
            Decks.ReloadDecks();
            await ReplyAsync("Reloading decks from files.");
        }
    }
}
