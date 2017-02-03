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

        private readonly IStateChangerStrategy interactStrategy;

        private Coroutine checkForNearbyEnemiesOrAlliesCoroutine;

        public WizardMoveToTargetState(WizardStateMachine owner,
                                       WizardBehaviour inParentBehaviour,
                                       TargetNavigation.Writer inTargetNavigation,
                                       TargetNavigationBehaviour inNavigation,
                                       IStateChangerStrategy interactionStrategy)
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            targetNavigation = inTargetNavigation;
            navigation = inNavigation;
            interactStrategy = interactionStrategy;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated += OnTargetNavigationUpdated;
            checkForNearbyEnemiesOrAlliesCoroutine = parentBehaviour.StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.NPCPerceptionRefreshInterval, CheckForNearbyEnemiesOrAllies));
            StartMovingTowardsTarget();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated -= OnTargetNavigationUpdated;
            StopCheckForNearbyEnemiesOrAlliesCoroutine();
        }

        private void StopCheckForNearbyEnemiesOrAlliesCoroutine()
        {
            if (checkForNearbyEnemiesOrAlliesCoroutine != null)
            {
                parentBehaviour.StopCoroutine(checkForNearbyEnemiesOrAlliesCoroutine);
                checkForNearbyEnemiesOrAlliesCoroutine = null;
            }
        }

        private void StartMovingTowardsTarget()
        {
            if (TargetIsEntity())
            {
                StartMovingTowardsTargetEntity();
            }
            else
            {
                StartMovingTowardsTargetPosition();
            }
        }

        private void StartMovingTowardsTargetEntity()
        {
            var targetGameObject = NPCUtils.GetTargetGameObject(Owner.Data.targetEntityId);

            var newState = interactStrategy.TryInteract(targetGameObject);
            if (newState == WizardFSMState.StateEnum.MOVING_TO_TARGET)
            {
                //if strategy says we need to keep chasing our target
                navigation.StartNavigation(Owner.Data.targetEntityId, SimulationSettings.NPCWizardSpellCastingSqrDistance);
            }
            else
            {
                //Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
                Owner.TriggerTransition(newState, Owner.Data.targetEntityId, SimulationSettings.InvalidPosition);
            }
        }

        private void StartMovingTowardsTargetPosition()
        {
            var targetPosition = Owner.Data.targetPosition.ToVector3();
            if (MathUtils.CompareEqualityEpsilon(targetPosition, SimulationSettings.InvalidPosition))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
                return;
            }
            navigation.StartNavigation(targetPosition, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        private void CheckForNearbyEnemiesOrAllies()
        {
            var nearestTarget = interactStrategy.FindEntity();
            if (EntityId.IsValidEntityId(nearestTarget))
            {
                if (!TargetIsEntity() || nearestTarget != Owner.Data.targetEntityId)
                {
                    Owner.TriggerTransition(WizardFSMState.StateEnum.MOVING_TO_TARGET, nearestTarget, SimulationSettings.InvalidPosition);
                }
            }
        }
        
        private void OnTargetNavigationUpdated(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                if (TargetIsEntity())
                {
                    StartMovingTowardsTargetEntity();
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
