using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.InteractionStrategies
{
    public class AttackOrDefendStrategy : IStateChangerStrategy
    {
        private readonly AttackEnemyBehaviour attackBehaviour;
        private readonly DefendFriendBehaviour defendBehaviour;

        private IStateChangerStrategy activeBehaviour;

        public AttackOrDefendStrategy(AttackEnemyBehaviour attackBehaviour, DefendFriendBehaviour defendBehaviour)
        {
            this.attackBehaviour = attackBehaviour;
            this.defendBehaviour = defendBehaviour;
            activeBehaviour = new DoNothing();
        }


        public EntityId FindEntity()
        {
            var nearestDefendableTarget = defendBehaviour.FindEntity();
            var defendableDistance = DistanceToTarget(nearestDefendableTarget, defendBehaviour.transform);

            var nearestAttackableTarget = attackBehaviour.FindEntity();
            var attackableDistance = DistanceToTarget(nearestAttackableTarget, attackBehaviour.transform);

            if (attackableDistance <= defendableDistance)
            {
                activeBehaviour = attackBehaviour;
                return nearestAttackableTarget;
            }
            else
            {
                activeBehaviour = defendBehaviour;
                return nearestDefendableTarget;
            }
        }

        private float DistanceToTarget(EntityId targetId, Transform sourceTransform)
        {
            if (targetId == EntityId.InvalidEntityId)
            {
                return float.MaxValue;
            }

            return MathUtils.SqrDistance(sourceTransform.position, NPCUtils.GetTargetGameObject(targetId).transform.position);
        }

        public WizardFSMState.StateEnum TryInteract(GameObject target)
        {
            var newState = activeBehaviour.TryInteract(target);
            if (newState != WizardFSMState.StateEnum.MOVING_TO_TARGET)
            {
                activeBehaviour = new DoNothing();
            }

            return newState;
        }
    }
}
