using System;
using System.Collections.Generic;

namespace SuperfightBot.Game
{
    class GameDeck
    {
        public List<Card> Attributes { get; private set; }
        public List<Card> Challenges { get; private set; }
        public List<Card> Characters { get; private set; }
        public List<Card> Locations { get; private set; }
        public List<Card> Scenarios { get; private set; }

        private Deck[] decks;

        private Random rand;

        public GameDeck(params Deck[] decks)
        {
            this.decks = decks;
            rand = new Random();
            Reset();
            Shuffle();
        }

        public void Reset()
        {
            Attributes = new List<Card>();
            Challenges = new List<Card>();
            Characters = new List<Card>();
            Locations = new List<Card>();
            Scenarios = new List<Card>();

            foreach (Deck deck in decks)
            {
                Attributes.AddRange(deck.Attributes);
                Challenges.AddRange(deck.Challenges);
                Characters.AddRange(deck.Characters);
                Locations.AddRange(deck.Locations);
                Scenarios.AddRange(deck.Scenarios);
            }
        }

        public void Shuffle()
        {
            Shuffle(Attributes, rand);
            Shuffle(Challenges, rand);
            Shuffle(Characters, rand);
            Shuffle(Locations, rand);
            Shuffle(Scenarios, rand);
        }

        private void Shuffle(List<Card> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private Card Draw(List<Card> list)
        {
            try
            {
                Card result = list[0];
                list.RemoveAt(0);
                return result;
            }
            catch (ArgumentOutOfRangeException)
            {
                Reset();
                if (list.Count > 0)
                {
                    return Draw(list);
                }
                else
                {
                    throw new Exception("There are no cards in the deck");
                }
            }
            
        }

        public Card DrawAttribute()
        {
            return Draw(Attributes);
        }

        public Card DrawChallenge()
        {
            return Draw(Challenges);
        }

        public Card DrawCharacter()
        {
            return Draw(Characters);
        }

        public Card DrawLocation()
        {
            return Draw(Locations);
        }

        public Card DrawScenario()
        {
            return Draw(Scenarios);
        }
    }
}
