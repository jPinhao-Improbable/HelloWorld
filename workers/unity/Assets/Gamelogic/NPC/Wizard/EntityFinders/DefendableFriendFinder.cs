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
    class DefendableFriendFinder : IEntityFinder
    {
        private readonly GameObject owner;

        public DefendableFriendFinder(GameObject behaviourOwner)
        {
            owner = behaviourOwner;
        }

        public FoundEntity FindEntity()
        {
            var layerMask = ~(1 << LayerMask.NameToLayer(SimulationSettings.TreeLayerName));
            var nearestDefendableTarget = NPCUtils.FindNearestTarget(owner, SimulationSettings.NPCViewRadius, NPCUtils.IsTargetDefendable, layerMask);

            var targetId = nearestDefendableTarget == null
                ? EntityId.InvalidEntityId
                : nearestDefendableTarget.EntityId();

            return new FoundEntity() {distance = NPCUtils.DistanceToTarget(owner, targetId), entity = targetId};
        }
    }
}
