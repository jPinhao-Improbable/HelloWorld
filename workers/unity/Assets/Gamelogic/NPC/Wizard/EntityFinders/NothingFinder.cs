using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Improbable;

namespace Assets.Gamelogic.NPC.Wizard.EntityFinders
{
    class NothingFinder : IEntityFinder
    {
        public FoundEntity FindEntity()
        {
            return new FoundEntity() {distance = float.MaxValue, entity = EntityId.InvalidEntityId};
        }
    }
}
