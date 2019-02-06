using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperfightBot.Game
{
    class GameContext
    {
        public ulong Id { get; private set; }

        public GameDeck Deck { get; private set; }

        public HashSet<IUser> Players { get; private set; }

        public HashSet<Deck> UsedDecks { get; private set; }

        public GameContext(ulong id)
        {
            Id = id;
            UsedDecks = new HashSet<Deck>();
            UsedDecks.Add(Decks.GetDeck("main"));
            Deck[] decks = UsedDecks.ToArray();
            Deck = new GameDeck(decks);
            Players = new HashSet<IUser>();
            ReadFromJson();

        }

        public void ResetDeck()
        {
            Deck[] decks = UsedDecks.ToArray();
            Deck = new GameDeck(decks);
        }

        public void AddPlayers(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                Players.Add(user);
            }
        }

        public void RemovePlayers(params IUser[] users)
        {
            foreach (IUser user in users)
            {
                Players.Remove(user);
            }
        }

        public void AddDecks(params string[] decks)
        {
            foreach (Deck deck in Decks.GetDecks(decks))
            {
                UsedDecks.Add(deck);
            }
        }

        public void RemoveDecks(params string[] decks)
        {
            foreach (Deck deck in Decks.GetDecks(decks))
            {
                UsedDecks.Remove(deck);
            }
        }

        public void SaveToJson()
        {
            using (StreamWriter writer = new StreamWriter("guilds/" + Id + ".json"))
            {
                GameContextJson gameJson = new GameContextJson();
                gameJson.usedDecks = new string[UsedDecks.Count];
                int i = 0;
                foreach (Deck deck in UsedDecks)
                {
                    gameJson.usedDecks[i] = deck.Name;
                    i++;
                }
                string json = JsonConvert.SerializeObject(gameJson);
                writer.WriteLine(json);
            }
        }

        public void ReadFromJson()
        {
            try
            {
                using (StreamReader reader = new StreamReader("guilds/" + Id + ".json"))
                {
                    string json = reader.ReadToEnd();
                    GameContextJson gameJson = JsonConvert.DeserializeObject<GameContextJson>(json);
                    AddDecks(gameJson.usedDecks);
                }
            }
            catch (FileNotFoundException)
            {
                
            }
            
        }

        private class GameContextJson
        {
            public string[] usedDecks;
        }
    }
}
