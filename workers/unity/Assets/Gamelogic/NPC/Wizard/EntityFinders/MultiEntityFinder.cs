using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gamelogic.NPC.Wizard.InteractionStrategies;
using Assets.Gamelogic.Utils;
using Improbable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Gamelogic.NPC.Wizard.EntityFinders
{
    class MultiEntityFinder : IEntityFinder
    {
        private readonly List<WeightedItem<IEntityFinder>> finders;

        public MultiEntityFinder(IEnumerable<WeightedItem<IEntityFinder>> entityFinders)
        {
            finders = entityFinders.OrderByDescending(strategy => strategy.Weighting).ToList();
        }

        public FoundEntity FindEntity()
        {
            var possibleTargets = finders.Select(finder => new WeightedItem<FoundEntity>(finder.Item.FindEntity(), finder.Weighting))
                                            .Where(foundTarget => foundTarget.Item.entity != EntityId.InvalidEntityId)
                                            .ToList();

            if (!possibleTargets.Any())
            {
                return new FoundEntity() {distance = float.MaxValue, entity = EntityId.InvalidEntityId};
            }

            var normalizedTargetWeightings = WeightsNormalizer<WeightedItem<FoundEntity>>.Normalize(possibleTargets).ToList();
            var randomSelectionList = RandomSelectionFriendlyFormat(normalizedTargetWeightings);

            var rnd = Random.value;
            var chosenTarget = randomSelectionList.FirstOrDefault(target => target.Weighting > rnd);

            return chosenTarget != null ? chosenTarget.Item : new FoundEntity() {distance = float.MaxValue, entity = EntityId.InvalidEntityId};
        }

        private List<WeightedItem<FoundEntity>> RandomSelectionFriendlyFormat(List<WeightedItem<FoundEntity>> weightedTargets)
        {
            float cumulativeTotal = 0;
            foreach (var target in weightedTargets)
            {
                var targetWeight = target.Weighting;

                target.Weighting += cumulativeTotal;
                cumulativeTotal += targetWeight;
            }

            return weightedTargets;
        }
    }
}
