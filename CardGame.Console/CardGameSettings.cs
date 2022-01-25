using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.Console
{
    /// <summary>
    ///     This class is part of the composition root and is used to load and save the game settings from an XML file.
    /// </summary>
    internal class CardGameSettings
    {
        private IDependencyResolver dependencyResolver;

        private CardGameSettings()
        {
        }

        public ICardCollection Cards { get; set; }

        public Func<ICard, int> GetCardValueDelegate { get; set; }

        public int MinimumVictoryScoreTotal { get; set; }

        public int RequiredVictoryScoreLead { get; set; }

        public int HighestCardScoreChange { get; set; }

        public int PlayerCountMinimum { get; set; }

        public int PlayerCountMaximum { get; set; }

        public int PlayerNameMinimumLength { get; set; }

        public int PlayerNameMaximumLength { get; set; }

        public bool VictoryScoreLeadMustBeAnExactValue { get; set; }

        public bool RankCardsByFaceValueFirst { get; set; }

        public char DrawCardKey { get; set; }

        public char ContinueRoundKey { get; set; }

        public char ExitKey { get; set; }

        private int faceValueScoreModifier = 100;

        private int suitScoreModifier = 1;

        private Dictionary<IFaceValue, CardMemberData> faceValueData;

        private Dictionary<ISuit, CardMemberData> suitData;

        private List<SpecialCardData> specialCards;

        private ISuit GetNewSuit(string name)
        {
            var suit = this.dependencyResolver.Get<ISuit>();
            suit.Name = name;

            return suit;
        }

        private IFaceValue GetNewFaceValue(string value)
        {
            var faceValue = this.dependencyResolver.Get<IFaceValue>();
            faceValue.Value = value;

            return faceValue;
        }

        private ICard GetNewCard(IFaceValue faceValue, ISuit suit)
        {
            var card = this.dependencyResolver.Get<ICard>();
            card.FaceValue = faceValue;
            card.Suit = suit;

            return card;
        }

        internal class CardMemberData
        {
            public int Rank { get; set; }

            public CardMemberData(int value)
            {
                this.Rank = value;
            }
        }

        internal class SpecialCardData
        {
            public ICard Card { get; set; }

            public int Count { get; set; }

            public int ScoreChange { get; set; }

            public SpecialCardData(ICard card, int scoreChange, int count)
            {
                Guard.AgainstNull(card, nameof(card));

                this.Card = card;
                this.ScoreChange = scoreChange;
                this.Count = count;
            }
        }

        internal static CardGameSettings Create(IDependencyResolver dependencyResolver)
        {
            Guard.AgainstNull(dependencyResolver, nameof(dependencyResolver));

            var fileName = Constants.SettingsFilePath;
            var settings = new CardGameSettings();
            settings.dependencyResolver = dependencyResolver;

            if (File.Exists(fileName))
            {
                try
                {
                    var storedSettings = LoadStoredSettings(settings, fileName);
                    return storedSettings;
                }
                catch (Exception) // occurs when the file has been modified outside of the program, likely by a user
                {
                    settings = new CardGameSettings();
                    settings.dependencyResolver = dependencyResolver;
                    System.Console.WriteLine("Error: Settings.xml file has been corrupted. Loading default settings.");
                    System.Console.WriteLine();
                }
            }

            settings.LoadDefaultSettings();
            settings.SaveFile(fileName);

            return settings;
        }

        private void AddSuit(string name, int rank)
        {
            var suit = this.GetNewSuit(name);
            var data = new CardMemberData(rank);

            this.suitData.Add(suit, data);
        }

        private void AddFaceValue(string value, int rank)
        {
            var suit = this.GetNewFaceValue(value);
            var data = new CardMemberData(rank);

            this.faceValueData.Add(suit, data);
        }

        private void LoadDefaultSettings()
        {
            this.PlayerNameMinimumLength = 1;
            this.PlayerNameMaximumLength = 10;
            this.PlayerCountMinimum = 2;
            this.PlayerCountMaximum = 4;
            this.HighestCardScoreChange = 2;
            this.MinimumVictoryScoreTotal = 21;
            this.RequiredVictoryScoreLead = 2;
            this.VictoryScoreLeadMustBeAnExactValue = false;
            this.RankCardsByFaceValueFirst = true;
            this.DrawCardKey = 'D';
            this.ContinueRoundKey = 'D';
            this.ExitKey = (char)27;

            this.Cards = new CardCollection();
            this.suitData = new Dictionary<ISuit, CardMemberData>();
            this.faceValueData = new Dictionary<IFaceValue, CardMemberData>();

            this.AddSuit("Clubs", 0);
            this.AddSuit("Diamonds", 1);
            this.AddSuit("Hearts", 2);
            this.AddSuit("Spades", 3);

            this.AddFaceValue("Two", 2);
            this.AddFaceValue("Three", 3);
            this.AddFaceValue("Four", 2);
            this.AddFaceValue("Five", 2);
            this.AddFaceValue("Six", 6);
            this.AddFaceValue("Seven", 7);
            this.AddFaceValue("Eight", 8);
            this.AddFaceValue("Nine", 9);
            this.AddFaceValue("Ten", 10);
            this.AddFaceValue("Jack", 11);
            this.AddFaceValue("Queen", 12);
            this.AddFaceValue("King", 13);
            this.AddFaceValue("Ace", 14);

            var penaltyCard = this.GetNewCard(this.GetNewFaceValue("Penalty"), null);
            this.specialCards = new List<SpecialCardData> { new SpecialCardData(penaltyCard, -1, 4) };

            this.LoadCards();
        }

        private void LoadCards()
        {
            if (!this.RankCardsByFaceValueFirst)
            {
                this.faceValueScoreModifier = 1;
                this.suitScoreModifier = 100;
            }

            foreach (var suit in suitData.Keys)
            {
                foreach (var faceValue in faceValueData.Keys)
                {
                    var card = this.GetNewCard(faceValue, suit);
                    this.Cards.Add(card);
                }
            }

            foreach (var specialCardData in this.specialCards)
            {
                for (int i = 0; i < specialCardData.Count; i++)
                {
                    this.Cards.Add(specialCardData.Card);
                }
            }

            this.GetCardValueDelegate = this.GetCardValue;
        }

        private int GetCardValue(ICard card)
        {
            Guard.AgainstNull(card, nameof(card));

            var specialCardData = this.specialCards.FirstOrDefault(scd => scd.Card == card);
            if (specialCardData != null)
            {
                return specialCardData.ScoreChange;
            }

            int value = 0;
            CardMemberData faceValueData;
            CardMemberData suitData;

            if (card.FaceValue != null && this.faceValueData.TryGetValue(card.FaceValue, out faceValueData))
            {
                value += faceValueData.Rank * faceValueScoreModifier;
            }

            if (card.Suit != null && this.suitData.TryGetValue(card.Suit, out suitData))
            {
                value += suitData.Rank * suitScoreModifier;
            }

            return value;
        }

        private static CardGameSettings LoadStoredSettings(CardGameSettings gameSettings, string fileName)
        {
            var doc = XDocument.Load(fileName);
            var settingsElement = doc.Element("Settings");

            var properties = typeof(CardGameSettings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.PropertyType.IsValueType).ToList();
            settingsElement.Attributes().ToList().ForEach(attr =>
            {
                var property = properties.FirstOrDefault(p => p.Name == attr.Name);
                if (property != null)
                {
                    gameSettings.SetValue(property, attr.Value);
                }
            });

            gameSettings.Cards = new CardCollection();
            gameSettings.suitData = new Dictionary<ISuit, CardMemberData>();
            gameSettings.faceValueData = new Dictionary<IFaceValue, CardMemberData>();
            gameSettings.specialCards = new List<SpecialCardData>();

            foreach (var faceValueElement in settingsElement.Element("Suits").Elements("Suit"))
            {
                gameSettings.AddSuit(faceValueElement.Attribute("Name").Value, Int32.Parse(faceValueElement.Attribute("Rank").Value));
            }

            foreach (var faceValueElement in settingsElement.Element("FaceValues").Elements("FaceValue"))
            {
                gameSettings.AddFaceValue(faceValueElement.Attribute("Value").Value, Int32.Parse(faceValueElement.Attribute("Rank").Value));
            }

            foreach (var specialCardElement in settingsElement.Element("SpecialCards").Elements("Card"))
            {
                var suitAttribute = specialCardElement.Attribute("Suit").Value;
                var faceValueAttribute = specialCardElement.Attribute("Value").Value;
                var scoreChange = Int32.Parse(specialCardElement.Attribute("ScoreChange").Value);
                var count = Int32.Parse(specialCardElement.Attribute("Count").Value);

                var card = gameSettings.GetNewCard(
                    faceValueAttribute == Constants.NullValue ? null : gameSettings.GetNewFaceValue(faceValueAttribute),
                    suitAttribute == Constants.NullValue ? null : gameSettings.GetNewSuit(suitAttribute));

                var data = new SpecialCardData(card, scoreChange, count);
                gameSettings.specialCards.Add(data);
            }

            gameSettings.LoadCards();

            return gameSettings;
        }

        private void SetValue(PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(this, value);
            }
            else if (property.PropertyType == typeof(bool))
            {
                property.SetValue(this, Boolean.Parse(value));
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(this, Int32.Parse(value));
            }
            else if (property.PropertyType == typeof(char))
            {
                property.SetValue(this, (char)Int32.Parse(value));
            }
        }

        private object GetOutputValue(object o)
        {
            if (o.GetType() == typeof(Char))
            {
                return (int)(char)o;
            }
            else
            {
                return o;
            }
        }

        private void SaveFile(string fileName)
        {
            var doc = new XDocument();
            var settings = new XElement("Settings");

            var properties = typeof(CardGameSettings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.PropertyType.IsValueType).ToList();
            properties.ForEach(p => settings.Add(new XAttribute(p.Name, GetOutputValue(p.GetValue(this)))));

            settings.Add(new XElement(
                "Suits",
                from data in this.suitData
                select new XElement("Suit",
                    new XAttribute("Name", data.Key.Name),
                    new XAttribute("Rank", data.Value.Rank))));

            settings.Add(new XElement(
                "FaceValues",
                from data in this.faceValueData
                select new XElement("FaceValue",
                    new XAttribute("Value", data.Key.Value),
                    new XAttribute("Rank", data.Value.Rank))));

            settings.Add(new XElement(
                "SpecialCards",
                from cardData in this.specialCards
                select new XElement("Card",
                    new XAttribute("Suit", cardData.Card.Suit?.Name ?? Constants.NullValue),
                    new XAttribute("Value", cardData.Card.FaceValue?.Value ?? Constants.NullValue),
                    new XAttribute("ScoreChange", cardData.ScoreChange),
                    new XAttribute("Count", cardData.Count))));

            doc.Add(settings);
            doc.Save(fileName);
        }
    }
}