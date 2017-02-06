using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Improbable;
using UnityEngine;

namespace Assets.Gamelogic.NPC.Wizard.EntityFinders
{
    class MultiEntityFinder : IEntityFinder
    {
        private readonly List<IEntityFinder> finders;
        private readonly Transform transform;

        public MultiEntityFinder(List<IEntityFinder> entityFinders, Transform ownerTransform)
        {
            finders = entityFinders;
            transform = ownerTransform;
        }

        public FoundEntity FindEntity()
        {
            var possibleTargets = finders.Select(finder => finder.FindEntity());

            var closestDistance = possibleTargets.Min(foundEntity => foundEntity.distance);
            var target = possibleTargets.First(finder => finder.distance == closestDistance);
            return target;
        }
    }
}
