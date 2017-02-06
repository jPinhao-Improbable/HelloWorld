using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public interface IInteractionStrategy
    {
        WizardFSMState.StateEnum TryInteract(GameObject target);
    }
}