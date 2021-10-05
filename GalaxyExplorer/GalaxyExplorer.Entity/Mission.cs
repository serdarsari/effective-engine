using System;
using System.Collections.Generic;

namespace GalaxyExplorer.Entity
{
    public class Mission
    {
        public int MissionId { get; set; }
        public int SpaceshipId { get; set; }
        public string Name { get; set; }
        public int PlannedDuration { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsCompleted { get; set; }
        public IEnumerable<MissionVoyager> MissionVoyagers { get; set; }
    }
}
