using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public interface IInteraction
    {
        WizardFSMState.StateEnum TryInteract(GameObject target);
    }
}