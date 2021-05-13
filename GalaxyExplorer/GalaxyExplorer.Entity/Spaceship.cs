﻿namespace GalaxyExplorer.Entity
{
    public class Spaceship
    {
        public int SpaceshipId { get; set; }
        public string Name { get; set; }
        public double Range { get; set; }
        public bool OnMission { get; set; }
        public int MaxCrewCount { get; set; }
    }
}
