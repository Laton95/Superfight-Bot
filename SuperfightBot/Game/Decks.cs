using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SuperfightBot.Game
{
    static class Decks
    {
        private static Dictionary<string, Deck> decks;

        public static void LoadDecks()
        {
            decks = new Dictionary<string, Deck>();

            foreach (string deck in GetDeckNames())
            {
                AddDeck(deck);
            }
        }

        public static void ReloadDecks()
        {
            foreach (Deck deck in decks.Values)
            {
                deck.Load();
            }
        }

        public static IEnumerable<string> GetDeckNames()
        {
            List<string> output = new List<string>();
            string deckDirectory = Directory.GetCurrentDirectory() + "/decks";
            foreach (string directory in Directory.GetDirectories(deckDirectory))
            {
                output.Add(directory.Substring(deckDirectory.Length + 1));
            }
            return output;
        }

        private static void AddDeck(string name)
        {
            decks.Add(name, new Deck(name));
        }

        public static Deck GetDeck(string name)
        {
            decks.TryGetValue(name, out Deck result);

            if (result != null)
            {
                return result;
            }

            throw new Exception("Deck not found: " + name);
        }

        public static IEnumerable<Deck> GetAllDecks()
        {
            return decks.Values.ToList();
        }

        public static IEnumerable<Deck> GetDecks(params string[] names)
        {
            List<Deck> output = new List<Deck>();

            foreach (string deckName in names)
            {
                output.Add(GetDeck(deckName));
            }

            return output;
        }
    }
}
