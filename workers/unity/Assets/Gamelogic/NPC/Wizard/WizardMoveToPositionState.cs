using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.FSM;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard
{
    class WizardMoveToPositionState : FsmBaseState<WizardStateMachine, WizardFSMState.StateEnum>
    {
        private readonly WizardBehaviour parentBehaviour;
        private readonly TargetNavigation.Writer targetNavigation;
        private readonly TargetNavigationBehaviour navigation;

        private readonly IEntityFinder newTargetFinder;

        private Coroutine findTargetCoroutine;

        public WizardMoveToPositionState(WizardStateMachine owner,
                                       WizardBehaviour inParentBehaviour,
                                       TargetNavigation.Writer inTargetNavigation,
                                       TargetNavigationBehaviour inNavigation,
                                       IEntityFinder targetFindingStrategy) 
            : base(owner)
        {
            parentBehaviour = inParentBehaviour;
            targetNavigation = inTargetNavigation;
            navigation = inNavigation;

            newTargetFinder = targetFindingStrategy;
        }

        public override void Enter()
        {
            targetNavigation.ComponentUpdated += OnTargetNavigationUpdated;
            findTargetCoroutine = parentBehaviour.StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.NPCPerceptionRefreshInterval, CheckForTarget));
            StartMovingTowardsPosition();
        }

        public override void Tick()
        {
        }

        public override void Exit(bool disabled)
        {
            targetNavigation.ComponentUpdated -= OnTargetNavigationUpdated;
            StopFindTargetCoroutine();
        }

        private void StopFindTargetCoroutine()
        {
            if (findTargetCoroutine != null)
            {
                parentBehaviour.StopCoroutine(findTargetCoroutine);
                findTargetCoroutine = null;
            }
        }

        private void StartMovingTowardsPosition()
        {
            var targetPosition = Owner.Data.targetPosition.ToVector3();
            if (MathUtils.CompareEqualityEpsilon(targetPosition, SimulationSettings.InvalidPosition))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
                return;
            }
            navigation.StartNavigation(targetPosition, SimulationSettings.NPCDefaultInteractionSqrDistance);
        }

        private void CheckForTarget()
        {
            var nearestTarget = newTargetFinder.FindEntity();
            if (EntityId.IsValidEntityId(nearestTarget.entity))
            {
                Owner.TriggerTransition(WizardFSMState.StateEnum.MOVING_TO_TARGET, nearestTarget.entity, SimulationSettings.InvalidPosition);
            }
        }

        private void OnTargetNavigationUpdated(TargetNavigation.Update update)
        {
            if (update.navigationFinished.Count > 0)
            {
                if (Owner.Data.targetEntityId != EntityId.InvalidEntityId)
                {
                    Debug.LogError("Trying to move to POSITION with a target entity set?");
                }

                Owner.TriggerTransition(WizardFSMState.StateEnum.IDLE, EntityId.InvalidEntityId, SimulationSettings.InvalidPosition);
            }
        }
    }
}
