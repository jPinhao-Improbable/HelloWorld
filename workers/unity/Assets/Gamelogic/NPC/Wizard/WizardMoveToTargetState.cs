using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    public class WizardMoveToTargetState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly WizardBehaviour parentBehaviour;
        private readonly TargetNavigation.Writer targetNavigation;
        private readonly TargetNavigationBehaviour navigation;

        private readonly IEntityFinder targetFinder;
        private readonly IInteractionStrategy interactStrategy;

        private Coroutine checkForNewTargetCoroutine;

        public WizardMoveToTargetState(WizardStateMachine owner,
                                       WizardBehaviour inParentBehaviour,
                                       TargetNavigation.Writer inTargetNavigation,
                                       IEntityFinder newTargetFinder,
                                       IInteractionStrategy interactionStrategy)
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            targetNavigation = inTargetNavigation;
            targetFinder = newTargetFinder;
            interactStrategy = interactionStrategy;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated += OnTargetNavigationUpdated;
            checkForNewTargetCoroutine = parentBehaviour.StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.NPCPerceptionRefreshInterval, CheckForNewTarget));
            StartMovingTowardsTarget();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated -= OnTargetNavigationUpdated;
            StopCheckForNewTarget();
        }

        private void StopCheckForNewTarget()
        {
            if (checkForNewTargetCoroutine != null)
            {
                parentBehaviour.StopCoroutine(checkForNewTargetCoroutine);
                checkForNewTargetCoroutine = null;
            }
        }

        private void StartMovingTowardsTarget()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);
            
            var newState = interactStrategy.TryInteract(targetGameObject);
            if (newState != WizardFSMState.StateEnum.MOVING_TO_TARGET)
            {
                //TODO: will IDLE do funny things if targetEntityId != invalid?
                Owner.TriggerTransition(newState, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
            }
        }

        private void CheckForNewTarget()
        {
            var nearestTarget = targetFinder.FindEntity();
            if (EntityId.IsValidEntityId(nearestTarget.entity))
            {
                //check we have a valid entity within range, and it's not the same one we're already targetting
                if (nearestTarget.entity != Owner.Data.targetEntityId)
                {
                    //change target
                    Owner.TriggerTransition(WizardFSMState.StateEnum.MOVING_TO_TARGET, nearestTarget.entity, SimulationSettings.InvalidPosition);
                }
            }
            else
            {
                //no entity found, go back to Idle
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
            }
        }
        
        private void OnTargetNavigationUpdated(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                if (TargetIsEntity())
                {
                    StartMovingTowardsTarget();
                }
                else
                {
                    Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
                }
            }
        }

        private bool TargetIsEntity()
        {
            return EntityId.IsValidEntityId(Owner.Data.targetEntityId);
        }
    }
}
