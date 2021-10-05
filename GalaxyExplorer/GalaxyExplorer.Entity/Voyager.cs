﻿using System;
using System.Collections.Generic;

namespace GalaxyExplorer.Entity
{
    public class Voyager
    {
        public int VoyagerId { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public DateTime FirstMissionDate { get; set; }
        public bool OnMission { get; set; }
        public IEnumerable<MissionVoyager> MissionVoyagers { get; set; }
    }
}
