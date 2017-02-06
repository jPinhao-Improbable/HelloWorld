using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.EntityFinders;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Improbable.Npc;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.StateFactories
{
    public static class WizardEntityFinderFactory
    {
        public static IEntityFinder Build(GameObject owningEntity)
        {
            var attackBehaviour = owningEntity.GetComponent<AttackEnemyBehaviour>();
            var defendBehaviour = owningEntity.GetComponent<DefendFriendBehaviour>();

            if (attackBehaviour != null && defendBehaviour != null)
            {
                return
                    new MultiEntityFinder(
                        new List<IEntityFinder> {attackBehaviour.EntityFinder, defendBehaviour.EntityFinder},
                        owningEntity.transform);
            }

            if (attackBehaviour == null && defendBehaviour == null)
            {
                return new NothingFinder();
            }

            return attackBehaviour != null ? attackBehaviour.EntityFinder : defendBehaviour.EntityFinder;
        }
    }
}
