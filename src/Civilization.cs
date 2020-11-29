﻿using System.Linq;
using civ2.Enums;

namespace civ2
{
    public class Civilization
    {
        public int Id { get; set; }
        public int CityStyle { get; set; }
        public string LeaderName { get; set; }
        public string TribeName { get; set; }
        public string Adjective { get; set; }
        public int Money { get; set; }
        public int ReseachingTech { get; set; }
        public bool[] Techs { get; set; }
        public int ScienceRate { get; set; }
        public int TaxRate { get; set; }
        public GovernmentType Government { get; set; }

        private int _luxRate;
        public int LuxRate 
        {
            get
            {
                _luxRate = 100 - TaxRate - ScienceRate;
                return _luxRate;
            }
            set
            {
                _luxRate = value;
            }
        }

        private int _population;
        public int Population
        {
            get
            {
                _population = 0;
                foreach (City city in Game.Cities.Where(n => n.Owner == Id))
                {
                    _population += city.Population;
                }
                return _population;
            }
        }
    }
}
