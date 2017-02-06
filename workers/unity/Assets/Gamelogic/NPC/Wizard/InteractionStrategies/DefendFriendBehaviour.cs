using Assets.Gamelogic.Core;
using Assets.Gamelogic.NPC.Wizard.EntityFinders;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public class DefendFriendBehaviour : MonoBehaviour, IInteractionStrategy
    {
        public IEntityFinder EntityFinder { get; private set; }

        public TargetNavigationBehaviour navigation;
        //[SerializeField] private TargetNavigationBehaviour navigation;

        public void Awake()
        {
            EntityFinder = new DefendableFriendFinder(gameObject);
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
                navigation.StartNavigation(target.EntityId(), SimulationSettings.NPCWizardSpellCastingSqrDistance);
                return WizardFSMState.StateEnum.MOVING_TO_TARGET;
            }

            return WizardFSMState.StateEnum.DEFENDING_TARGET;
        }
    }
}
