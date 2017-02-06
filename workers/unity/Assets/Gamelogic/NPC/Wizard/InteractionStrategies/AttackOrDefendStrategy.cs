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
    public class AttackOrDefendStrategy : IInteractionStrategy
    {
        private readonly AttackEnemyBehaviour attackBehaviour;
        private readonly DefendFriendBehaviour defendBehaviour;

        public AttackOrDefendStrategy(AttackEnemyBehaviour attackBehaviour, DefendFriendBehaviour defendBehaviour)
        {
            this.attackBehaviour = attackBehaviour;
            this.defendBehaviour = defendBehaviour;
        }

        public WizardFSMState.StateEnum TryInteract(GameObject target)
        {
            var newState = attackBehaviour.TryInteract(target);
            if (newState == WizardFSMState.StateEnum.IDLE)
            {
                newState = defendBehaviour.TryInteract(target);
            }
            
            return newState;
        }
    }
}
