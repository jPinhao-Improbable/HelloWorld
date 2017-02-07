﻿using System.Collections.Generic;
using Assets.Gamelogic.NPC.Wizard.EntityFinders;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
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
                        new List<WeightedItem<IEntityFinder>>
                        {
                            new WeightedItem<IEntityFinder>(attackBehaviour.EntityFinder, Random.value),
                            new WeightedItem<IEntityFinder>(defendBehaviour.EntityFinder, Random.value)
                        });
            }

            if (attackBehaviour == null && defendBehaviour == null)
            {
                return new NothingFinder();
            }

            return attackBehaviour != null ? attackBehaviour.EntityFinder : defendBehaviour.EntityFinder;
        }
    }
}
