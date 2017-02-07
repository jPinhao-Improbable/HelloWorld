using System.Collections.Generic;
using System.Linq;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;

namespace Assets.Gamelogic.NPC.Wizard.EntityFinders
{
    class MultiEntityFinder : IEntityFinder
    {
        private readonly List<IEntityFinder> finders;

        public MultiEntityFinder(List<IEntityFinder> entityFinders)
        {
            finders = entityFinders;
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
