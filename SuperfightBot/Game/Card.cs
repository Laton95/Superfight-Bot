using System;
using System.Collections.Generic;
using System.Text;

namespace SuperfightBot.Game
{
    class Card
    {
        public enum CardType
        {
            ATTRIBUTE,
            CHALLENGE,
            CHARACTER,
            LOCATION,
            SCENARIO
        }

        public string Content { get; private set; }

        public CardType Type { get; private set; }

        private Deck deck;

        public Card(string content, CardType type, Deck deck)
        {
            Content = content;
            Type = type;
            this.deck = deck;
        }

        public override string ToString()
        {
            return deck.Emoji + Content;
        }
    }
}
