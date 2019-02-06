using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static SuperfightBot.Game.Card;

namespace SuperfightBot.Game
{
    class Deck
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Emoji { get; private set; }
        public List<Card> Attributes { get; private set; }
        public List<Card> Challenges { get; private set; }
        public List<Card> Characters { get; private set; }
        public List<Card> Locations { get; private set; }
        public List<Card> Scenarios { get; private set; }

        public Deck(string name)
        {
            Name = name;
            Load();
        }

        public void Load()
        {
            Attributes = new List<Card>();
            Challenges = new List<Card>();
            Characters = new List<Card>();
            Locations = new List<Card>();
            Scenarios = new List<Card>();

            LoadCards(Attributes, CardType.ATTRIBUTE);
            LoadCards(Challenges, CardType.CHALLENGE);
            LoadCards(Characters, CardType.CHARACTER);
            LoadCards(Locations, CardType.LOCATION);
            LoadCards(Scenarios, CardType.SCENARIO);

            using (StreamReader reader = new StreamReader("decks/" + Name + "/deck.json"))
            {
                string json = reader.ReadToEnd();
                DeckJson deckJson = JsonConvert.DeserializeObject<DeckJson>(json);
                Description = deckJson.description;
                Emoji = deckJson.emoji;
            }
        }

        private void LoadCards(List<Card> cards, CardType type)
        {
            using (StreamReader reader = new StreamReader("decks/" + Name + "/" + type.ToString().ToLower()))
            {
                while (!reader.EndOfStream)
                {
                    string card = reader.ReadLine();
                    if (card != string.Empty)
                    {
                        cards.Add(new Card(card, type, this));
                    }
                    
                }
            }
        }

        private class DeckJson
        {
            public string description;
            public string emoji;
        }
    }
}
