using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Entities
{
    public class NPC : Entity
    {
        public NPC(int entityID) : base(entityID)
        {
            stats.MinRollActions = 0;
            stats.MaxRollActions = 0;
            stats.BonusActions = 0;
        }
    }
}
