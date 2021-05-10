using Civ2engine.Enums;

namespace Civ2engine.Units
{
    public class UnitDefinition
    {
        public string Name { get; set; }
        public int Until { get; set; }
        public UnitGAS Domain { get; set; }
        public int Move { get; set; }
        public int Range { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Hitp { get; set; }
        public int Firepwr { get; set; }
        public int Cost { get; set; }
        public int Hold { get; set; }
        public int AIrole { get; set; }
        public int Prereq { get; set; }
        public string Flags { get; set; }
        public Enums.UnitType Type { get; set; }
    }
}