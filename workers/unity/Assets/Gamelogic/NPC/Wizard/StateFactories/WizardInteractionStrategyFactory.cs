using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.EntityFinders;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.StateFactories
{
    class WizardInteractionStrategyFactory
    {
        public static IInteractionStrategy Build(GameObject owningEntity)
        {
            var attackBehaviour = owningEntity.GetComponent<AttackEnemyBehaviour>();
            var defendBehaviour = owningEntity.GetComponent<DefendFriendBehaviour>();

            if (attackBehaviour != null && defendBehaviour != null)
            {
                return new AttackOrDefendStrategy(attackBehaviour, defendBehaviour);
            }

            if (attackBehaviour == null && defendBehaviour == null)
            {
                return new JustMoveTotarget();
            }

            return attackBehaviour != null ? (IInteractionStrategy) attackBehaviour : defendBehaviour;
        }
    }
}
