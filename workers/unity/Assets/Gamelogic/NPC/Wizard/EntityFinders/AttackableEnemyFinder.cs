using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.EntityFinders
{
    class AttackableEnemyFinder : IEntityFinder
    {
        private readonly GameObject owner;

        public AttackableEnemyFinder(GameObject behaviourOwner)
        {
            owner = behaviourOwner;
        }

        public FoundEntity FindEntity()
        {
            var layerMask = ~(1 << LayerMask.NameToLayer(SimulationSettings.TreeLayerName));
            var nearestAttackableTarget = NPCUtils.FindNearestTarget(owner, SimulationSettings.NPCViewRadius, NPCUtils.IsTargetAttackable, layerMask);

            var targetId = nearestAttackableTarget == null
                ? EntityId.InvalidEntityId
                : nearestAttackableTarget.EntityId();

            return new FoundEntity() { distance = NPCUtils.DistanceToTarget(owner, targetId), entity = targetId };
        }
    }
}
