using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public class DefendFriendBehaviour : MonoBehaviour, IStateChangerStrategy
    {
        public EntityId FindEntity()
        {
            var layerMask = ~(1 << LayerMask.NameToLayer(SimulationSettings.TreeLayerName));
            var nearestDefendableTarget = NPCUtils.FindNearestTarget(this.gameObject, SimulationSettings.NPCViewRadius, NPCUtils.IsTargetDefendable, layerMask);

            return nearestDefendableTarget == null ? EntityId.InvalidEntityId : nearestDefendableTarget.EntityId();
        }

        public WizardFSMState.StateEnum TryInteract(GameObject target)
        {
            if (target == null || !NPCUtils.IsTargetDefendable(gameObject, target))
            {
                //target is gone, back to idle
                return WizardFSMState.StateEnum.IDLE;
            }
            if (!NPCUtils.IsWithinInteractionRange(transform.position, target.transform.position, SimulationSettings.NPCWizardSpellCastingSqrDistance))
            {
                //target too far, keep moving to target
                return WizardFSMState.StateEnum.MOVING_TO_TARGET;
            }

            return WizardFSMState.StateEnum.DEFENDING_TARGET;
        }
    }
}
