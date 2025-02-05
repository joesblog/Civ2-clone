using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;

namespace Civ2engine
{
    public class History
    {
        private readonly Game _game;

        private List<HistoryEvent> Events = new();
        

        internal History(Game game)
        {
            _game = game;
            
        }

        public void AdvanceDiscovered(int advance, int civ)
        {
            Events.Add(new ResearchEvent(advance, civ, _game));
        }

        public void CityBuilt(City city)
        {
            Events.Add(new CityBuiltEvent(city,_game));
        }

        public int TotalCitiesBuilt(int civId)
        {
            return Events.Count(e => e.Civ == civId && e.EventType == HistoryEventType.CityBuilt);
        }
    }

    public class CityBuiltEvent : HistoryEvent
    {
        public CityBuiltEvent(City city, Game game) : base(HistoryEventType.CityBuilt, city.Owner.Id, game)
        {
            X = city.X;
            Y = city.Y;
            Name = city.Name;
        }

        public string Name { get; }

        public int Y { get;  }

        public int X { get; }
    }

    public class ResearchEvent : HistoryEvent
    {
        public int Advance { get; }

        public ResearchEvent(int advance, int civ, Game game) : base(HistoryEventType.AdvanceDiscovered, civ, game)
        {
            Advance = advance;
        }
    }

    public abstract class HistoryEvent
    {
        public HistoryEventType EventType { get; }
        public int Civ { get; }

        public int Turn { get; }
        protected HistoryEvent(HistoryEventType eventType, int civ, Game game)
        {
            EventType = eventType;
            Civ = civ;
            Turn = game.TurnNumber;
        }

    }
}