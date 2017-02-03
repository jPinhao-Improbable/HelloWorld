using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public class DoNothing : IStateChangerStrategy
    {
        public EntityId FindEntity()
        {
            return EntityId.InvalidEntityId;
        }

        public WizardFSMState.StateEnum TryInteract(GameObject target)
        {
            return WizardFSMState.StateEnum.MOVING_TO_TARGET;
        }
    }
}
