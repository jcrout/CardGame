using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CardGame.Core;
using CardGame.TextBased;
using Ninject.Extensions.Factory;

namespace CardGame.Console
{
    /// <summary>
    ///     The composition root of the program, where bindings are defined and settings are loaded.
    /// </summary>
    public class NinjectCompositionRoot : Ninject.Modules.NinjectModule
    {
        /// <summary>
        ///     Dummy class used to simplify saving/loading bindings where the service is bound to a proxy factory generated by Ninject.
        /// </summary>
        private class FactoryType { }

        private List<Tuple<Type, Type>> customBindings = new List<Tuple<Type, Type>>();

        private void LoadStoredBindings(string fileName)
        {
            var doc = XDocument.Load(fileName);
            var bindings = doc.Element("Config").Elements();
            foreach (var bindingElement in bindings)
            {
                var serviceType = Type.GetType(bindingElement.Attribute("Service").Value);
                var targetType = Type.GetType(bindingElement.Attribute("Target").Value);

                if (targetType == typeof(FactoryType))
                {
                    this.BindToFactory(serviceType);
                }
                else
                {
                    this.Bind(serviceType, targetType);
                }
            }
        }

        private void SaveBindings(string fileName)
        {
            var doc = new XDocument();
            var configElement = new XElement(
                "Config",
                from binding in this.customBindings
                select new XElement(
                    "Binding",
                    new XAttribute("Service", binding.Item1.AssemblyQualifiedName),
                    new XAttribute("Target", binding.Item2.AssemblyQualifiedName)));

            doc.Add(configElement);
            doc.Save(fileName);
        }

        private Ninject.Syntax.IBindingWhenInNamedWithOrOnSyntax<object> Bind(Type serviceType, Type targetType)
        {
            this.customBindings.Add(new Tuple<Type, Type>(serviceType, targetType));
            return this.Bind(serviceType).To(targetType);
        }

        private new Ninject.Syntax.IBindingWhenInNamedWithOrOnSyntax<T2> Bind<T1, T2>() where T2 : T1
        {
            this.customBindings.Add(new Tuple<Type, Type>(typeof(T1), typeof(T2)));
            return this.Bind<T1>().To<T2>();
        }

        private Ninject.Syntax.IBindingWhenInNamedWithOrOnSyntax<T> BindToFactory<T>() where T : class
        {
            this.customBindings.Add(new Tuple<Type, Type>(typeof(T), typeof(FactoryType)));
            return this.Bind<T>().ToFactory();
        }

        private Ninject.Syntax.IBindingWhenInNamedWithOrOnSyntax<object> BindToFactory(Type serviceType)
        {
            this.customBindings.Add(new Tuple<Type, Type>(serviceType, typeof(FactoryType)));
            return this.Bind(serviceType).ToFactory(serviceType);
        }

        private void LoadDefaultBindings()
        {
            this.Bind<ITextInterface, ConsoleInterface>().InSingletonScope();
            this.Bind<IPlayer, Player>();
            this.Bind<IPlayerFactory, PlayerFactory>();
            this.Bind<IPlayerCollection, PlayerCollection>();
            this.Bind<IPlayerCollectionFactory, CardGame.TextBased.PlayerCollectionFactory>();
            this.Bind<ICardGame, CardGame.Core.CardGame>();
            this.Bind<ICardGameFactory, CardGameFactory>();
            this.Bind<IRandomGenerator, RandomGenerator>();
            this.Bind<ICard, Card>();
            this.Bind<ICardCollection, CardCollection>();
            this.BindToFactory<ICardCollectionFactory>();
            this.Bind<IFaceValue, FaceValue>();
            this.Bind<ISuit, Suit>();
            this.Bind<IDeck, Deck>();
            this.BindToFactory<IVictoryCheckerFactory>();
            this.Bind<IDeckFactory, DeckFactory>();
            this.BindToFactory<IScorerFactory>();
            this.Bind<IScorer, CardGameScorer>();
            this.Bind<IVictoryChecker, VictoryChecker>();
            this.Bind<ICommandHandler<DrawCardsCommand>, DrawCardsCommandHandler>();
            this.Bind<ICommandHandler<UpdateScoresCommand>, UpdateScoresCommandHandler>();
            this.Bind<ICommandHandler<CheckForGameEndCommand>, CheckForGameEndCommandHandler>();
        }

        private void LoadBindings()
        {
            var fileName = Constants.ConfigFilePath;
            if (File.Exists(fileName))
            {
                try
                {
                    this.LoadStoredBindings(fileName);
                }
                catch (Exception)
                {
                    this.LoadDefaultBindings();
                    this.SaveBindings(fileName);
                }
            }
            else
            {
                this.LoadDefaultBindings();
                this.SaveBindings(fileName);
            }
        }

        private void LoadSettings()
        {
            var dependencyResolver = new NinjectDependencyResolver(this.Kernel);
            var settings = CardGameSettings.Create(dependencyResolver);

            this.Bind<PlayerSettings>().ToConstant(
                new PlayerSettings()
                {
                    MinimumPlayerCount = settings.PlayerCountMinimum,
                    MaximumPlayerCount = settings.PlayerCountMaximum,
                    MinimumNameLength = settings.PlayerNameMinimumLength,
                    MaximumNameLength = settings.PlayerNameMaximumLength
                });

            this.Bind<KeyInputSettings>().ToConstant(
                new KeyInputSettings()
                {
                    DrawCardKey = settings.DrawCardKey,
                    ContinueRoundKey = settings.ContinueRoundKey,
                    ExitKey = settings.ExitKey
                });

            this.Bind<DeckSettings>().ToConstant(
                new DeckSettings()
                {
                    Cards = settings.Cards
                });

            this.Bind<ScoringSettings>().ToConstant(
                new ScoringSettings()
                {
                    GetCardValueDelegate = settings.GetCardValueDelegate,
                    WinnerBonusPointTotal = settings.HighestCardScoreChange
                });

            this.Bind<VictoryCheckerSettings>().ToConstant(
                new VictoryCheckerSettings()
                {
                    MinimumVictoryScoreTotal = settings.MinimumVictoryScoreTotal,
                    RequiredScoreLead = settings.RequiredVictoryScoreLead,
                    ScoreLeadMustBeExactValue = settings.VictoryScoreLeadMustBeAnExactValue
                });
        }

        public override void Load()
        {
            this.LoadBindings();
            this.LoadSettings();
        }
    }
}