using Assets.Gamelogic.Core;
using Assets.Gamelogic.NPC.Wizard.EntityFinders;
using Improbable;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    [EngineType(EnginePlatform.FSim)]
    public class DefendFriendBehaviour : MonoBehaviour, IInteractionStrategy
    {
        [SerializeField] private float MinInteractionDistance = SimulationSettings.NPCWizardSpellCastingSqrDistanceMin;
        [SerializeField] private float MaxInteractionDistance = SimulationSettings.NPCWizardSpellCastingSqrDistanceMax;
        private float InteractionDistance;

        public void Start()
        {
            InteractionDistance = Random.Range(MinInteractionDistance, MaxInteractionDistance);
        }

        public IEntityFinder EntityFinder { get; private set; }

        public TargetNavigationBehaviour navigation;

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
            if (!NPCUtils.IsWithinInteractionRange(transform.position, target.transform.position, InteractionDistance))
            {
                //target too far, keep moving to target
                navigation.StartNavigation(target.EntityId(), InteractionDistance);
                return WizardFSMState.StateEnum.MOVING_TO_TARGET;
            }

            return WizardFSMState.StateEnum.DEFENDING_TARGET;
        }
    }
}
